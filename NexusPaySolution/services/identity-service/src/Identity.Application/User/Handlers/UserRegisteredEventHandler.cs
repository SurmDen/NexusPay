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
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        public UserRegisteredEventHandler(IProducer producer)
        {
            _producer = producer;
        }

        private readonly IProducer _producer;

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            await _producer.SendObject("user.registered", notification);
        }
    }
}
