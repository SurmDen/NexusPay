using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Exceptions
{
    public class InvalidWalletOperationException : Exception
    {
        public InvalidWalletOperationException(string message) : base(message) { }
    }
}
