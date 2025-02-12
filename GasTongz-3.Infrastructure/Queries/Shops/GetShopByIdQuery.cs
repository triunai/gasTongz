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
    public record GetShopByIdQuery(int Id) : IRequest<ShopDto?>;


    public class GetShopByIdQueryHandler : IRequestHandler<GetShopByIdQuery, ShopDto?>
    {
        private readonly IShopRepository _shopRepository;

        public GetShopByIdQueryHandler(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        public async Task<ShopDto?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
        {
            var shop = await _shopRepository.GetByIdAsync(request.Id);
            return shop != null ? new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Location = shop.Location,
                CreatedAt = shop.CreatedAt,
                CreatedBy = shop.CreatedBy
            } : null;
        }
    }
}
