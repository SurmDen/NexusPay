using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Interfaces
{
    public interface IConsumer
    {
        public Task Subscribe(string routingKey, string queueName);
    }
}
