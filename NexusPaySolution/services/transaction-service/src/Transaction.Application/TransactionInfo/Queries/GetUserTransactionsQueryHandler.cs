using MediatR;
using Transaction.Application.Interfaces;
using Transaction.Application.TransactionInfo.DTOs;
using Transaction.Domain.Repositories;

namespace Transaction.Application.TransactionInfo.Queries
{
    public class GetUserTransactionsQueryHandler : IRequestHandler<GetUserTransactionsQuery, List<TransactionDto>>
    {
        public GetUserTransactionsQueryHandler(ILoggerService logger, ITransactionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private readonly ILoggerService _logger;
        private readonly ITransactionRepository _repository;

        public async Task<List<TransactionDto>> Handle(GetUserTransactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userTransactions = await _repository.GetUserTransactionsAsync(request.UserId);

                var transactionDtos = userTransactions.Select(t => new TransactionDto()
                {
                    Amount = t.Amount,
                    Message = t.Message,
                    ErrorMessage = t.ErrorMessage,
                    Status = t.Status,
                    Time = t.TransactionTime
                });

                return transactionDtos.ToList();
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "GetUserTransactionsQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
