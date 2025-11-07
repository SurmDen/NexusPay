using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Wallet.Commands
{
    public class UpdateUserEmailCommandHandler : IRequestHandler<UpdateUserEmailCommand>
    {
        public UpdateUserEmailCommandHandler(ILoggerService loggerService, IWalletRepository walletRepository, IMediator mediator)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public async Task Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletRepository.UpdateUserEmailAsync(request.UserEmail, request.UserId);

                foreach (var ev in wallet.GetEvents())
                {
                    await _mediator.Publish(ev);
                }

                await _loggerService.LogInfo($"user email for wallet {wallet.Id} updated", "UpdateUserEmailCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "UpdateUserEmailCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
