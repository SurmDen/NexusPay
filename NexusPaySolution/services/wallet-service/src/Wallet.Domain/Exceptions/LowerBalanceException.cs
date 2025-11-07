using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Exceptions
{
    public class LowerBalanceException : Exception
    {
        public LowerBalanceException(string message) : base(message) { }
    }
}
