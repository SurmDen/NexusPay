using Logging.Application.Interfaces;
using Logging.Domain.Entities;
using Logging.Domain.Repositories;

namespace Logging.Application.Services
{
    public class LoggerService : ILoggerService
    {
        public LoggerService(ILoggingRepository loggingRepository)
        {
            _repository = loggingRepository;
        }

        private readonly ILoggingRepository _repository;

        public async Task LogInfo(string message, string action)
        {
            LogMessage logMessage = new LogMessage("logging-service", "Information", message, DateTime.UtcNow, action);

            await _repository.CreateLogAsync(logMessage);
        }

        public async Task LogWarning(string message, string action)
        {
            LogMessage logMessage = new LogMessage("logging-service", "Warning", message, DateTime.UtcNow, action);

            await _repository.CreateLogAsync(logMessage);
        }

        public async Task LogError(string message, string action, string? exception)
        {
            LogMessage logMessage = new LogMessage("logging-service", "Error", message, DateTime.UtcNow, action, exception);

            await _repository.CreateLogAsync(logMessage);
        }
    }
}
