using MediatR;

namespace Wallet.Domain.Events
{
    public class EmailUpdatedEvent : INotification
    {
        public EmailUpdatedEvent(Guid userId, bool success, string email)
        {
            OccuredOn = DateTime.UtcNow;
            UserId = userId;
            Success = success;
            Email = email;
        }

        public Guid UserId { get; private set; }

        public DateTime OccuredOn { get; private set; }

        public bool Success { get; private set; }

        public string Email { get; private set; }
    }
}
