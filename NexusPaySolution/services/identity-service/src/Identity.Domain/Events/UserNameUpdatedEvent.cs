using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class UserNameUpdatedEvent : DomainEvent, INotification
    {
        public UserNameUpdatedEvent(Guid id, string name) : base(DateTime.UtcNow)
        {
            UserId = id;
            UserName = name;
        }

        public string UserName { get; private set; }

        public Guid UserId { get; private set; }
    }
}
