using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;
using Wallet.Domain.Models;

namespace Wallet.Application.Services
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
                ServiceName = "wallet-service",
                Message = message,
                Action = action,
                LogLevel = "Information"
            };

            await _producer.SendObject("logging.wallet", logMessage);
        }

        public async Task LogWarning(string message, string action)
        {
            LogMessage logMessage = new LogMessage()
            {
                ServiceName = "wallet-service",
                Message = message,
                Action = action,
                LogLevel = "Warning"
            };

            await _producer.SendObject("logging.wallet", logMessage);
        }

        public async Task LogError(string message, string action, string? exception)
        {
            LogMessage logMessage = new LogMessage()
            {
                ServiceName = "wallet-service",
                Message = message,
                Action = action,
                LogLevel = "Error",
                Exception = exception
            };

            await _producer.SendObject("logging.wallet", logMessage);
        }
    }
}
