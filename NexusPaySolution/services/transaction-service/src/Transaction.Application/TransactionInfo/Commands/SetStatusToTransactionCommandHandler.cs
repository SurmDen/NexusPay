using MediatR;
using System.Transactions;
using Transaction.Application.Interfaces;
using Transaction.Domain.Repositories;

namespace Transaction.Application.TransactionInfo.Commands
{
    public class SetStatusToTransactionCommandHandler : IRequestHandler<SetStatusToTransactionCommand>
    {
        public SetStatusToTransactionCommandHandler(ILoggerService logger, ITransactionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private readonly ILoggerService _logger;
        private readonly ITransactionRepository _repository;

        public async Task Handle(SetStatusToTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ErrorMessage != null)
                {
                    await _repository.SetStatusToTransactionAsync(request.TransactionId, request.Status, request.ErrorMessage);
                }
                else
                {
                    await _repository.SetStatusToTransactionAsync(request.TransactionId, request.Status);
                }

                await _logger.LogInfo($"transaction with id: {request.TransactionId} sets to status {request.Status}", "CreateTransactionCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "SetStatusToTransactionCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
