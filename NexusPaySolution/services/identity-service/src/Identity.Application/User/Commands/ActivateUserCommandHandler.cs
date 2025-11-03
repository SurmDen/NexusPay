using Identity.Application.Interfaces;
using Identity.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
    {
        public ActivateUserCommandHandler(IUserRepository userRepository, IMediator mediator, ILoggerService loggerService)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _loggerService = loggerService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.ActivateUserAsync(request.UserId);

                foreach (var userEvent in user.DomainEvents)
                {
                    await _mediator.Publish(userEvent);
                }

                user.ClearEventList();

                await _loggerService.LogInfo($"User with id {request.UserId} activated", "ActivateUserCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "ActivateUserCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
