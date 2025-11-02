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
    public class UserEmailUpdatedEventHandler : INotificationHandler<UserEmailUpdatedEvent>
    {
        public UserEmailUpdatedEventHandler(IProducer producer)
        {
            _producer = producer;
        }

        private readonly IProducer _producer;

        public async Task Handle(UserEmailUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("user.email.updated", notification);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
