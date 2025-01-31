using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using _3_GasTongz.Infrastructure.DbPersistance;

namespace _3_GasTongz.Infrastructure.Repos
{
public class InventoryRepository : IInventoryRepository
{
        private readonly DapperContext _context;

        public InventoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Inventory?> GetInventoryAsync(int shopId, int productId)
        {
            using var db = _context.CreateConnection(); // ✅ Get connection here  
            db.Open();
            var sql = @"
                SELECT
                    [Id],
                    [ShopId],
                    [ProductId],
                    [Quantity],
                    [Status],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy]
                FROM [dbo].[Inventory]
                WHERE [ShopId] = @ShopId
                  AND [ProductId] = @ProductId;
            ";

        return await db.QueryFirstOrDefaultAsync<Inventory>(sql, new { ShopId = shopId, ProductId = productId });
         }   

         public async Task UpdateAsync(Inventory inventory)
            {
            using var db = _context.CreateConnection(); // ✅ Get connection here  
            db.Open();
            var sql = @"
                    UPDATE [dbo].[Inventory]
                    SET
                        [Quantity] = @Quantity,
                        [Status] = @Status,
                        [UpdatedAt] = @UpdatedAt,
                        [UpdatedBy] = @UpdatedBy
                    WHERE [Id] = @Id;
                ";

            await db.ExecuteAsync(sql, new
            {
                Id = inventory.Id,
                Quantity = inventory.Quantity,
                Status = inventory.Status,
                UpdatedAt = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy
            });
           }
}
}
