using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class UserEmailUpdatedEvent : DomainEvent, INotification
    {
        public UserEmailUpdatedEvent(Guid id, string email) : base(DateTime.UtcNow)
        {
            UserEmail = email;
            UserId = id;
        }

        public Guid UserId { get; private set; }

        public string UserEmail { get; private set; }
    }
}
