using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    public class TransactionDetail
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? UpdatedBy { get; private set; }

        private TransactionDetail() { }

        public TransactionDetail(int productId, int quantity, decimal unitPrice, int? userId)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;

            CreatedBy = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
