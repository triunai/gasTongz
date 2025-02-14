using _2_GasTongz.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;

namespace _3_GasTongz.Infrastructure.Queries.Inventory.StockLevel
{
    public record GetLowStockInventoryQuery : IRequest<List<LowStockInventoryViewModel>>;

    public class GetLowStockInventoryQueryHandler : IRequestHandler<GetLowStockInventoryQuery, List<LowStockInventoryViewModel>>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<GetLowStockInventoryQueryHandler> _logger;

        public GetLowStockInventoryQueryHandler(
            IInventoryRepository inventoryRepository,
            ILogger<GetLowStockInventoryQueryHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<List<LowStockInventoryViewModel>> Handle(
            GetLowStockInventoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _inventoryRepository.GetLowStockInventory();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock inventory.");
                return new List<LowStockInventoryViewModel>();
            }
        }
    }
}
