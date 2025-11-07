using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Events
{
    public class TransactionDeniedEvent : INotification
    {
        public TransactionDeniedEvent(Guid transactionId, string message)
        {
            TransactionId = transactionId;
            Message = message;
            OccuredOn = DateTime.UtcNow;
        }

        public Guid TransactionId { get; set; }

        public DateTime OccuredOn { get; set; }

        public string Message { get; set; }
    }
}
