using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;
using Wallet.Infrastructure.Data;

namespace Wallet.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        public WalletRepository(ILoggerService loggerService, WalletDbContext walletDbContext)
        {
            _dbContext = walletDbContext;
            _loggerService = loggerService;
        }

        private readonly WalletDbContext _dbContext;
        private readonly ILoggerService _loggerService;

        public async Task<WalletModel> CreateWalletAsync(string userName, string userEmail, Guid userId)
        {
            try
            {
                WalletModel walletModel = new WalletModel(userName, userEmail, userId);

                await _dbContext.AddAsync(walletModel);

                await _dbContext.SaveChangesAsync();

                await _loggerService.LogInfo($"wallet for user {userEmail} created", "WalletRepository.CreateWalletAsync");

                return walletModel;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "WalletRepository.CreateWalletAsync", e.GetType().FullName);

                throw;
            }
        }

        public async Task DeleteWalletAsync(Guid userId)
        {
            WalletModel? walletModel = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

            if (walletModel == null)
            {
                await _loggerService.LogWarning($"Wallet with id {userId} not found", "WalletRepository.DeleteWalletAsync");

                throw new NotFoundException($"Wallet with id {userId} not found");
            }

            _dbContext.Remove(walletModel);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<decimal> GetWalletBalanceByUserIdAsync(Guid userId)
        {
            WalletModel? walletModel = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

            if (walletModel == null)
            {
                await _loggerService.LogWarning($"Wallet with id {userId} not found", "WalletRepository.GetWalletBalanceByUserIdAsync");

                throw new NotFoundException($"Wallet with id {userId} not found");
            }

            return walletModel.Balance;
        }

        public async Task UpdateBalancesAsync(Guid senderId, Guid receiverId, decimal amount)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    WalletModel? sender = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == senderId);

                    if (sender == null)
                    {
                        await _loggerService.LogWarning($"Wallet with sender id {senderId} not found", "WalletRepository.UpdateBalancesAsync");

                        throw new NotFoundException($"Wallet with sender id {senderId} not found");
                    }

                    sender.GetMoneyFromBalance(amount);


                    WalletModel? receiver = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == receiverId);

                    if (receiver == null)
                    {
                        await _loggerService.LogWarning($"Wallet with receiver id {receiverId} not found", "WalletRepository.UpdateBalancesAsync");

                        throw new NotFoundException($"Wallet with receiver id {receiverId} not found");
                    }

                    receiver.AddMoneyToBalance(amount);

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();

                    await _loggerService.LogError(e.Message, "WalletRepository.UpdateBalancesAsync", e.GetType().FullName);

                    throw;
                }
            }
        }

        public async Task<WalletModel> UpdateUserEmailAsync(string email, Guid userId)
        {
            WalletModel? walletModel = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

            if (walletModel == null)
            {
                await _loggerService.LogWarning($"Wallet with id {userId} not found", "WalletRepository.UpdateUserEmailAsync");

                throw new NotFoundException($"Wallet with id {userId} not found");
            }

            try
            {
                walletModel.UpdateUserEmail(email);

                await _dbContext.SaveChangesAsync();

                return walletModel;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "WalletRepository.UpdateUserEmailAsync", e.GetType().FullName);

                throw;
            }
        }

        public async Task<WalletModel> UpdateUserNameAsync(string name, Guid userId)
        {
            WalletModel? walletModel = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == userId);

            if (walletModel == null)
            {
                await _loggerService.LogWarning($"Wallet with id {userId} not found", "WalletRepository.UpdateUserNameAsync");

                throw new NotFoundException($"Wallet with id {userId} not found");
            }

            try
            {
                walletModel.UpdateUserName(name);

                await _dbContext.SaveChangesAsync();

                return walletModel;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "WalletRepository.UpdateUserNameAsync", e.GetType().FullName);

                throw;
            }
        }
    }
}
