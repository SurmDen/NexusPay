using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.EmailNotification.Command;
using Notification.Application.EmailNotification.Queries;
using Notification.Application.Interfaces;

namespace Notification.API.Controllers
{
    //port 5003
    [ApiController]
    [Route("api/v1/notification")]
    public class NotificationController : ControllerBase
    {
        public NotificationController(ILoggerService loggerService, IMediator mediator)
        {
            _loggerService = loggerService;
            _mediator = mediator;
        }

        private readonly ILoggerService _loggerService;
        private readonly IMediator _mediator;

        //[Authorize(Admin)]
        [HttpGet]
        public async Task<IActionResult> GetNotificationsAsync([FromQuery] DateTime? from = null)
        {
            string methodName = $"{nameof(NotificationController)}.{nameof(GetNotificationsAsync)}";

            try
            {
                await _loggerService.LogInfo($"Getting email notifications from: {from?.ToString() ?? "all"}", methodName);

                var query = new GetNotificationsQuery { From = from };
                var notifications = await _mediator.Send(query);

                await _loggerService.LogInfo($"Returned {notifications.Count} email notifications", methodName);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                await _loggerService.LogError("Failed to get email notifications", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(Admin)]
        [HttpDelete("cleanup")]
        public async Task<IActionResult> RemoveNotificationsAsync([FromQuery] DateTime before)
        {
            string methodName = $"{nameof(NotificationController)}.{nameof(RemoveNotificationsAsync)}";

            try
            {
                await _loggerService.LogInfo($"Removing email notifications before: {before}", methodName);

                var command = new RemoveNotificationsCommand { Before = before };
                await _mediator.Send(command);

                await _loggerService.LogInfo($"Email notifications before {before} removed successfully", methodName);
                return Ok(new { message = "Email notifications removed successfully", code = 200 });
            }
            catch (Exception ex)
            {
                await _loggerService.LogError($"Failed to remove email notifications before {before}", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(Admin)]
        [HttpDelete("cleanup/days")]
        public async Task<IActionResult> RemoveNotificationsOlderThanDaysAsync([FromQuery] int days)
        {
            string methodName = $"{nameof(NotificationController)}.{nameof(RemoveNotificationsOlderThanDaysAsync)}";

            try
            {
                if (days <= 0)
                {
                    await _loggerService.LogWarning($"Invalid days parameter: {days}", methodName);
                    return BadRequest(new { message = "Days must be greater than 0", code = 400 });
                }

                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                await _loggerService.LogInfo($"Removing email notifications older than {days} days (before: {cutoffDate})", methodName);

                var command = new RemoveNotificationsCommand { Before = cutoffDate };
                await _mediator.Send(command);

                await _loggerService.LogInfo($"Email notifications older than {days} days removed successfully", methodName);
                return Ok(new { message = $"Email notifications older than {days} days removed successfully", code = 200 });
            }
            catch (Exception ex)
            {
                await _loggerService.LogError($"Failed to remove email notifications older than {days} days", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }
    }
}
