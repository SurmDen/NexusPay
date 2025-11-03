using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Domain.Events
{
    public class LogReceivedEvent : INotification
    {
        public string ServiceName { get; set; } = string.Empty;

        public string LogLevel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string? Exception { get; set; }

        public string Action { get; set; } = string.Empty;
    }
}
