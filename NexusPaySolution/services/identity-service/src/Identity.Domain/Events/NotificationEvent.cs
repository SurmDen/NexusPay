using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class NotificationEvent : DomainEvent, INotification
    {
        public NotificationEvent(string subject, string email, string body) : base(DateTime.UtcNow)
        {
            Subject = subject;
            Email = email;
            Body = body;
        }

        public string Subject { get; set; }

        public string Email { get; private set; }

        public string Body { get; private set; }
    }
}
