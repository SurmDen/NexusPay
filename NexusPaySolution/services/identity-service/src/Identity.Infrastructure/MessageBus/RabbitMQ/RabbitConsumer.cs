using Identity.Application.Interfaces;
using Identity.Infrastructure.MessageBus.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Identity.Infrastructure.MessageBus.RabbitMQ
{
    public class RabbitConsumer : IConsumer, IDisposable
    {
        public RabbitConsumer(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;

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

        public async Task Subscribe<T>(string routingKey, string queueName, Action<T> handler)
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

                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
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


                        T? message = JsonSerializer.Deserialize<T>(messageString);

                        if (message == null)
                        {
                            throw new InvalidOperationException($"Couldn't deserialize message string to type {typeof(T).Name}");
                        }


                        handler(message);

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

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
