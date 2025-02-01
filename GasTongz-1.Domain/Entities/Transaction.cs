using _1_GasTongz.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1_GasTongz.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; private set; }
        public int ShopId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string? ReceiptImagePath { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public int? CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? UpdatedBy { get; private set; }

        // Navigation to details
        private readonly List<TransactionDetail> _transactionDetails = new();
        public IReadOnlyCollection<TransactionDetail> TransactionDetails => _transactionDetails.AsReadOnly();

        private Transaction() { }

        public Transaction(int shopId, PaymentMethod paymentMethod, PaymentStatus paymentStatus,
                           decimal totalAmount, int? createdBy)
        {
            ShopId = shopId;
            TransactionDate = DateTime.Now;  
            PaymentMethod = paymentMethod;
            PaymentStatus = paymentStatus;
            TotalAmount = totalAmount;

            CreatedBy = createdBy;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }


        // todo: Validtation: these methods are DOGSHIT until you add validation
        public void AddDetail(int productId, int quantity, decimal unitPrice, int? userId)
        {
            // Optionally, validate that product + quantity is in stock, etc.
            var detail = new TransactionDetail(productId, quantity, unitPrice, userId);
            _transactionDetails.Add(detail);

            // Recalculate total
            TotalAmount += (quantity * unitPrice);

            // Update timestamps
            UpdatedBy = userId;
            UpdatedAt = DateTime.Now;
        }

        public void MarkPaymentSuccess(int? userId)
        {
            PaymentStatus = PaymentStatus.Success;
            UpdatedBy = userId;
            UpdatedAt = DateTime.Now;
        }

        public void SetReceiptImagePath(string path, int? userId)
        {
            ReceiptImagePath = path;
            UpdatedBy = userId;
            UpdatedAt = DateTime.Now;
        }
        public void AddLoadedDetail(TransactionDetail detail)
        {
            _transactionDetails.Add(detail);
        }
    }
}
