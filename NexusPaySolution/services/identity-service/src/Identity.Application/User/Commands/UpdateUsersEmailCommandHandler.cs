using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class UpdateUsersEmailCommandHandler : IRequestHandler<UpdateUsersEmailCommand>
    {
        public UpdateUsersEmailCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

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
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
