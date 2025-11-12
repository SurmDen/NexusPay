using Microsoft.EntityFrameworkCore;
using Transaction.Application.Interfaces;
using Transaction.Domain.Entities;
using Transaction.Domain.Exceptions;
using Transaction.Domain.Repositories;
using Transaction.Infrastructure.Data;

namespace Transaction.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public TransactionRepository(ILoggerService loggerService, TransactionDbContext transactionDbContext)
        {
            _loggerService = loggerService;
            _transactionDbContext = transactionDbContext;
        }

        private readonly ILoggerService _loggerService;
        private readonly TransactionDbContext _transactionDbContext;

        public async Task<TransactionInfo> CreateTransactionAsync(Guid senderId, Guid receiverId, decimal amount, string? message = null)
        {
            try
            {
                TransactionInfo transactionInfo = new TransactionInfo(senderId, receiverId, amount, message);

                await _transactionDbContext.Transactions.AddAsync(transactionInfo);

                await _transactionDbContext.SaveChangesAsync();

                return transactionInfo;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "TransactionRepository.CreateTransactionAsync", e.GetType().FullName);

                throw;
            }
        }

        public async Task<List<TransactionInfo>> GetAllTransactionsAsync()
        {
            return await _transactionDbContext.Transactions.AsNoTracking().ToListAsync();
        }

        public async Task<TransactionInfo> GetTransactionByIdAsync(Guid transactionId)
        {
            TransactionInfo? transaction = await _transactionDbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
            {
                await _loggerService.LogWarning($"Transaction with id {transactionId.ToString()} not found", "TransactionRepository.GetTransactionByIdAsync");

                throw new NotFountException($"Transaction with id {transactionId.ToString()} not found");
            }

            return transaction;
        }

        public async Task<List<TransactionInfo>> GetUserTransactionsAsync(Guid userId)
        {
            List<TransactionInfo>? userTransactions = await _transactionDbContext.Transactions
                .AsNoTracking()
                .Where(t => t.SenderUserId == userId || t.ReceiverUserId == userId)
                .ToListAsync();

            if(userTransactions == null || userTransactions.Count == 0)
            {
                await _loggerService.LogWarning($"users transaction list is empty, user Id: {userId.ToString()}", "TransactionRepository.GetUserTransactionsAsync");

                throw new NotFountException($"users transaction list is empty, user Id: {userId.ToString()}");
            }

            return userTransactions;
        }

        public async Task SetStatusToTransactionAsync(Guid transactionId, string status, string? errorMessage = null)
        {
            TransactionInfo? transaction  = await _transactionDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
            {
                await _loggerService.LogWarning($"Transaction with id {transactionId.ToString()} not found", "TransactionRepository.SetStatusToTransactionAsync");

                throw new NotFountException($"Transaction with id {transactionId.ToString()} not found");
            }

            transaction.SetStatus(status);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                transaction.SetError(errorMessage);
            }

            await _transactionDbContext.SaveChangesAsync();
        }
    }
}
