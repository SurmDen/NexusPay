using Notification.Application.Interfaces;
using Notification.Domain.Models;

namespace Notification.Application.Services
{
    public class LoggerService : ILoggerService
    {
        public LoggerService(IProducer producer)
        {
            _producer = producer;
        }

        private readonly IProducer _producer;

        public async Task LogInfo(string message, string action)
        {
            LogMessage logMessage = new LogMessage()
            {
                ServiceName = "notification-service",
                Message = message,
                Action = action,
                LogLevel = "Information"
            };

            await _producer.SendObject("logging", logMessage);
        }

        public async Task LogWarning(string message, string action)
        {
            LogMessage logMessage = new LogMessage()
            {
                ServiceName = "notification-service",
                Message = message,
                Action = action,
                LogLevel = "Warning"
            };

            await _producer.SendObject("logging", logMessage);
        }

        public async Task LogError(string message, string action, string? exception)
        {
            LogMessage logMessage = new LogMessage()
            {
                ServiceName = "notification-service",
                Message = message,
                Action = action,
                LogLevel = "Error",
                Exception = exception
            };

            await _producer.SendObject("logging", logMessage);
        }
    }
}
