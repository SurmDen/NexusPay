using Logging.Application.Interfaces;
using Logging.Domain.Entities;
using Logging.Domain.Repositories;
using Logging.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logging.Infrastructure.Repositories
{
    public class LoggingRepository : ILoggingRepository
    {
        public LoggingRepository(ILoggerService loggerService, LoggingDbContext loggingDbContext)
        {
            _context = loggingDbContext;
            _loggerService = loggerService;
        }

        private readonly LoggingDbContext _context;
        private readonly ILoggerService _loggerService;

        public async Task CreateLogAsync(LogMessage logMessage)
        {
            _context.LogMessages.Add(logMessage);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogsBeforeAsync(DateTime date)
        {
            string methodName = $"{nameof(LoggingRepository)}.{nameof(DeleteLogsBeforeAsync)}";

            try
            {
                await _loggerService.LogInfo($"Deleting logs before: {date}", methodName);

                var logsToDelete = _context.LogMessages.Where(x => x.Timestamp < date);

                int count = await logsToDelete.CountAsync();

                _context.LogMessages.RemoveRange(logsToDelete);

                await _context.SaveChangesAsync();

                await _loggerService.LogInfo($"Deleted {count} logs before {date}", methodName);
            }
            catch (Exception ex)
            {
                await _loggerService.LogError($"Failed to delete logs before {date}", methodName, ex.Message);

                throw;
            }
        }

        public async Task<List<LogMessage>> GetLogsAsync(string? serviceName = null, string? logLevel = null, DateTime? from = null)
        {
            string methodName = $"{nameof(LoggingRepository)}.{nameof(GetLogsAsync)}";

            try
            {
                await _loggerService.LogInfo($"Getting logs with filters - Service: {serviceName ?? "any"}, Level: {logLevel ?? "any"}, From: {from?.ToString() ?? "any"}", methodName);

                var query = _context.LogMessages.AsQueryable();

                if (!string.IsNullOrEmpty(serviceName))
                {
                    query = query.Where(x => x.ServiceName == serviceName);
                }

                if (!string.IsNullOrEmpty(logLevel))
                {
                    query = query.Where(x => x.LogLevel == logLevel);
                }

                if (from.HasValue)
                {
                    query = query.Where(x => x.Timestamp >= from.Value);
                }

                var logs = await query.ToListAsync();

                await _loggerService.LogInfo($"Retrieved {logs.Count} logs", methodName);

                return logs;
            }
            catch (Exception ex)
            {
                await _loggerService.LogError("Failed to get logs", methodName, ex.Message);

                throw;
            }
        }
    }
}
