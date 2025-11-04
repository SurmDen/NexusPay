using Microsoft.EntityFrameworkCore;
using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using Notification.Infrastructure.Data;

namespace Notification.Infrastructure.Repositories
{
    public class EmailNotificationRepository : IEmailNotificationRepository
    {
        public EmailNotificationRepository(NotificationDbContext notificationDbContext, ILoggerService loggerService)
        {
            _context = notificationDbContext;
            _logger = loggerService;
        }

        private readonly NotificationDbContext _context;
        private readonly ILoggerService _logger;

        public async Task AddNotificationAsync(EmailNotification notification)
        {
            string methodName = $"{nameof(EmailNotificationRepository)}.{nameof(AddNotificationAsync)}";

            try
            {
                await _logger.LogInfo($"Adding email notification for: {notification.Email}", methodName);

                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"Email notification added with ID: {notification.Id}", methodName);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Failed to add email notification for: {notification.Email}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<List<EmailNotification>> GetNotificationsAsync(DateTime? from = null)
        {
            string methodName = $"{nameof(EmailNotificationRepository)}.{nameof(GetNotificationsAsync)}";

            try
            {
                await _logger.LogInfo($"Getting email notifications from: {from?.ToString() ?? "all"}", methodName);

                var notifications = _context.EmailNotification.AsNoTracking();

                if (from != null)
                {
                    notifications = notifications.Where(n => n.OccuredOn >= from.Value);
                }

                var result = await notifications.ToListAsync();

                await _logger.LogInfo($"Retrieved {result.Count} email notifications", methodName);
                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogError("Failed to get email notifications", methodName, ex.Message);
                throw;
            }
        }

        public async Task RemoveNotificationsAsync(DateTime before)
        {
            string methodName = $"{nameof(EmailNotificationRepository)}.{nameof(RemoveNotificationsAsync)}";

            try
            {
                await _logger.LogInfo($"Removing email notifications before: {before}", methodName);

                var notifications = await _context.EmailNotification
                    .Where(n => n.OccuredOn < before)
                    .ToListAsync();

                _context.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"Removed {notifications.Count} email notifications before {before}", methodName);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Failed to remove email notifications before {before}", methodName, ex.Message);
                throw;
            }
        }
    }
}
