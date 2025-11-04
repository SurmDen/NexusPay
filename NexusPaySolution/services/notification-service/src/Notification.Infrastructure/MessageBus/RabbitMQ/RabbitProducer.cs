using Microsoft.Extensions.Options;
using Notification.Application.Interfaces;
using Notification.Infrastructure.MessageBus.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Notification.Infrastructure.MessageBus.RabbitMQ
{
    public class RabbitProducer : IProducer, IDisposable
    {
        public RabbitProducer(IOptions<RabbitMQOptions> options)
        {
            _rabbitmqOptions = options.Value;

            string methodName = $"{nameof(RabbitProducer)}.ctor";

            try
            {

                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitmqOptions.Host,
                    Port = _rabbitmqOptions.Port,
                    UserName = _rabbitmqOptions.UserName,
                    Password = _rabbitmqOptions.Password
                };

                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                _channel.ExchangeDeclareAsync(
                    exchange: _rabbitmqOptions.ExchangeName,
                    type: _rabbitmqOptions.ExchangeType,
                    durable: true,
                    autoDelete: false
                ).Wait();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private readonly RabbitMQOptions _rabbitmqOptions;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public async Task SendMessage(string routingKey, string message)
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(SendMessage)}";

            try
            {

                if (string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("Message cannot be null or empty", nameof(message));
                }

                if (string.IsNullOrEmpty(routingKey))
                {
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                var messageBytes = Encoding.UTF8.GetBytes(message);

                await _channel.BasicPublishAsync(
                    exchange: _rabbitmqOptions.ExchangeName,
                    routingKey: routingKey,
                    body: messageBytes
                );

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendObject<T>(string routingKey, T obj)
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(SendObject)}";

            try
            {

                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }

                if (string.IsNullOrEmpty(routingKey))
                {
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                string message = JsonSerializer.Serialize<T>(obj);
                await SendMessage(routingKey, message);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Dispose()
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(Dispose)}";

            _channel?.CloseAsync().Wait();
            _channel?.Dispose();

            _connection?.CloseAsync().Wait();
            _connection?.Dispose();
        }
    }
}
