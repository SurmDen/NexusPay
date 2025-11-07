using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Wallet.Commands
{
    public class UpdateUserEmailCommand : IRequest
    {
        public Guid UserId { get; set; }

        public string UserEmail { get; set; } = string.Empty;
    }
}
