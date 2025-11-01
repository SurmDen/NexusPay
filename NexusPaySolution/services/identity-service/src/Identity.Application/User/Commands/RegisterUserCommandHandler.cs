using Identity.Domain.Repositories;
using Identity.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.ValueObjects;

namespace Identity.Application.User.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        public RegisterUserCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Password password = new Password(request.Password);

                Email email = new Email(request.Email);

                var user = new Domain.Entities.User(request.UserName, email, password);

                var registeredUser = await _userRepository.CreateUserAsync(user);

                foreach (var userEvents in registeredUser.DomainEvents)
                {
                    await _mediator.Publish(userEvents);
                }

                user.ClearEventList();

                return registeredUser.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
