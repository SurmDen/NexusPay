using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Application.Wallet.Commands;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class CreateTransactionEventHandler : INotificationHandler<CreateTransactionEvent>
    {
        public CreateTransactionEventHandler(ILoggerService loggerService, IMediator mediator)
        {
            _logger = loggerService;
            _mediator = mediator;
        }

        private readonly ILoggerService _logger;
        private readonly IMediator _mediator;

        public async Task Handle(CreateTransactionEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                CreateTransactionCommand command = new CreateTransactionCommand()
                {
                    SenderUserId = notification.SenderId,
                    ReceiverUserId = notification.ReceiverId,
                    TransactionId = notification.TransactionId,
                    Amount = notification.Amount
                };

                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "CreateTransactionEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
