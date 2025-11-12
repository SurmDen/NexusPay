using MediatR;
using Transaction.Application.Interfaces;
using Transaction.Domain.Events;
using Transaction.Domain.Repositories;

namespace Transaction.Application.TransactionInfo.Commands
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
    {
        public CreateTransactionCommandHandler(IMediator mediator, ILoggerService logger, ITransactionRepository repository)
        {
            _logger = logger;
            _mediator = mediator;
            _repository = repository;
        }

        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;
        private readonly ITransactionRepository _repository;

        public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _repository.CreateTransactionAsync(request.SenderId, request.ReceiverId, request.Amount, request.Message);

                Guid transactionId = transaction.Id;

                TransactionCreatedEvent createdEvent = new TransactionCreatedEvent(transaction.Id, transaction.SenderUserId, transaction.ReceiverUserId, transaction.Amount);

                await _mediator.Publish(createdEvent);

                await _logger.LogInfo($"transaction with id: {transactionId} created, status pending", "CreateTransactionCommandHandler.Handle");

                return transactionId;
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "CreateTransactionCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
