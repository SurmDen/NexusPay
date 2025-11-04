using MediatR;
using Notification.Application.EmailNotification.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Queries
{
    public class GetNotificationsQuery : IRequest<List<EmailNotificationDto>>
    {
        public DateTime? From { get; set; } = null;
    }
}
