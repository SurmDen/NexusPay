using MediatR;
using Notification.Application.EmailNotification.DTOs;
using Notification.Application.Interfaces;
using Notification.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Queries
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<EmailNotificationDto>>
    {
        public GetNotificationsQueryHandler(ILoggerService loggerService, IEmailNotificationRepository emailNotificationRepository)
        {
            _emailNotificationRepository = emailNotificationRepository;
            _loggerService = loggerService;
        }

        private readonly ILoggerService _loggerService;
        private readonly IEmailNotificationRepository _emailNotificationRepository;

        public async Task<List<EmailNotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var notifications = await _emailNotificationRepository.GetNotificationsAsync(request.From);

                var dtoList = notifications.Select(n =>
                {
                    return new EmailNotificationDto()
                    {
                        Email = n.Email,
                        Subject = n.Subject,
                        Body = n.Body,
                        OccuredOn = n.OccuredOn,
                        IsSuccess = n.IsSuccess,
                    };
                });

                return dtoList.ToList();
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "GetNotificationsQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
