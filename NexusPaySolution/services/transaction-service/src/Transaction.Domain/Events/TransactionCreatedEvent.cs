using MediatR;

namespace Transaction.Domain.Events
{
    public class TransactionCreatedEvent : INotification
    {
        public TransactionCreatedEvent(Guid transactionId, Guid senderId, Guid receiverId, decimal amount)
        {
            TransactionId = transactionId;
            SenderId = senderId;
            SenderId = receiverId;
            Amount = amount;

            OccuredOn = DateTime.UtcNow;
        }

        public Guid TransactionId { get; private set; }

        public Guid SenderId { get; private set; }

        public Guid ReceiverId { get; private set; }

        public DateTime OccuredOn { get; private set; }

        public decimal Amount { get; private set; }
    }
}
