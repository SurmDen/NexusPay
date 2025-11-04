using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.EmailNotification.Command
{
    public class RemoveNotificationsCommand : IRequest
    {
        [Required]
        public DateTime Before { get; set; }
    }
}
