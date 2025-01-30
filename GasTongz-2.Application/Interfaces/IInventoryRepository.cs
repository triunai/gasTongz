using _1_GasTongz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_GasTongz.Application.Interfaces
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieves an Inventory record by Shop + Product IDs.
        /// </summary>
        Task<Inventory?> GetInventoryAsync(int shopId, int productId);

        /// <summary>
        /// Updates an existing Inventory record in the database.
        /// </summary>
        Task UpdateAsync(Inventory inventory);
    }
}
