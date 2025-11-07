using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class WalletCreatedEventHandler : INotificationHandler<WalletCreatedEvent>
    {
        public WalletCreatedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(WalletCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("wallet.created", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "WalletCreatedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
