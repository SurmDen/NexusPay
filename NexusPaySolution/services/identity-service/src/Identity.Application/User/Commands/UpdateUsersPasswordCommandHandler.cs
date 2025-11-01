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
    public class UpdateUsersPasswordCommandHandler : IRequestHandler<UpdateUsersPasswordCommand>
    {
        public UpdateUsersPasswordCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public async Task Handle(UpdateUsersPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Password password = new Password(request.Password);

                var user = await _userRepository.UpdateUserPasswordAsync(request.UserId, password);

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
