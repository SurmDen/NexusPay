using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Command
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
    {
        public CreateNotificationCommandHandler(ILoggerService loggerService, IEmailNotificationRepository emailNotificationRepository)
        {
            _emailNotificationRepository = emailNotificationRepository;
            _loggerService = loggerService;
        }

        private readonly ILoggerService _loggerService;
        private readonly IEmailNotificationRepository _emailNotificationRepository;

        public async Task Handle(CreateNotificationCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                var emailNotification = new Notification.Domain.Entities.EmailNotification(
                    notification.Message,
                    notification.Email,
                    notification.Subject,
                    notification.OccuredOn,
                    notification.IsSuccess);

                await _emailNotificationRepository.AddNotificationAsync(emailNotification);

                await _loggerService.LogInfo("Notification added", "CreateNotificationCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "CreateNotificationCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
