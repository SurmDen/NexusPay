using MediatR;
using Transaction.Application.Interfaces;
using Transaction.Domain.Events;

namespace Transaction.Application.TransactionInfo.Handlers
{
    public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
    {
        public TransactionCreatedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("transaction.created", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "TransactionCreatedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
