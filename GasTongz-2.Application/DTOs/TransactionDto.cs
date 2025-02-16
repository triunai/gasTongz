using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Application.DTOs
{
    public class TransactionDto : TransactionSummaryDto
    {
        public List<TransactionDetailDto> TransactionDetails { get; set; }
    }

    public class TransactionSummaryDto
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } // Use string if enum isn't serializable
        public string PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ReceiptImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }

    // TransactionDetailDto.cs (for detail view)
    public class TransactionDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
