using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Infrastructure.MessageBus.Options
{
    public class RabbitMQOptions
    {
        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public string ExchangeName { get; set; } = "nexus.events";

        public string ExchangeType { get; set; } = "direct";
    }
}
