using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transaction.Application.Interfaces;
using Transaction.Application.TransactionInfo.Commands;
using Transaction.Application.TransactionInfo.Queries;
using Transaction.Domain.Exceptions;

namespace Transaction.API.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    public class TransactionController : ControllerBase
    {
        public TransactionController(IMediator mediator, ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _mediator = mediator;
        }

        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionCommand createTransaction)
        {
            try
            {
                var id = await _mediator.Send(createTransaction);

                return Ok(new {transaction_id = id, status = "200"});
            }
            catch (ArgumentException e)
            {
                await _loggerService.LogWarning(e.Message, "TransactionController.CreateTransactionAsync");

                return Problem(statusCode:400, title: "Create transaction error", detail:"invalid parameters");
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "TransactionController.CreateTransactionAsync", e.GetType().FullName);

                return Problem(statusCode: 500, title: "Internal server error");
            }
        }

        [HttpGet("get/all")]
        public async Task<IActionResult> GetAllTransactionAsync()
        {
            try
            {
                var transactions = await _mediator.Send(new GetAllTransactiosQuery());

                return Ok(transactions);
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "TransactionController.GetAllTransactionAsync", e.GetType().FullName);

                return Problem(statusCode: 500, title: "Internal server error");
            }
        }

        [HttpGet("{transactionId:guid}")]
        public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                GetTransactionQuery getTransactionQuery = new GetTransactionQuery()
                {
                    TransactionId = transactionId
                };

                var transaction = await _mediator.Send(getTransactionQuery);

                return Ok(transaction);
            }
            catch (NotFountException e)
            {
                await _loggerService.LogWarning(e.Message, "TransactionController.GetTransactionByIdAsync");

                return Problem(statusCode: 400, title: "Get transaction error", detail: e.Message);
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "TransactionController.GetTransactionByIdAsync", e.GetType().FullName);

                return Problem(statusCode: 500, title: "Internal server error");
            }
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserTransactionsByIdAsync(Guid userId)
        {
            try
            {
                GetUserTransactionsQuery getTransactionsQuery = new GetUserTransactionsQuery()
                {
                    UserId = userId
                };

                var transaction = await _mediator.Send(getTransactionsQuery);

                return Ok(transaction);
            }
            catch (NotFountException e)
            {
                await _loggerService.LogWarning(e.Message, "TransactionController.GetUserTransactionsByIdAsync");

                return Problem(statusCode: 400, title: "Get users's transactions error", detail: e.Message);
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "TransactionController.GetUserTransactionsByIdAsync", e.GetType().FullName);

                return Problem(statusCode: 500, title: "Internal server error");
            }
        }

    }
}
