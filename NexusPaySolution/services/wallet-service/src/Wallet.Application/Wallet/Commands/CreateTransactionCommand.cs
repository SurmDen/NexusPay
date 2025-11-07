using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Wallet.Commands
{
    public class CreateTransactionCommand : IRequest
    {
        public Guid TransactionId { get; set; }

        public Guid SenderUserId { get; set; }

        public Guid ReceiverUserId { get; set; }

        public decimal Amount { get; set; }
    }
}
