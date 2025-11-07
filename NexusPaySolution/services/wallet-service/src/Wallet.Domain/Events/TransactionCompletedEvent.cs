using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Events
{
    public class TransactionCompletedEvent : INotification
    {
        public TransactionCompletedEvent(Guid transactionId)
        {
            TransactionId = transactionId;
            OccuredOn = DateTime.UtcNow;
        }

        public Guid TransactionId { get; private set; }

        public DateTime OccuredOn { get; private set; }
    }
}
