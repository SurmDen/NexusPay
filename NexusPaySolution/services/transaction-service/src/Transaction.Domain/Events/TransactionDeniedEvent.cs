using MediatR;

namespace Transaction.Domain.Events
{
    public class TransactionDeniedEvent : INotification
    {
        public Guid TransactionId { get; set; }

        public DateTime OccuredOn { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
