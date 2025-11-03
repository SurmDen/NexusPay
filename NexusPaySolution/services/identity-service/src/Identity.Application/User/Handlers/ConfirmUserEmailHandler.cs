using Identity.Application.Interfaces;
using Identity.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Handlers
{
    public class ConfirmUserEventHandler : INotificationHandler<ConfirmUserEvent>
    {
        public ConfirmUserEventHandler(IProducer producer)
        {
            _producer = producer;
        }

        private readonly IProducer _producer;

        public async Task Handle(ConfirmUserEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("user.comfirm", notification);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
