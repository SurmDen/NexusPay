using Identity.Domain.Repositories;
using MediatR;
using Identity.Domain.ValueObjects;
using Identity.Application.User.DTOs;

namespace Identity.Application.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        public RegisterUserCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Password password = new Password(request.Password);

                Email email = new Email(request.Email);

                RoleName roleName = new RoleName("User");

                var user = new Domain.Entities.User(request.UserName, email, password, roleName);

                var registeredUser = await _userRepository.CreateUserAsync(user);

                foreach (var userEvents in registeredUser.DomainEvents)
                {
                    await _mediator.Publish(userEvents);
                }

                user.ClearEventList();

                UserDto userDto = new UserDto()
                {
                    UserName = registeredUser.UserName,
                    Email = registeredUser.UserEmail.Value,
                    IsActive = registeredUser.IsActive,
                    Id = registeredUser.Id,
                    CreatedAt = registeredUser.CreatedAt,
                    UpdatedAt = registeredUser.UpdatedAt
                };

                return userDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
