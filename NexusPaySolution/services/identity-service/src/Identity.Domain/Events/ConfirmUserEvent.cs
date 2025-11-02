using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class ConfirmUserEvent : DomainEvent, INotification
    {
        public ConfirmUserEvent(Guid userId, string email, int code) : base(DateTime.UtcNow)
        {
            UserId = userId;
            Email = email;
            ConfirmationCode = code;
        }

        public Guid UserId { get; private set; }

        public string Email { get; private set; }

        public int ConfirmationCode { get; private set; }
    }
}
