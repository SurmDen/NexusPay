using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.Commands
{
    public class DeleteLogsCommand : IRequest
    {
        public DateTime From { get; set; }
    }
}
