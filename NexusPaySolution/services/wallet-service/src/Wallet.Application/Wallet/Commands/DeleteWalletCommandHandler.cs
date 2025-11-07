using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Wallet.Commands
{
    public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand>
    {
        public DeleteWalletCommandHandler(ILoggerService loggerService, IWalletRepository walletRepository, IMediator mediator)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public async Task Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _walletRepository.DeleteWalletAsync(request.UserId);

                await _loggerService.LogInfo($"wallet for user {request.UserId} deleted", "DeleteWalletCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "DeleteWalletCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
