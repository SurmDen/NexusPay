using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class TransactionDeniedEventHandler : INotificationHandler<TransactionDeniedEvent>
    {
        public TransactionDeniedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(TransactionDeniedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("wallet.transaction.denied", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "TransactionDeniedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
