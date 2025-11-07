using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class TransactionCompletedEventHandler : INotificationHandler<TransactionCompletedEvent>
    {
        public TransactionCompletedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(TransactionCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("wallet.transaction.completed", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "TransactionCompletedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
