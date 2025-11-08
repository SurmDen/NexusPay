using MediatR;

namespace Transaction.Domain.Events
{
    public class TransactionCompletedEvent : INotification
    {
        public Guid TransactionId { get; set; }

        public DateTime OccuredOn { get; set; }
    }
}
