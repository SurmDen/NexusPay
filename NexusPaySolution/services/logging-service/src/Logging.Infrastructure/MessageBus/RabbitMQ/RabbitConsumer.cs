using Logging.Application.Interfaces;
using Logging.Domain.Events;
using Logging.Infrastructure.MessageBus.Options;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Logging.Infrastructure.MessageBus.RabbitMQ
{
    public class RabbitConsumer : IConsumer, IDisposable
    {
        public RabbitConsumer(IOptions<RabbitMQOptions> options, IMediator mediator)
        {
            _options = options.Value;
            _mediator = mediator;

            string methodName = $"{nameof(RabbitConsumer)}.ctor";

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
            catch (Exception ex)
            {

                throw;
            }
        }

        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IMediator _mediator;

        public async Task Subscribe(string routingKey, string queueName)
        {
            string methodName = $"{nameof(RabbitConsumer)}.{nameof(Subscribe)}";

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
                    string consumerMethodName = $"{nameof(RabbitConsumer)}.ReceivedAsync";

                    try
                    {

                        var body = ea.Body.ToArray();

                        string messageString = Encoding.UTF8.GetString(body);


                        LogReceivedEvent? message = JsonSerializer.Deserialize<LogReceivedEvent>(messageString);

                        if (message != null)
                        {
                            await _mediator.Publish(message);

                            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                        }
                    }
                    catch (Exception ex)
                    {

                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);


                        throw;
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            string methodName = $"{nameof(RabbitConsumer)}.{nameof(Dispose)}";

            _channel?.CloseAsync().Wait();
            _channel?.Dispose();

            _connection?.CloseAsync().Wait();
            _connection?.Dispose();
        }
    }
}
