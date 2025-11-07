using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Wallet.Queries
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, decimal>
    {
        public GetBalanceQueryHandler(ILoggerService loggerService, IWalletRepository walletRepository)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;

        public async Task<decimal> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _walletRepository.GetWalletBalanceByUserIdAsync(request.UserId);
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "GetBalanceQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
