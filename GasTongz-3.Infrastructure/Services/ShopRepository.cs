using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using _3_GasTongz.Infrastructure.DbPersistance;
using Microsoft.Extensions.Logging;
using System.Globalization;
namespace _3_GasTongz.Infrastructure.Repos
{
    public class ShopRepository : IShopRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ShopRepository> _logger;

        public ShopRepository(DapperContext context, ILogger <ShopRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Inserts a new shop record into the database and returns its generated ID.
        /// </summary>
        public async Task<int> CreateAsync(Shop shop)
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                INSERT INTO [dbo].[Shops]
                (
                    [Name],
                    [Location],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy]
                )
                VALUES
                (
                    @Name,
                    @Location,
                    @CreatedAt,
                    @CreatedBy,
                    @UpdatedAt,
                    @UpdatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var parameters = new
            {
                Name = shop.Name,
                Location = shop.Location,
                CreatedAt = shop.CreatedAt,
                CreatedBy = shop.CreatedBy,
                UpdatedAt = shop.UpdatedAt,
                UpdatedBy = shop.UpdatedBy
            };

            return await db.ExecuteScalarAsync<int>(sql, parameters);
        }

        /// <summary>
        /// Retrieves a shop by its ID.
        /// </summary>
        public async Task<Shop?> GetByIdAsync(int shopId)
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                SELECT 
                    [Id],
                    [Name],
                    [Location],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy],
                    [isDeleted]
                FROM [dbo].[Shops]
                WHERE [Id] = @Id AND [IsDeleted] = 0;;
            ";

            return await db.QueryFirstOrDefaultAsync<Shop>(sql, new { Id = shopId });
        }

        /// <summary>
        /// Retrieves all shops ordered by their Name.
        /// </summary>
        public async Task<IEnumerable<Shop>> GetAllAsync()
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                SELECT 
                    [Id],
                    [Name],
                    [Location],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy],
                    [isDeleted]
                FROM [dbo].[Shops]
                WHERE [IsDeleted] = 0
                ORDER BY [Name];
            ";

            return await db.QueryAsync<Shop>(sql);
        }

        public async Task<Shop?> GetByNameAsync(string name)
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                SELECT TOP 1 *
                FROM [dbo].[Shops]
                WHERE [Name] = @Name AND [IsDeleted] = 0;";
            return await db.QueryFirstOrDefaultAsync<Shop>(sql, new { Name = name });
        }

        public async Task UpdateAsync(Shop shop)
        {
            var existingShop = await GetByIdAsync(shop.Id);

            if (existingShop == null)
            {
                _logger.LogError("Shop not found in get.");
            }

            if (existingShop.IsDeleted)
            {
                _logger.LogError("Cannot update a deleted shop.");
            }
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"
                UPDATE [dbo].[Shops]
                SET 
                    [Name] = @Name,
                    [Location] = @Location,
                    [UpdatedAt] = GETDATE(),
                    [UpdatedBy] = @UpdatedBy
                WHERE [Id] = @Id";

            await db.ExecuteAsync(sql, new
            {
                Id = shop.Id,
                Name = shop.Name,
                Location = shop.Location,
                UpdatedBy = shop.UpdatedBy
            });
        }

        public async Task DeleteAsync(int shopId)
        {
            using var db = _context.CreateConnection();
            db.Open();
            var sql = @"UPDATE [dbo].[Shops]  
                        SET [isDeleted] = 1  
                        WHERE [Id] =@Id;";
            await db.ExecuteAsync(sql, new { Id = shopId });
        }


    }
}
