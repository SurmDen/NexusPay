using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Wallet.Commands
{
    public class UpdateUserNameCommand : IRequest
    {
        public string UserName { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
