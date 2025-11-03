using Logging.Application.Log.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.Queries
{
    public class GetLogsQuery : IRequest<List<LogMessageDto>>
    {
        public DateTime? From { get; set; } = null;

        public string? ServiceName { get; set; } = null;

        public string? LogLevel { get; set; } = null;
    }
}
