using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    public class Product
    {
        // You can refine the Status property to be an enum if you prefer.
        public int Id { get; private set; }
        public string ProductName { get; private set; } = default!;
        public string? ProductType { get; private set; }
        public string? Description { get; private set; }
        public char? Status { get; private set; } // e.g., 'A' (Active), 'I' (Inactive) ?

        public DateTime CreatedAt { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? UpdatedBy { get; private set; }

        private Product() { }

        public Product(string productName, string? productType, string? description, int? createdBy)
        {
            ProductName = productName;
            ProductType = productType;
            Description = description;
            Status = 'A'; // default "Active"
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate(int? userId)
        {
            Status = 'I';
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
