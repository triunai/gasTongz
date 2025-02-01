using _4_GasTongz.API.Controllers;
using Commands.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Queries.Transactions;

namespace _4_GasTongz.API.Controllers.Transaction
{
    [Route("api/[controller]")]
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
    }
}
