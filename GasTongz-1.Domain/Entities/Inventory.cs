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
        public bool IsDeleted { get; private set; }

        private Inventory() { }

        // ctor for fresh stock
        public Inventory(int shopId, int productId, int quantity, char status, int? createdBy)
        {
            ShopId = shopId;
            ProductId = productId;
            Quantity = quantity;
            Status = status;
            CreatedBy = createdBy;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void UpdateQuantity(int newQuantity, int? userId)
        {
            if (newQuantity < 0)
            {
                Console.WriteLine("Update quantity error, new quantity is lower than 0");
                return;
            }

            Quantity = newQuantity;
            UpdatedBy = userId;
            UpdatedAt = DateTime.Now;
        }

        public void ChangeStatus(char newStatus, int? userId)
        {
            Status = newStatus;
            UpdatedBy = userId;
            UpdatedAt = DateTime.Now;
        }
    }
}
