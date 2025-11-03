using Identity.Application.Interfaces;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.User.Commands
{
    public class UpdateUsersEmailCommandHandler : IRequestHandler<UpdateUsersEmailCommand>
    {
        public UpdateUsersEmailCommandHandler(IUserRepository userRepository, IMediator mediator, ILoggerService loggerService)
        {
            _mediator = mediator;
            _loggerService = loggerService;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        public async Task Handle(UpdateUsersEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Email email = new Email(request.Email);

                var user = await _userRepository.UpdateUserEmailAsync(request.UserId, email);

                foreach (var userEvent in user.DomainEvents)
                {
                    await _mediator.Publish(userEvent);
                }

                user.ClearEventList();

                await _loggerService.LogInfo($"User with id: {request.UserId} email updated", "UpdateUsersEmailCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "UpdateUsersEmailCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
