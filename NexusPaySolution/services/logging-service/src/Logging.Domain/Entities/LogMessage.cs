using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Domain.Entities
{
    public class LogMessage : Entity
    {
        public LogMessage(string service, string level, string message, DateTime? time, string action, string? exception = null)
        {
            Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(service)) throw new ArgumentException("Invalid service name");

            ServiceName = service;

            if (string.IsNullOrWhiteSpace(level)) throw new ArgumentException("Invalid log level");

            LogLevel = level;

            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Invalid message");

            Message = message;

            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Invalid action");

            Action = action;

            if (time == null)
            {
                throw new ArgumentException("Invalid time format");
            }

            Timestamp = time.Value;

            Exception = exception;
        }

        private LogMessage()
        {
            
        }

        public string ServiceName { get; private set; }

        public string LogLevel { get; private set; }

        public string Message { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string? Exception { get; private set; }

        public string Action { get; private set; }
    }
}
