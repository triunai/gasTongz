using _2_GasTongz.Application.DTOs;
using _2_GasTongz.Application.Interfaces;
using MediatR;
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

        public GetShopsQueryHandler(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        public async Task<List<ShopDto>> Handle(GetShopsQuery request, CancellationToken cancellationToken)
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
    }
}
