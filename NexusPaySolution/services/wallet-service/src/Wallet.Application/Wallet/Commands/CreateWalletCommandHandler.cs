using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;
using Wallet.Domain.Repositories;

namespace Wallet.Application.Wallet.Commands
{
    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand>
    {
        public CreateWalletCommandHandler(ILoggerService loggerService, IWalletRepository walletRepository, IMediator mediator)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;   
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public async Task Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletRepository.CreateWalletAsync(request.UserName, request.UserEmail, request.UserId);

                foreach (var ev in wallet.GetEvents())
                {
                    await _mediator.Publish(ev);
                }

                await _loggerService.LogInfo($"wallet for user {request.UserId} created", "CreateWalletCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError($"wallet for user {request.UserEmail} not created", "CreateWalletCommandHandler.Handle", e.Message);

                throw;
            }
        }
    }
}
