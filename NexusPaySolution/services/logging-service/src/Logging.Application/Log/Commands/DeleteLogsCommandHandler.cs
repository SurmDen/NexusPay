using Logging.Application.Interfaces;
using Logging.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.Commands
{
    public class DeleteLogsCommandHandler : IRequestHandler<DeleteLogsCommand>
    {
        public DeleteLogsCommandHandler(ILoggerService loggerService, ILoggingRepository loggingRepository)
        {
            _loggerService = loggerService;
            _repository = loggingRepository;
        }

        private readonly ILoggerService _loggerService;
        private readonly ILoggingRepository _repository;

        public async Task Handle(DeleteLogsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.DeleteLogsBeforeAsync(request.From);

                await _loggerService.LogInfo($"Logs from {request.From.ToShortDateString()} deleted", "DeleteLogsCommandHandler.Handle");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "DeleteLogsCommandHandler.Handle", e.GetType().FullName);

                throw;
            }   
        }
    }
}
