using MediatR;
using Notification.Application.Interfaces;
using Notification.Domain.Repositories;

namespace Notification.Application.EmailNotification.Command
{
    public class RemoveNotificationsCommandHandler : IRequestHandler<RemoveNotificationsCommand>
    {
        public RemoveNotificationsCommandHandler(ILoggerService loggerService, IEmailNotificationRepository emailNotificationRepository)
        {
            _emailNotificationRepository = emailNotificationRepository;
            _loggerService = loggerService;
        }

        private readonly ILoggerService _loggerService;
        private readonly IEmailNotificationRepository _emailNotificationRepository;

        public async Task Handle(RemoveNotificationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _emailNotificationRepository.RemoveNotificationsAsync(request.Before);

                await _loggerService.LogInfo($"Notifications before {request.Before.ToShortDateString()} removed", "RemoveNotificationsCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "RemoveNotificationsCommandHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
