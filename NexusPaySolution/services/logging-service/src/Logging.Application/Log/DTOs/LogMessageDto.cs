using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.DTOs
{
    public class LogMessageDto
    {
        public string ServiceName { get; set; } = string.Empty;

        public string LogLevel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string? Exception { get; set; }

        public string Action { get; set; } = string.Empty;
    }
}
