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
    public class UserDeactivatedEventHandler : INotificationHandler<UserDeactivatedEvent>
    {
        public UserDeactivatedEventHandler(IProducer producer)
        {
            _producer = producer;
        }

        private readonly IProducer _producer;

        public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("user.deactivated", notification);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
