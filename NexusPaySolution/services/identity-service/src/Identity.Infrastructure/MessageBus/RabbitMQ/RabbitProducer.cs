using Identity.Application.Interfaces;
using Identity.Infrastructure.MessageBus.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Identity.Infrastructure.MessageBus.RabbitMQ
{
    public class RabbitProducer : IProducer, IDisposable
    {
        public RabbitProducer(IOptions<RabbitMQOptions> options)
        {
            _rabbitmqOptions = options.Value;

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
                );
        }

        private readonly RabbitMQOptions _rabbitmqOptions;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public async Task SendMessage(string routingKey, string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            try
            {
                await _channel.BasicPublishAsync
                (
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
            string message = JsonSerializer.Serialize<T>(obj);
            await SendMessage(routingKey, message);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
