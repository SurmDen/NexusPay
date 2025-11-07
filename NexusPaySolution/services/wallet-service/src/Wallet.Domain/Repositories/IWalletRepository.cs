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
        public Task<WalletModel> CreateWalletAsync();

        public Task UpdateBalancesAsync(Guid senderId, Guid receiverId, decimal amount);

        public Task<WalletModel> UpdateUserNameAsync(string name, Guid userId);

        public Task<WalletModel> UpdateUserEmailAsync(string email, Guid userId);

        public Task BlockWalletId(Guid walletId);

        public Task UnblockWalletId(Guid walletId);
    }
}
