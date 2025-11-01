using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class UserRegisteredEvent : DomainEvent, INotification
    {
        public UserRegisteredEvent(string name, string email, Guid id) : base(DateTime.UtcNow)
        {
            UserName = name;
            Email = email;
            UserId = id;
        }

        public string UserName { get; private set; }

        public string Email { get; private set; }

        public Guid UserId { get; private set; }
    }
}
