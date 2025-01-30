using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using Dapper;
using System.Data;

namespace _3_GasTongz.Infrastructure.Repos
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _db;

        public TransactionRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Transaction transaction)
        {
            // 1) Insert the transaction header
            //    and capture the newly generated [Id]
            var sqlInsertTransaction = @"
                INSERT INTO [dbo].[Transactions]
                (
                    [ShopId],
                    [TransactionDate],
                    [PaymentMethod],
                    [PaymentStatus],
                    [TotalAmount],
                    [ReceiptImagePath],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy]
                )
                VALUES
                (
                    @ShopId,
                    @TransactionDate,
                    @PaymentMethod,
                    @PaymentStatus,
                    @TotalAmount,
                    @ReceiptImagePath,
                    @CreatedAt,
                    @CreatedBy,
                    @UpdatedAt,
                    @UpdatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var newTransactionId = await _db.ExecuteScalarAsync<int>(sqlInsertTransaction, new
            {
                ShopId = transaction.ShopId,
                TransactionDate = transaction.TransactionDate,
                PaymentMethod = transaction.PaymentMethod.ToString(), // if stored as string or 'QR'
                PaymentStatus = transaction.PaymentStatus.ToString(),
                TotalAmount = transaction.TotalAmount,
                ReceiptImagePath = transaction.ReceiptImagePath,
                CreatedAt = transaction.CreatedAt,
                CreatedBy = transaction.CreatedBy,
                UpdatedAt = transaction.UpdatedAt,
                UpdatedBy = transaction.UpdatedBy
            });

            // 2) Insert line items
            var sqlInsertDetail = @"
                INSERT INTO [dbo].[TransactionDetails]
                (
                    [TransactionId],
                    [ProductId],
                    [Quantity],
                    [UnitPrice],
                    [CreatedAt],
                    [CreatedBy],
                    [UpdatedAt],
                    [UpdatedBy]
                )
                VALUES
                (
                    @TransactionId,
                    @ProductId,
                    @Quantity,
                    @UnitPrice,
                    @CreatedAt,
                    @CreatedBy,
                    @UpdatedAt,
                    @UpdatedBy
                );
            ";

            foreach (var detail in transaction.TransactionDetails)
            {
                await _db.ExecuteAsync(sqlInsertDetail, new
                {
                    TransactionId = newTransactionId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    CreatedAt = detail.CreatedAt,
                    CreatedBy = detail.CreatedBy,
                    UpdatedAt = detail.UpdatedAt,
                    UpdatedBy = detail.UpdatedBy
                });
            }

            return newTransactionId;
        }

        public async Task<Transaction?> GetByIdAsync(int transactionId)
        {
            // 1) Fetch Transaction header
            var sqlTransaction = @"
                SELECT
                    t.[Id],
                    t.[ShopId],
                    t.[TransactionDate],
                    t.[PaymentMethod],
                    t.[PaymentStatus],
                    t.[TotalAmount],
                    t.[ReceiptImagePath],
                    t.[CreatedAt],
                    t.[CreatedBy],
                    t.[UpdatedAt],
                    t.[UpdatedBy]
                FROM [dbo].[Transactions] t
                WHERE t.[Id] = @Id;
            ";

            var transaction = await _db.QueryFirstOrDefaultAsync<Transaction>(sqlTransaction, new { Id = transactionId });
            if (transaction == null)
                return null;

            // 2) Fetch line items
            var sqlDetails = @"
                SELECT
                    d.[Id],
                    d.[TransactionId],  -- We'll need this if the domain constructor requires it
                    d.[ProductId],
                    d.[Quantity],
                    d.[UnitPrice],
                    d.[CreatedAt],
                    d.[CreatedBy],
                    d.[UpdatedAt],
                    d.[UpdatedBy]
                FROM [dbo].[TransactionDetails] d
                WHERE d.[TransactionId] = @TransactionId;
            ";

            var details = await _db.QueryAsync<TransactionDetail>(sqlDetails, new { TransactionId = transactionId });

            // 3) Attach details to the transaction
            //    If your domain uses a private List<> you can do reflection or a specialized method
            //    For simplicity, let's assume we can do something like a method AddLoadedDetails()

            foreach (var detail in details)
            {
                // This approach depends on how your domain constructor handles these
                // You might re-initialize a new domain object or do direct assignment
                transaction.AddLoadedDetail(detail);
            }

            return transaction;
        }
    }
}
