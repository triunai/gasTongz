using _1_GasTongz.Domain.Entities;
using _1_GasTongz.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_GasTongz.Application.Interfaces
{
    public interface ITransactionRepository
    {
    /// <summary>
    /// Persists a new Transaction (and its line items) to the database.
    /// Returns the newly created Transaction ID.
    /// </summary>
    Task<int> CreateAsync(Transaction transaction);

    /// <summary>
    /// Retrieves a Transaction by ID, possibly including line items.
    /// </summary>
    Task<Transaction?> GetByIdAsync(int transactionId);

    Task<int> CreateTransactionWithInventoryUpdate(
        int shopId,
        PaymentMethod paymentMethod,
        string lineItemsJson,
        int? userId
    );
    }



}
