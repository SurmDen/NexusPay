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
        public RabbitProducer(IOptions<RabbitMQOptions> options, ILoggerService logger)
        {
            _rabbitmqOptions = options.Value;
            _logger = logger;

            string methodName = $"{nameof(RabbitProducer)}.ctor";

            try
            {
                _logger.LogInfo($"Initializing RabbitMQ producer for host: {_rabbitmqOptions.Host}:{_rabbitmqOptions.Port}", methodName).Wait();

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

                _logger.LogInfo($"RabbitMQ producer initialized successfully. Exchange: {_rabbitmqOptions.ExchangeName}", methodName).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize RabbitMQ producer", methodName, ex.Message).Wait();
                throw;
            }
        }

        private readonly RabbitMQOptions _rabbitmqOptions;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILoggerService _logger;

        public async Task SendMessage(string routingKey, string message)
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(SendMessage)}";

            try
            {
                await _logger.LogInfo($"Sending message to routing key: {routingKey}, message length: {message?.Length ?? 0}", methodName);

                if (string.IsNullOrEmpty(message))
                {
                    await _logger.LogWarning("Attempted to send empty message", methodName);
                    throw new ArgumentException("Message cannot be null or empty", nameof(message));
                }

                if (string.IsNullOrEmpty(routingKey))
                {
                    await _logger.LogWarning("Attempted to send message with empty routing key", methodName);
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                var messageBytes = Encoding.UTF8.GetBytes(message);

                await _channel.BasicPublishAsync(
                    exchange: _rabbitmqOptions.ExchangeName,
                    routingKey: routingKey,
                    body: messageBytes
                );

                await _logger.LogInfo($"Message sent successfully to routing key: {routingKey}", methodName);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Failed to send message to routing key: {routingKey}", methodName, ex.Message);
                throw;
            }
        }

        public async Task SendObject<T>(string routingKey, T obj)
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(SendObject)}";

            try
            {
                await _logger.LogInfo($"Serializing and sending object of type {typeof(T).Name} to routing key: {routingKey}", methodName);

                if (obj == null)
                {
                    await _logger.LogWarning("Attempted to send null object", methodName);
                    throw new ArgumentNullException(nameof(obj));
                }

                if (string.IsNullOrEmpty(routingKey))
                {
                    await _logger.LogWarning("Attempted to send object with empty routing key", methodName);
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                string message = JsonSerializer.Serialize<T>(obj);
                await SendMessage(routingKey, message);

                await _logger.LogInfo($"Object of type {typeof(T).Name} sent successfully to routing key: {routingKey}", methodName);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Failed to send object of type {typeof(T).Name} to routing key: {routingKey}", methodName, ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            string methodName = $"{nameof(RabbitProducer)}.{nameof(Dispose)}";

            try
            {
                _logger.LogInfo("Disposing RabbitMQ producer resources", methodName).Wait();

                _channel?.CloseAsync().Wait();
                _channel?.Dispose();

                _connection?.CloseAsync().Wait();
                _connection?.Dispose();

                _logger.LogInfo("RabbitMQ producer resources disposed successfully", methodName).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error disposing RabbitMQ producer resources", methodName, ex.Message).Wait();
            }
        }
    }
}
