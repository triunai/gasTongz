using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    // If you want to reduce inventory after each transaction, you might write that logic in the Application layer (e.g., a service that checks Inventory and deducts quantity upon a successful sale).
    public class Inventory
    {
        public int Id { get; private set; }
        public int ShopId { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public char Status { get; private set; } // 'F' (Filled), 'E' (Empty), etc.

        public DateTime CreatedAt { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? UpdatedBy { get; private set; }

        private Inventory() { }

        public Inventory(int shopId, int productId, int quantity, char status, int? createdBy)
        {
            ShopId = shopId;
            ProductId = productId;
            Quantity = quantity;
            Status = status;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateQuantity(int newQuantity, int? userId)
        {
            if (newQuantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");

            Quantity = newQuantity;
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(char newStatus, int? userId)
        {
            Status = newStatus;
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
