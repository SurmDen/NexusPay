using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using Notification.Infrastructure.MessageBus.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification.Infrastructure.MessageBus.RabbitMQ
{
    public class RabbitConsumer : IConsumer, IDisposable
    {
        public RabbitConsumer(IOptions<RabbitMQOptions> options, IMediator mediator, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _mediator = mediator;

            try
            {

                var factory = new ConnectionFactory()
                {
                    HostName = _options.Host,
                    Port = _options.Port,
                    UserName = _options.UserName,
                    Password = _options.Password
                };

                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                _channel.ExchangeDeclareAsync(
                    exchange: _options.ExchangeName,
                    type: _options.ExchangeType,
                    durable: true,
                    autoDelete: false
                ).Wait();

            }
            catch (Exception)
            {
                throw;
            }

            _serviceProvider = serviceProvider;
        }

        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public async Task Subscribe<T>(string routingKey, string queueName)
        {
            try
            {

                if (string.IsNullOrEmpty(routingKey))
                {
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                if (string.IsNullOrEmpty(queueName))
                {
                    throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));
                }


                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                await _channel.QueueBindAsync(
                    queue: queueName,
                    exchange: _options.ExchangeName,
                    routingKey: routingKey
                );

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {

                    try
                    {
                        var body = ea.Body.ToArray();

                        string messageString = Encoding.UTF8.GetString(body);


                        T? message = JsonSerializer.Deserialize<T>(messageString);

                        if (message != null)
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                                await mediator.Publish(message);
                            }

                            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                        }
                    }
                    catch (Exception)
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);

                        throw;
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.CloseAsync().Wait();
            _channel?.Dispose();

            _connection?.CloseAsync().Wait();
            _connection?.Dispose();
        }
    }
}
