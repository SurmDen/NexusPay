using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Events
{
    public class NameUpdatedEvent : INotification
    {
        public NameUpdatedEvent(Guid userId, bool success, string name)
        {
            OccuredOn = DateTime.UtcNow;
            UserId = userId;
            Success = success;
            Name = name;
        }

        public Guid UserId { get; private set; }

        public DateTime OccuredOn { get; private set; }

        public bool Success { get; private set; }

        public string Name { get; private set; }
    }
}
