using _1_GasTongz.Domain.Entities;
using System.Threading.Tasks;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;
//todo: get usings from git changes
namespace _2_GasTongz.Application.Interfaces
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Creates a new Inventory record (initial stock creation).
        /// </summary>
        /// <param name="inventory">The inventory entity to insert.</param>
        /// <returns>The newly created Inventory record's ID.</returns>
        Task<int> CreateAsync(Inventory inventory);

        /// <summary>
        /// Retrieves an Inventory record by Shop and Product IDs.
        /// </summary>
        /// <param name="shopId">The ID of the shop.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The Inventory entity if found; otherwise, null.</returns>
        Task<Inventory?> GetInventoryAsync(int shopId, int productId);

        /// <summary>
        /// Increases the stock for a given shop and product by the specified amount.
        /// If no inventory record exists, a new one is created.
        /// </summary>
        /// <param name="shopId">The ID of the shop.</param>
        /// <param name="productId">The ID of the product (likely fixed to 1 for gas cylinders).</param>
        /// <param name="amount">The quantity to add.</param>
        /// <param name="updatedBy">The ID of the admin performing the operation.</param>
        Task IncreaseStockAsync(int shopId, int productId, int amount, int? updatedBy);

        /// <summary>
        /// Reduces the stock for a given shop and product by the specified amount.
        /// If the resulting quantity is zero, the status should be updated to indicate empty.
        /// Throws an error or returns a failure result if there is insufficient stock.
        /// </summary>
        /// <param name="shopId">The ID of the shop.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="amount">The quantity to deduct.</param>
        /// <param name="updatedBy">The ID of the admin performing the operation.</param>
        Task ReduceStockAsync(int shopId, int productId, int amount, int? updatedBy);

        /// <summary>
        /// Updates an existing Inventory record in the database.
        /// This method is used for general updates outside of specific stock adjustments.
        /// </summary>
        /// <param name="inventory">The updated inventory entity.</param>
        Task UpdateAsync(Inventory inventory);


        Task SoftDeleteByShopIdAsync(int shopId);

        Task SoftDeleteInventoryAsync(int inventoryId);

        Task<IEnumerable<Inventory>> GetAllInventoriesAsync();

        Task<Inventory?> GetInventoryByIdAsync(int inventoryId);
        Task<Inventory?> GetInventoryIncludingDeletedAsync(int shopId, int productId);
        Task<List<LowStockInventoryViewModel>> GetLowStockInventory();
    }
}
