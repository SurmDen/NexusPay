using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Command
{
    public class CreateNotificationCommand : IRequest
    {
        public string Email { get;  set; } = string.Empty;

        public string Subject { get;  set; } = string.Empty;

        public string Message { get;  set; } = string.Empty;

        public DateTime OccuredOn { get; set; }

        public bool IsSuccess { get; set; }
    }
}
