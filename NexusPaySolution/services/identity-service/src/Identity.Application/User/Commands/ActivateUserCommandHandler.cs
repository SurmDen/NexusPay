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
        public ActivateUserCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

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
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
