using Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Repositories
{
    public interface IEmailNotificationRepository
    {
        public Task AddNotificationAsync(EmailNotification notification);

        public Task<List<EmailNotification>> GetNotificationsAsync(DateTime? from = null);

        public Task RemoveNotificationsAsync(DateTime before);
    }
}
