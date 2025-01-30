using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace _3_GasTongz.Infrastructure.Repos
{
public class InventoryRepository : IInventoryRepository
{
    private readonly IDbConnection _db;

    public InventoryRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<Inventory?> GetInventoryAsync(int shopId, int productId)
    {
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

        return await _db.QueryFirstOrDefaultAsync<Inventory>(sql, new { ShopId = shopId, ProductId = productId });
    }

    public async Task UpdateAsync(Inventory inventory)
    {
        var sql = @"
                UPDATE [dbo].[Inventory]
                SET
                    [Quantity] = @Quantity,
                    [Status] = @Status,
                    [UpdatedAt] = @UpdatedAt,
                    [UpdatedBy] = @UpdatedBy
                WHERE [Id] = @Id;
            ";

        await _db.ExecuteAsync(sql, new
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
