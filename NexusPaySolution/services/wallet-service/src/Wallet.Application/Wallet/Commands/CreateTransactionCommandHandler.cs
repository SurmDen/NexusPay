using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Events;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Wallet.Commands
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand>
    {
        public CreateTransactionCommandHandler(ILoggerService loggerService, IWalletRepository walletRepository, IMediator mediator)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public async Task Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _walletRepository.UpdateBalancesAsync(request.SenderUserId, request.ReceiverUserId, request.Amount);

                TransactionCompletedEvent transactionCompletedEvent = new TransactionCompletedEvent(request.TransactionId);

                await _mediator.Publish(transactionCompletedEvent);

                await _loggerService.LogInfo($"transaction {request.TransactionId} completed", "CreateTransactionCommandHandler.Handle");
            }
            catch (Exception e)
            {
                TransactionDeniedEvent transactionDeniedEvent = new TransactionDeniedEvent(request.TransactionId, e.Message);

                await _mediator.Publish(transactionDeniedEvent);

                await _loggerService.LogError(e.Message, "CreateTransactionCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
