using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Commands.Shops;
using _3_GasTongz.Infrastructure.Queries.Shops;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace _4_GasTongz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : BaseController
    {
        public ShopsController(IMediator mediator, ILogger<BaseController> logger)
                   : base(mediator, logger) { }
        /// <summary>
        /// Creates a new shop.
        /// POST /shops
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopCommand command)
        {
            return await SendRequest(command, "Shop created successfully");
        }

        /// <summary>
        /// Retrieves all shops.
        /// GET /shops
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllShops()
        {
            return await SendRequest(new GetShopsQuery(), "Shops retrieved successfully");
        }

        /// <summary>
        /// Retrieves a single shop by its ID.
        /// GET /shops/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid shop ID.");
            }

            return await SendRequest(new GetShopByIdQuery(id), "Shop retrieved successfully");
        }
    }
}