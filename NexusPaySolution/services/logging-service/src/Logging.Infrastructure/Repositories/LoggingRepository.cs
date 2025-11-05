using Logging.Application.Interfaces;
using Logging.Domain.Entities;
using Logging.Domain.Repositories;
using Logging.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logging.Infrastructure.Repositories
{
    public class LoggingRepository : ILoggingRepository
    {
        public LoggingRepository(LoggingDbContext loggingDbContext)
        {
            _context = loggingDbContext;
        }

        private readonly LoggingDbContext _context;

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

                var logsToDelete = _context.LogMessages.Where(x => x.Timestamp < date);

                int count = await logsToDelete.CountAsync();

                _context.LogMessages.RemoveRange(logsToDelete);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<LogMessage>> GetLogsAsync(string? serviceName = null, string? logLevel = null, DateTime? from = null)
        {

            try
            {

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

                return logs;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
