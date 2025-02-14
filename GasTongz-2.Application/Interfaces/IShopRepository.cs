using _1_GasTongz.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;

namespace _2_GasTongz.Application.Interfaces
{
    public interface IShopRepository
    {
        /// <summary>
        /// Inserts a new shop record into the database.
        /// </summary>
        /// <param name="shop">The shop entity to create.</param>
        /// <returns>The newly created shop's ID.</returns>
        Task<int> CreateAsync(Shop shop);

        /// <summary>
        /// Retrieves the shop record with the specified ID.
        /// </summary>
        /// <param name="shopId">The ID of the shop.</param>
        /// <returns>The Shop entity if found; otherwise, null.</returns>
        Task<Shop?> GetByIdAsync(int shopId);

        /// <summary>
        /// Retrieves all shop records from the database.
        /// </summary>
        /// <returns>A collection of Shop entities.</returns>
        Task<IEnumerable<Shop>> GetAllAsync();

    
        // literally just used to check for duplicates in CreateTransactionCommand
        Task<Shop?> GetByNameAsync(string name);
        Task UpdateAsync(Shop shop);
        Task DeleteAsync(int shopId);



    }
}
