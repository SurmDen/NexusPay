using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface IEmailNotificationSender
    {
        public Task SendAsync(string email, string subject, string message);
    }
}
