using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface IProducer
    {
        public Task SendMessage(string routingKey, string message);

        public Task SendObject<T>(string routingKey, T obj);
    }
}
