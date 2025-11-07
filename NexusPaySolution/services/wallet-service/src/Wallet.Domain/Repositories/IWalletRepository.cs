using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Entities;

namespace Wallet.Domain.Repositories
{
    public interface IWalletRepository
    {
        public Task<WalletModel> GetWalletByUserIdAsync(Guid userId);

        public Task<WalletModel> CreateWalletAsync(string valute, string userName, string userEmail, Guid userId);

        public Task UpdateBalancesAsync(Guid senderId, Guid receiverId, decimal amount);

        public Task<WalletModel> UpdateUserNameAsync(string name, Guid userId);

        public Task<WalletModel> UpdateUserEmailAsync(string email, Guid userId);

        public Task DeleteWalletAsync(Guid userId);

        public Task BlockWalletAsync(Guid walletId);

        public Task UnblockWalletAsync(Guid walletId);
    }
}
