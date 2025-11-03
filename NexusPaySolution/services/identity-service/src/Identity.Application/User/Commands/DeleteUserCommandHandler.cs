using Identity.Application.Interfaces;
using Identity.Domain.Events;
using Identity.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        public DeleteUserCommandHandler(IUserRepository userRepository, IMediator mediator, ILoggerService loggerService)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _loggerService = loggerService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.DeleteUserAsync(request.UserId);

                await _mediator.Publish(new UserDeletedEvent(user.Id));

                await _loggerService.LogInfo($"User with id: {request.UserId} deleted", "DeleteUserCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "DeleteUserCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
