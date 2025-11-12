using MediatR;

namespace Transaction.Application.TransactionInfo.Commands
{
    public class SetStatusToTransactionCommand : IRequest
    {
        public Guid TransactionId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }
}
