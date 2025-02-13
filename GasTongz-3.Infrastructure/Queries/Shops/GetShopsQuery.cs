using _2_GasTongz.Application.DTOs;
using _2_GasTongz.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.Queries.Shops
{
    public record GetShopsQuery() : IRequest<List<ShopDto>>;

    public class GetShopsQueryHandler : IRequestHandler<GetShopsQuery, List<ShopDto>>
    {
        private readonly IShopRepository _shopRepository;
        private readonly ILogger<GetShopsQueryHandler> _logger;

        public GetShopsQueryHandler(IShopRepository shopRepository, ILogger<GetShopsQueryHandler> logger)
        {
            _shopRepository = shopRepository;
            _logger = logger;
        }

        public async Task<List<ShopDto>> Handle(GetShopsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var shops = await _shopRepository.GetAllAsync();
                return shops.Select(s => new ShopDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    CreatedAt = s.CreatedAt,
                    CreatedBy = s.CreatedBy
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shops.");
                return new List<ShopDto>(); // Return empty list on error
            }
        }
    }
}
