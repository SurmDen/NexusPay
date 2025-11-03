using Logging.Application.Interfaces;
using Logging.Application.Log.Commands;
using Logging.Application.Log.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Logging.API.Controllers
{
    [ApiController]
    [Route("api/v1/logs")]
    public class LogsController : ControllerBase
    {
        public LogsController(ILoggerService loggerService, IMediator mediator)
        {
            _loggerService = loggerService;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IMediator _mediator;

        [HttpGet]
        public async Task<IActionResult> GetLogsAsync([FromQuery] string? serviceName = null, [FromQuery] string? logLevel = null, [FromQuery] DateTime? from = null)
        {
            string methodName = $"{nameof(LogsController)}.{nameof(GetLogsAsync)}";

            try
            {
                await _loggerService.LogInfo($"Getting logs with filters - Service: {serviceName ?? "any"}, Level: {logLevel ?? "any"}, From: {from?.ToString() ?? "any"}", methodName);

                var query = new GetLogsQuery
                {
                    ServiceName = serviceName,
                    LogLevel = logLevel,
                    From = from
                };

                var logs = await _mediator.Send(query);

                await _loggerService.LogInfo($"Returned {logs.Count} logs", methodName);

                return Ok(logs);
            }
            catch (Exception ex)
            {
                await _loggerService.LogError("Failed to get logs", methodName, ex.Message);

                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpDelete("cleanup")]
        public async Task<IActionResult> DeleteOldLogsAsync([FromQuery] DateTime before)
        {
            string methodName = $"{nameof(LogsController)}.{nameof(DeleteOldLogsAsync)}";

            try
            {
                await _loggerService.LogInfo($"Deleting logs before: {before}", methodName);

                var command = new DeleteLogsCommand { From = before };

                await _mediator.Send(command);

                await _loggerService.LogInfo($"Logs before {before} deleted successfully", methodName);

                return Ok(new { message = "Logs deleted successfully", code = 200 });
            }
            catch (Exception ex)
            {
                await _loggerService.LogError($"Failed to delete logs before {before}", methodName, ex.Message);

                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpDelete("cleanup/days")]
        public async Task<IActionResult> DeleteLogsOlderThanDaysAsync([FromQuery] int days)
        {
            string methodName = $"{nameof(LogsController)}.{nameof(DeleteLogsOlderThanDaysAsync)}";

            try
            {
                if (days <= 0)
                {
                    await _loggerService.LogWarning($"Invalid days parameter: {days}", methodName);

                    return BadRequest(new { message = "Days must be greater than 0", code = 400 });
                }

                var cutoffDate = DateTime.UtcNow.AddDays(-days);

                await _loggerService.LogInfo($"Deleting logs older than {days} days (before: {cutoffDate})", methodName);

                var command = new DeleteLogsCommand { From = cutoffDate };

                await _mediator.Send(command);

                await _loggerService.LogInfo($"Logs older than {days} days deleted successfully", methodName);

                return Ok(new { message = $"Logs older than {days} days deleted successfully", code = 200 });
            }
            catch (Exception ex)
            {
                await _loggerService.LogError($"Failed to delete logs older than {days} days", methodName, ex.Message);

                return Problem(title: "Internal server error", statusCode: 500);
            }
        }
    }
}
