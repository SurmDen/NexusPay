using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        public ConfirmEmailCommandHandler(IUserRepository userRepository, IMediator mediator, IDistributedCache cache)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _cache = cache;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        IDistributedCache _cache;

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? codeString = await _cache.GetStringAsync($"email_comfirmation_{request.Email}");

                if (string.IsNullOrEmpty(codeString))
                {
                    throw new NotFoundException($"code, sended to email: {request.Email} not found");
                }

                bool isParsed = int.TryParse(codeString, out int code);

                if (!isParsed)
                {
                    throw new ArgumentException("invalid code format");
                }

                if (code == request.Code)
                {
                    var user = await _userRepository.ActivateUserAsync(request.UserId);

                    foreach (var userEvent in user.DomainEvents)
                    {
                        await _mediator.Publish(userEvent);
                    }

                    user.ClearEventList();

                    _cache.Remove($"email_comfirmation_{request.Email}");

                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
