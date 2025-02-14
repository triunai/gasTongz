using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.Queries.Inventory
{
    public record GetAllInventoriesQuery() : IRequest<List<InventoryDto>>;

    public class GetAllInventoriesQueryValidator : AbstractValidator<GetAllInventoriesQuery>
    {
        // todo: token or jwt auth 
    }

    public class GetAllInventoriesQueryHandler : IRequestHandler<GetAllInventoriesQuery, List<InventoryDto>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<GetAllInventoriesQueryHandler> _logger;

        public GetAllInventoriesQueryHandler(
            IInventoryRepository inventoryRepository,
            ILogger<GetAllInventoriesQueryHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<List<InventoryDto>> Handle(
            GetAllInventoriesQuery request,
            CancellationToken cancellationToken)
        {
            var inventories = await _inventoryRepository.GetAllInventoriesAsync();
            return inventories.Select(i => new InventoryDto
            {
                Id = i.Id,
                ShopId = i.ShopId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Status = i.Status,
                IsDeleted = i.IsDeleted,
                CreatedAt = i.CreatedAt,
                CreatedBy = i.CreatedBy,
                UpdatedAt = i.UpdatedAt,
                UpdatedBy = i.UpdatedBy
            }).ToList();
        }
    }

}
