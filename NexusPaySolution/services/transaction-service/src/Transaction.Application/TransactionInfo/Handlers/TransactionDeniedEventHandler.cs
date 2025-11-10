using MediatR;
using Transaction.Application.Interfaces;
using Transaction.Application.TransactionInfo.Commands;
using Transaction.Domain.Events;

namespace Transaction.Application.TransactionInfo.Handlers
{
    public class TransactionDeniedEventHandler : INotificationHandler<TransactionDeniedEvent>
    {
        public TransactionDeniedEventHandler(IMediator mediator, ILoggerService logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public async Task Handle(TransactionDeniedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                SetStatusToTransactionCommand command = new SetStatusToTransactionCommand()
                {
                    TransactionId = notification.TransactionId,
                    Status = "denied"
                };

                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "TransactionDeniedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
