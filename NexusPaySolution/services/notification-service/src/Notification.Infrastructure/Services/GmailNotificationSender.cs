using Notification.Application.Interfaces;

namespace Notification.Infrastructure.Services
{
    public class EmailNotificationSender : IEmailNotificationSender
    {
        public async Task SendAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}
