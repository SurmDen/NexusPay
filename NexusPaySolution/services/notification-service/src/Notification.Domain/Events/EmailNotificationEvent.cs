using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Events
{
    public class EmailNotificationEvent : INotification 
    {
        public string Email { get;  set; } = string.Empty;

        public string Subject { get;  set; } = string.Empty;

        public string Body { get;  set; } = string.Empty;

        public DateTime OccuredOn { get; set; }
    }
}
