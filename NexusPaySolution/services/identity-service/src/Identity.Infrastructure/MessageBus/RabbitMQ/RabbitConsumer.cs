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
        public RabbitConsumer(IOptions<RabbitMQOptions> options, ILoggerService logger)
        {
            _options = options.Value;
            _logger = logger;

            string methodName = $"{nameof(RabbitConsumer)}.ctor";

            try
            {
                _logger.LogInfo($"Initializing RabbitMQ consumer for host: {_options.Host}:{_options.Port}", methodName).Wait();

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

                _logger.LogInfo($"RabbitMQ consumer initialized successfully. Exchange: {_options.ExchangeName}", methodName).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize RabbitMQ consumer", methodName, ex.Message).Wait();
                throw;
            }
        }

        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILoggerService _logger;

        public async Task Subscribe<T>(string routingKey, string queueName, Action<T> handler)
        {
            string methodName = $"{nameof(RabbitConsumer)}.{nameof(Subscribe)}";

            try
            {
                await _logger.LogInfo($"Subscribing to queue: {queueName} with routing key: {routingKey} for type {typeof(T).Name}", methodName);

                if (string.IsNullOrEmpty(routingKey))
                {
                    await _logger.LogWarning("Attempted to subscribe with empty routing key", methodName);
                    throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));
                }

                if (string.IsNullOrEmpty(queueName))
                {
                    await _logger.LogWarning("Attempted to subscribe with empty queue name", methodName);
                    throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));
                }

                if (handler == null)
                {
                    await _logger.LogWarning("Attempted to subscribe with null handler", methodName);
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
                        await _logger.LogInfo($"Received message from queue: {queueName}, delivery tag: {ea.DeliveryTag}", consumerMethodName);

                        var body = ea.Body.ToArray();
                        string messageString = Encoding.UTF8.GetString(body);

                        await _logger.LogInfo($"Message received, length: {messageString.Length} bytes", consumerMethodName);

                        T? message = JsonSerializer.Deserialize<T>(messageString);

                        if (message == null)
                        {
                            await _logger.LogError($"Failed to deserialize message to type {typeof(T).Name}", consumerMethodName, $"Message string: {messageString}");
                            throw new InvalidOperationException($"Couldn't deserialize message string to type {typeof(T).Name}");
                        }

                        await _logger.LogInfo($"Successfully deserialized message to type {typeof(T).Name}", consumerMethodName);

                        handler(message);

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                        await _logger.LogInfo($"Message processed successfully, delivery tag: {ea.DeliveryTag}", consumerMethodName);
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogError($"Error processing message, delivery tag: {ea.DeliveryTag}", consumerMethodName, ex.Message);

                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);

                        await _logger.LogWarning($"Message NACKed and requeued, delivery tag: {ea.DeliveryTag}", consumerMethodName);

                        throw;
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

                await _logger.LogInfo($"Successfully subscribed to queue: {queueName} with routing key: {routingKey}", methodName);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Failed to subscribe to queue: {queueName} with routing key: {routingKey}", methodName, ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            string methodName = $"{nameof(RabbitConsumer)}.{nameof(Dispose)}";

            try
            {
                _logger.LogInfo("Disposing RabbitMQ consumer resources", methodName).Wait();

                _channel?.CloseAsync().Wait();
                _channel?.Dispose();

                _connection?.CloseAsync().Wait();
                _connection?.Dispose();

                _logger.LogInfo("RabbitMQ consumer resources disposed successfully", methodName).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error disposing RabbitMQ consumer resources", methodName, ex.Message).Wait();
            }
        }
    }
}
