using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; protected set; }

        public string Body { get; protected set; } = string.Empty;

        public DateTime OccuredOn { get; protected set; }

        public bool IsSuccess { get; set; }
    }
}
