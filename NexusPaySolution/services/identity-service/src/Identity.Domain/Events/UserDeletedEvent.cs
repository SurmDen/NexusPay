using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class UserDeletedEvent : DomainEvent, INotification
    {
        public UserDeletedEvent(Guid userId) : base(DateTime.UtcNow)
        {
            UserId = userId;
        }

        public Guid UserId { get; private set; }
    }
}
