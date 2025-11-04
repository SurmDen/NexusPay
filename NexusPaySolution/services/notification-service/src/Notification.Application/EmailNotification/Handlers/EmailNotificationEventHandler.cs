using MediatR;
using Notification.Application.EmailNotification.Command;
using Notification.Application.Interfaces;
using Notification.Domain.Events;
using Notification.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Handlers
{
    public class EmailNotificationEventHandler : INotificationHandler<EmailNotificationEvent>
    {
        public EmailNotificationEventHandler(IEmailNotificationSender sender, ILoggerService loggerService, IMediator mediator)
        {
            _loggerService = loggerService;
            _sender = sender;
            _mediator = mediator;
        }

        private readonly IEmailNotificationSender _sender;
        private readonly ILoggerService _loggerService;
        private readonly IMediator _mediator;

        public async Task Handle(EmailNotificationEvent notification, CancellationToken cancellationToken)
        {
            CreateNotificationCommand command = new CreateNotificationCommand()
            {
                OccuredOn = notification.OccuredOn,
                Message = notification.Body,
                Subject = notification.Subject,
                Email = notification.Email
            };

            try
            {
                await _sender.SendAsync(notification.Email, notification.Subject, notification.Body);

                command.IsSuccess = true;

                await _mediator.Send(command);

                await _loggerService.LogInfo($"Email notification sended to {notification.Email}", "EmailNotificationEventHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "EmailNotificationEventHandler.Handle", e.GetType().FullName);

                command.IsSuccess = false;

                await _mediator.Send(command);

                throw;
            }
        }
    }
}
