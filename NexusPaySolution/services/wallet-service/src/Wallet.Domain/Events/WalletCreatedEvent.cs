using MediatR;

namespace Wallet.Domain.Events
{
    public class WalletCreatedEvent : INotification
    {
        public WalletCreatedEvent(Guid userId, bool success)
        {
            OccuredOn = DateTime.UtcNow;
            UserId = userId;
            Success = success;
        }

        public DateTime OccuredOn { get; private set; }

        public Guid UserId { get; private set; }

        public bool Success { get; private set; }
    }
}
