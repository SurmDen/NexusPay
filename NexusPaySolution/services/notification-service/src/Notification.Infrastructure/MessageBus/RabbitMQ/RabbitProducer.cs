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
            catch (Exception)
            {
                throw;
            }
        }

        private readonly RabbitMQOptions _rabbitmqOptions;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public async Task SendMessage(string routingKey, string message)
        {

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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendObject<T>(string routingKey, T obj)
        {

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
