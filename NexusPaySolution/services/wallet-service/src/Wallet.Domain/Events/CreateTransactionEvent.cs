using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Events
{
    public class CreateTransactionEvent : INotification
    {
        public Guid TransactionId { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        public DateTime OccuredOn { get; set; }

        public decimal Amount { get; set; }
    }
}
