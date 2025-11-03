using Logging.Application.Interfaces;
using Logging.Domain.Entities;
using Logging.Domain.Events;
using Logging.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.EventHandlers
{
    public class LogReceivedEventHandler : INotificationHandler<LogReceivedEvent>
    {
        public LogReceivedEventHandler(ILoggerService loggerService, ILoggingRepository loggingRepository)
        {
            _loggerService = loggerService;
            _repository = loggingRepository;
        }

        private readonly ILoggingRepository _repository;
        private readonly ILoggerService _loggerService;

        public async Task Handle(LogReceivedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                LogMessage logMessage = new LogMessage
                (
                    service: notification.ServiceName,
                    level: notification.LogLevel,
                    message: notification.Message,
                    time: notification.Timestamp,
                    action: notification.Action,
                    exception: notification.Exception ?? null
                );

                await _repository.CreateLogAsync(logMessage);

                await _loggerService.LogInfo($"Log from {notification.ServiceName} added", "LogReceivedEventHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "LogReceivedEventHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
