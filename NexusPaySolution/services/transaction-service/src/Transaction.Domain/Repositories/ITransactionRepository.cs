using Transaction.Domain.Entities;

namespace Transaction.Domain.Repositories
{
    public interface ITransactionRepository
    {
        public Task<TransactionInfo> CreateTransactionAsync(Guid senderId, Guid receiverId, decimal amount, string? message = null);

        public Task<List<TransactionInfo>> GetAllTransactionsAsync();

        public Task<TransactionInfo> GetUserTransactionsAsync(Guid userId);

        public Task SetStatusToTransactionAsync(Guid transactionId, string? errorMessage = null);
    }
}
