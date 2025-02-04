using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
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
        private readonly IShopRepository _shopRepository;

        // Injecting IShopRepository along with the usual BaseController dependencies.
        public ShopsController(IMediator mediator, ILogger<BaseController> logger, IShopRepository shopRepository)
            : base(mediator, logger)
        {
            _shopRepository = shopRepository;
        }

        /// <summary>
        /// Creates a new shop.
        /// POST /shops
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] Shop shop)
        {
            // Minimal parameter validation; can be extended via FluentValidation.
            if (string.IsNullOrWhiteSpace(shop.Name))
            {
                return BadRequestResponse("Shop name must not be empty.");
            }

            // Create the shop using the repository.
            var newId = await _shopRepository.CreateAsync(shop);

            // Optionally, retrieve the newly created shop record.
            var createdShop = await _shopRepository.GetByIdAsync(newId);

            // Return a 201 Created response with the new shop data.
            return CreatedResponse(createdShop, $"api/shops/{newId}", "Shop created successfully");
        }

        /// <summary>
        /// Retrieves all shops.
        /// GET /shops
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllShops()
        {
            var shops = await _shopRepository.GetAllAsync();
            return SuccessResponse(shops, "Shops retrieved successfully");
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
                return BadRequestResponse("Invalid shop ID.");
            }

            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null)
            {
                return NotFoundResponse("Shop not found.");
            }

            return SuccessResponse(shop, "Shop retrieved successfully");
        }
    }
}
