using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.DTOs
{
    public class EmailNotificationDto
    {
        public string Email { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }

        public DateTime OccuredOn { get; set; }
    }
}
