using Logging.Domain.Entities;

namespace Logging.Domain.Repositories
{
    public interface ILoggingRepository
    {
        public Task CreateLogAsync(LogMessage logMessage);

        public Task<List<LogMessage>> GetLogsAsync(string? serviceName = null, string? logLevel = null, DateTime? from = null);

        public Task DeleteLogsBeforeAsync(DateTime date);
    }
}
