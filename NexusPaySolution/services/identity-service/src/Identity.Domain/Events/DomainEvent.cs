using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public class DomainEvent
    {
        public DomainEvent(DateTime date)
        {
            OccuredOn = date;
        }

        public DateTime OccuredOn { get; private set; }
    }
}
