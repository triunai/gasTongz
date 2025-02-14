
using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Queries.Inventory
{

    // Query request: retrieve inventory by shop and product IDs
    public record GetInventoryByShopAndProductQuery(int ShopId, int ProductId) : IRequest<InventoryDto?>;

    // Validator for the query
    public class GetInventoryByShopAndProductQueryValidator : AbstractValidator<GetInventoryByShopAndProductQuery>
    {
        public GetInventoryByShopAndProductQueryValidator()
        {
            RuleFor(q => q.ShopId)
                .GreaterThan(0)
                .WithMessage("ShopId must be greater than zero.");
            RuleFor(q => q.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero.");
        }
    }

    // Handler for the query
    public class GetInventoryByShopAndProductQueryHandler : IRequestHandler<GetInventoryByShopAndProductQuery, InventoryDto?>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<GetInventoryByShopAndProductQueryHandler> _logger;

        public GetInventoryByShopAndProductQueryHandler(
            IInventoryRepository inventoryRepository,
            ILogger<GetInventoryByShopAndProductQueryHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<InventoryDto?> Handle(GetInventoryByShopAndProductQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve inventory using shop and product IDs
                var inventory = await _inventoryRepository.GetInventoryAsync(query.ShopId, query.ProductId);
                return inventory != null ? new InventoryDto
                {
                    Id = inventory.Id,
                    ShopId = inventory.ShopId,
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                    Status = inventory.Status,
                    IsDeleted = inventory.IsDeleted,
                    CreatedAt = inventory.CreatedAt,
                    CreatedBy = inventory.CreatedBy,
                    UpdatedAt = inventory.UpdatedAt,
                    UpdatedBy = inventory.UpdatedBy
                } : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory for ShopId: {ShopId}, ProductId: {ProductId}", query.ShopId, query.ProductId);
                throw; // Let the global exception handler deal with this error.
            }
        }
    }
}