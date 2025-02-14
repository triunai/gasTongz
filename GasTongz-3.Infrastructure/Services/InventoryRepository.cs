using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using _3_GasTongz.Infrastructure.DbPersistance;
using Microsoft.Extensions.Logging;

namespace _3_GasTongz.Infrastructure.Repos
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<InventoryRepository> _logger;
        private readonly IShopRepository _shopRepository; // Optional dependency for shop verification

        public InventoryRepository(DapperContext context, ILogger<InventoryRepository> logger, IShopRepository shopRepository)
        {
            _context = context;
            _logger = logger;
            _shopRepository = shopRepository;
        }

        public async Task<Inventory?> GetInventoryAsync(int shopId, int productId)
        {
            using var db = _context.CreateConnection();
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
            using var db = _context.CreateConnection();
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

        public async Task<int> CreateAsync(Inventory inventory)
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                INSERT INTO [dbo].[Inventory]
                (
                    [ShopId],
                    [ProductId],
                    [Quantity],
                    [Status],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy]
                )
                VALUES
                (
                    @ShopId,
                    @ProductId,
                    @Quantity,
                    @Status,
                    @CreatedAt,
                    @CreatedBy,
                    @UpdatedAt,
                    @UpdatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            return await db.ExecuteScalarAsync<int>(sql, new
            {
                ShopId = inventory.ShopId,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                Status = inventory.Status,
                CreatedAt = inventory.CreatedAt,
                CreatedBy = inventory.CreatedBy,
                UpdatedAt = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy
            });
        }

        public async Task IncreaseStockAsync(int shopId, int productId, int amount, int? updatedBy)
        {
            // Validate shop existence first
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                _logger.LogWarning("Shop with ID {ShopId} not found. Cannot increase inventory.", shopId);
                return;
            }

            var inventory = await GetInventoryAsync(shopId, productId);
            if (inventory != null)
            {
                _logger.LogInformation("Increasing stock for ShopId: {ShopId}, ProductId: {ProductId} by {Amount}.", shopId, productId, amount);
                inventory.UpdateQuantity(inventory.Quantity + amount, updatedBy);
                inventory.ChangeStatus('F', updatedBy);
                await UpdateAsync(inventory);
            }
            else
            {
                _logger.LogWarning("Inventory record for ShopId: {ShopId}, ProductId: {ProductId} not found. Creating new inventory record.", shopId, productId);
                // TODO: Validate further if needed.
                var newInventory = new Inventory(shopId, productId, amount, 'F', updatedBy);
                await CreateAsync(newInventory);
            }
        }

        public async Task ReduceStockAsync(int shopId, int productId, int amount, int? updatedBy)
        {
            var inventory = await GetInventoryAsync(shopId, productId);
            if (inventory == null)
            {
                _logger.LogWarning("Inventory record not found for ShopId: {ShopId}, ProductId: {ProductId}.", shopId, productId);
                return;
            }

            if (inventory.Quantity < amount)
            {
                _logger.LogWarning("Insufficient stock for ShopId: {ShopId}, ProductId: {ProductId}. Current: {Quantity}, Requested Reduction: {Amount}.",
                    shopId, productId, inventory.Quantity, amount);
                return;
            }

            int newQuantity = inventory.Quantity - amount;
            inventory.UpdateQuantity(newQuantity, updatedBy);

            // If the new quantity is zero, mark as Empty ('E'); otherwise, remain Filled ('F')
            inventory.ChangeStatus(newQuantity == 0 ? 'E' : 'F', updatedBy);
            await UpdateAsync(inventory);
        }
    }
}


