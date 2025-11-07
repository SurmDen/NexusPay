using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Interfaces;
using Wallet.Application.Wallet.Queries;
using Wallet.Domain.Exceptions;

namespace Wallet.API.Controllers
{
    [ApiController]
    [Route("api/v1/wallet")]
    public class WalletController : ControllerBase
    {
        public WalletController(IMediator mediator, ILoggerService logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        private IMediator _mediator;
        private ILoggerService _logger;

        [HttpGet("balance/get/userid/{id:guid}")]
        public async Task<IActionResult> GetBalanceAsync(Guid id)
        {
            try
            {
                GetBalanceQuery query = new GetBalanceQuery()
                {
                    UserId = id
                };

                decimal walletBalance = await _mediator.Send(query);

                return Ok(new { balance = walletBalance, code = 200 });
            }
            catch(NotFoundException e)
            {
                return BadRequest(new {code = 400, message = e.Message});
            }
            catch (Exception e)
            {
                await _logger.LogError(e.Message, "WalletController.GetBalanceAsync", e.GetType().FullName);

                return Problem(statusCode: 500, title: "Server error", detail: "Error occured while try to get balance");
            }
        }
    }
}
