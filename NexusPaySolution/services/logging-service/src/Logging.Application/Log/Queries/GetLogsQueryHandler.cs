using Logging.Application.Interfaces;
using Logging.Application.Log.DTOs;
using Logging.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Application.Log.Queries
{
    public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, List<LogMessageDto>>
    {
        public GetLogsQueryHandler(ILoggerService loggerService, ILoggingRepository loggingRepository)
        {
            _loggerService = loggerService;
            _repository = loggingRepository;
        }

        private readonly ILoggerService _loggerService;
        private readonly ILoggingRepository _repository;

        public async Task<List<LogMessageDto>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var logs = await _repository.GetLogsAsync(request.ServiceName, request.LogLevel, request.From);

                var logsDto = logs.Select(log =>
                {
                    return new LogMessageDto()
                    {
                        ServiceName = log.ServiceName,
                        LogLevel = log.LogLevel,
                        Message = log.Message,
                        Exception = log.Exception,
                        Timestamp = log.Timestamp,
                        Action = log.Action
                    };
                });

                return logsDto.ToList();
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "GetLogsQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
