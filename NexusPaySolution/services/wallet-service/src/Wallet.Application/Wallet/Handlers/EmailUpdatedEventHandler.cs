using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class EmailUpdatedEventHandler : INotificationHandler<EmailUpdatedEvent>
    {
        public EmailUpdatedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(EmailUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("wallet.email.updated", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "EmailUpdatedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
