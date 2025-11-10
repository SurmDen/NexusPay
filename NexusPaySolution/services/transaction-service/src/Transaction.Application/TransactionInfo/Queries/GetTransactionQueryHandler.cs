using MediatR;
using Transaction.Application.Interfaces;
using Transaction.Application.TransactionInfo.DTOs;
using Transaction.Domain.Repositories;

namespace Transaction.Application.TransactionInfo.Queries
{
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto>
    {
        public GetTransactionQueryHandler(ILoggerService logger, ITransactionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private readonly ILoggerService _logger;
        private readonly ITransactionRepository _repository;

        public async Task<TransactionDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _repository.GetTransactionByIdAsync(request.TransactionId);

                TransactionDto transactionDto = new TransactionDto()
                {
                    Amount = transaction.Amount,
                    Message = transaction.Message,
                    ErrorMessage = transaction.ErrorMessage,
                    Status = transaction.Status,
                    Time = transaction.TransactionTime
                };

                return transactionDto;
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "GetTransactionQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
