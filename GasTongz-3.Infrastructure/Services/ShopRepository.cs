using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using _3_GasTongz.Infrastructure.DbPersistance; 

namespace _3_GasTongz.Infrastructure.Repos
{
    public class ShopRepository : IShopRepository
    {
        private readonly DapperContext _context;

        public ShopRepository(DapperContext context)
        {
            _context = context;
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
                    [UpdatedBy]
                FROM [dbo].[Shops]
                WHERE [Id] = @Id;
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
                    [UpdatedBy]
                FROM [dbo].[Shops]
                ORDER BY [Name];
            ";

            return await db.QueryAsync<Shop>(sql);
        }
    }
}
