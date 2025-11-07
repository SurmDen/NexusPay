using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Wallet.Queries
{
    public class GetBalanceQuery : IRequest<decimal>
    {
        public Guid UserId { get; set; }
    }
}
