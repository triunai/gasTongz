using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure;
using _3_GasTongz.Infrastructure.Commands.Inventory;
using _3_GasTongz.Infrastructure.Queries.Inventory;
using Commands.Inventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Queries.Inventory;

namespace _4_GasTongz.API.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : BaseController
    {

        public InventoryController(IMediator mediator, ILogger<BaseController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Retrieves inventory details for a specific shop and product.
        /// </summary>
        /// <param name="shopId">ID of the shop.</param>
        /// <param name="productId">ID of the product.</param>
        /// <returns>Inventory details if found; otherwise, NotFound.</returns>
        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] int shopId, [FromQuery] int productId)
        {
            if (shopId <= 0 || productId <= 0)
            {
                return BadRequest(new { Success = false, Message = "ShopId and ProductId must be greater than zero." });
            }

            // Create the query object
            var query = new GetInventoryByShopAndProductQuery(shopId, productId);

            // Use the helper method from BaseController to send the query via MediatR
            return await SendRequest(query, "Inventory record retrieved successfully");
        }

        /// <summary>
        /// Updates an existing inventory record.
        /// </summary>
        /// <param name="command">UpdateInventoryCommand containing shop ID, product ID, new quantity, new status, and updated by info.</param>
        /// <returns>A success response if the update is successful.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateInventory([FromBody] UpdateInventoryCommand command)
        {
            // Basic parameter validation; deeper validation should be performed via FluentValidation
            if (command.ShopId <= 0 || command.ProductId <= 0)
            {
                return BadRequest(new { Success = false, Message = "ShopId and ProductId must be greater than zero." });
            }

            // Send the update command via MediatR using the helper method from BaseController.
            return await SendRequest(command, "Inventory updated successfully");
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryCommand command)
        {
            return await SendRequest(command, "Inventory created successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            return await SendRequest(new DeleteInventoryCommand(id), "Inventory deleted successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            return await SendRequest(
                new GetAllInventoriesQuery(),
                "Inventories retrieved successfully");
        }
    }
}
