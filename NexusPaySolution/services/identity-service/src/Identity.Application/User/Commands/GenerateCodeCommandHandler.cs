using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Events;
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
    public class GenerateCodeCommandHandler : IRequestHandler<GenerateCodeCommand>
    {
        public GenerateCodeCommandHandler(IMediator mediator, IDistributedCache cache, ICodeGenerator codeGenerator, ILoggerService loggerService)
        {
            _mediator = mediator;
            _cache = cache;
            _codeGenerator = codeGenerator;
            _loggerService = loggerService;
        }

        private readonly IMediator _mediator;
        private readonly IDistributedCache _cache;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ILoggerService _loggerService;

        public async Task Handle(GenerateCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                int code = _codeGenerator.GenerateCode();

                var cacheOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                await _cache.SetStringAsync($"email_comfirmation_{request.Email}", $"{code}", cacheOptions);

                NotificationEvent confirmUserEvent = new NotificationEvent("Nexus Pay", request.Email, $"<h1>Добро пожаловать!</h1><p>Ваш код: <b>{code}</b></p>");

                await _mediator.Publish(confirmUserEvent);

                await _loggerService.LogInfo("Code generated", "GenerateCodeCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "GenerateCodeCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
