using MediatR;
using Wallet.Application.Interfaces;
using Wallet.Application.Wallet.Commands;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        public UserRegisteredEventHandler(ILoggerService loggerService, IMediator mediator)
        {
            _logger = loggerService;
            _mediator = mediator;
        }

        private readonly ILoggerService _logger;
        private readonly IMediator _mediator;


        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                CreateWalletCommand createWalletCommand = new CreateWalletCommand()
                {
                    UserEmail = notification.Email,
                    UserId = notification.UserId,
                    UserName = notification.UserName
                };

                await _mediator.Send(createWalletCommand);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "UserRegisteredEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
