using Wallet.Domain.Entities;

namespace Wallet.Domain.Repositories
{
    public interface IWalletRepository
    {
        public Task<decimal> GetWalletBalanceByUserIdAsync(Guid userId);

        public Task<WalletModel> CreateWalletAsync(string userName, string userEmail, Guid userId);

        public Task UpdateBalancesAsync(Guid senderId, Guid receiverId, decimal amount);

        public Task<WalletModel> UpdateUserNameAsync(string name, Guid userId);

        public Task<WalletModel> UpdateUserEmailAsync(string email, Guid userId);

        public Task DeleteWalletAsync(Guid userId);
    }
}
