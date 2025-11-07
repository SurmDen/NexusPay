using MediatR;

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
