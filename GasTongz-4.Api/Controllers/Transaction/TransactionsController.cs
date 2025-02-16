using _3_GasTongz.Infrastructure.Queries.Transactions;
using _4_GasTongz.API.Controllers;
using Commands.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Queries.Transactions;

namespace _4_GasTongz.API.Controllers.Transaction
{
    
    [ApiController]
    public class TransactionsController : BaseController
    {
        public TransactionsController(IMediator mediator, ILogger<BaseController> logger)
            : base(mediator, logger)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
        {
            // Use the SendRequest helper method to send the command via MediatR.
            return await SendRequest(command, "Transaction created successfully");
        }


        [HttpGet("{transactionId:int}")]
        public async Task<IActionResult> GetTransactionById([FromRoute] int transactionId)
        {
            if (transactionId <= 0)
            {
                return BadRequest(new { Success = false, Message = "TransactionId must be greater than zero." });
            }

            // Create the query object.
            var query = new GetTransactionByIdQuery(transactionId);
            // Send the query via MediatR using the BaseController's SendRequest helper.
            return await SendRequest(query, "Transaction retrieved successfully");
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetTransactionsSummary()
        {
            return await SendRequest(new GetTransactionsSummaryQuery(), "Sales summary retrieved");
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyTransactions()
        {
            return await SendRequest(new GetMonthlyTransactionsQuery(), "Monthly sales retrieved");
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentTransactions()
        {
            return await SendRequest(new GetRecentTransactionsQuery(), "Recent transactions retrieved");
        }

    }
}
