using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Interfaces;
using Wallet.Application.Services;
using Wallet.Domain.Events;

namespace Wallet.Application.Wallet.Handlers
{
    public class NameUpdatedEventHandler : INotificationHandler<NameUpdatedEvent>
    {
        public NameUpdatedEventHandler(IProducer producer, ILoggerService logger)
        {
            _logger = logger;
            _producer = producer;
        }

        private readonly IProducer _producer;
        private readonly ILoggerService _logger;

        public async Task Handle(NameUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.SendObject("wallet.name.updated", notification);
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "NameUpdatedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
