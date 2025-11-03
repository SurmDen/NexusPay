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
        public ConfirmEmailCommandHandler(IUserRepository userRepository, IMediator mediator, IDistributedCache cache, ILoggerService loggerService)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _cache = cache;
            _loggerService = loggerService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        IDistributedCache _cache;
        private readonly ILoggerService _loggerService;

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? codeString = await _cache.GetStringAsync($"email_comfirmation_{request.Email}");

                if (string.IsNullOrEmpty(codeString))
                {
                    await _loggerService.LogWarning($"code, sended to email: {request.Email} not found", "ConfirmEmailCommandHandler.Handle");

                    throw new NotFoundException($"code, sended to email: {request.Email} not found");
                }

                bool isParsed = int.TryParse(codeString, out int code);

                if (!isParsed)
                {
                    await _loggerService.LogWarning("invalid code format", "ConfirmEmailCommandHandler.Handle");

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

                    await _loggerService.LogInfo($"Email confirmed", "ConfirmEmailCommandHandler.Handle");

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "ConfirmEmailCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
