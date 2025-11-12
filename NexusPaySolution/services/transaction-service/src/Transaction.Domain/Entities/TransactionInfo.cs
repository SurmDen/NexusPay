namespace Transaction.Domain.Entities
{
    public class TransactionInfo : Entity
    {
        public TransactionInfo(Guid senderId, Guid receiverId, decimal amount, string? message = null)
        {
            Status = "pending";

            TransactionTime = DateTime.UtcNow;

            Id = Guid.NewGuid();

            if (senderId == Guid.Empty)
            {
                throw new ArgumentException($"Empty sender Id");
            }

            SenderUserId = senderId;

            if (receiverId == Guid.Empty)
            {
                throw new ArgumentException($"Empty receiver Id");
            }

            ReceiverUserId = receiverId;

            if (amount <= 0)
            {
                throw new ArgumentException($"Sum must be more than 0");
            }

            Amount = amount;

            if (message != null)
            {
                Message = message;
            }
        }

        private TransactionInfo()
        {
            
        }

        public DateTime TransactionTime { get; private set; }

        public decimal Amount { get; private set; }

        public string? Message { get; private set; }

        public Guid SenderUserId { get; private set; }

        public Guid ReceiverUserId { get; private set; }

        public string Status { get; private set; }

        public string? ErrorMessage { get; private set; }


        public void SetStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentNullException("invalid transaction status");
            }

            Status = status;
        }

        public void SetError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentNullException("error message was null");
            }

            ErrorMessage = error;
        }
    }
}
