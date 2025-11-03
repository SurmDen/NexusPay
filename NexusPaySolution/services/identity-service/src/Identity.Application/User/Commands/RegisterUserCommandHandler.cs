using Identity.Domain.Repositories;
using MediatR;
using Identity.Domain.ValueObjects;
using Identity.Application.User.DTOs;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Identity.Application.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        public RegisterUserCommandHandler(IUserRepository userRepository, IMediator mediator, ICodeGenerator codeGenerator, IDistributedCache cache, ILoggerService loggerService)
        {
            _mediator = mediator;
            _codeGenerator = codeGenerator;
            _userRepository = userRepository;
            _cache = cache;
            _loggerService = loggerService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ICodeGenerator _codeGenerator;
        IDistributedCache _cache;
        private readonly ILoggerService _loggerService;

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Password password = new Password(request.Password);

                Email email = new Email(request.Email);

                RoleName roleName = new RoleName("User");

                var user = new Domain.Entities.User(request.UserName, email, password, roleName);

                var registeredUser = await _userRepository.CreateUserAsync(user);

                int code = _codeGenerator.GenerateCode();

                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                await _cache.SetStringAsync($"email_comfirmation_{user.UserEmail.Value}", $"{code}", cacheOptions);

                registeredUser.AddConfirmationEvent(code);

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
                    UpdatedAt = registeredUser.UpdatedAt,
                    RoleName = registeredUser.RoleName.Value
                };

                await _loggerService.LogInfo($"User {request.Email} registered", "RegisterUserCommandHandler.Handle");

                return userDto;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "RegisterUserCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
