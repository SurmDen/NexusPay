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
    public class UpdateUsersNameCommandHandler : IRequestHandler<UpdateUsersNameCommand>
    {
        public UpdateUsersNameCommandHandler(IUserRepository userRepository, IMediator mediator, ILoggerService loggerService)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _loggerService = loggerService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        public async Task Handle(UpdateUsersNameCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.UpdateUserNameAsync(request.UserId, request.UserName);

                foreach (var userEvent in user.DomainEvents)
                {
                    await _mediator.Publish(userEvent);
                }

                user.ClearEventList();

                await _loggerService.LogInfo($"User with id: {request.UserId} name updated", "UpdateUsersNameCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "UpdateUsersNameCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
