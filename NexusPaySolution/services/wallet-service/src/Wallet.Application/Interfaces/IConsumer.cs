using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Interfaces
{
    public interface IConsumer
    {
        public Task Subscribe<T>(string routingKey, string queueName);
    }
}
