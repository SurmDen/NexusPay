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
    public class UpdateUserNameCommandHandler : IRequestHandler<UpdateUserNameCommand>
    {
        public UpdateUserNameCommandHandler(ILoggerService loggerService, IWalletRepository walletRepository, IMediator mediator)
        {
            _loggerService = loggerService;
            _walletRepository = walletRepository;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;

        public async Task Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletRepository.UpdateUserNameAsync(request.UserName, request.UserId);

                foreach (var ev in wallet.GetEvents())
                {
                    await _mediator.Publish(ev);
                }

                await _loggerService.LogInfo($"user name for wallet {wallet.Id} updated", "UpdateUserNameCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "UpdateUserNameCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
