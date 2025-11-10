using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Application.TransactionInfo.Commands
{
    public class SetStatusToTransactionCommand : IRequest
    {
        public Guid TransactionId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; } = string.Empty;
    }
}
