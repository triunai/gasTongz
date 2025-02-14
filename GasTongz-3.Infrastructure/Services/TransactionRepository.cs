using _1_GasTongz.Domain.Entities;
using _1_GasTongz.Domain.Enums;
using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.DbPersistance;
using Dapper;
using System.Data;
using static _2_GasTongz.Application.DTOs.Reports.ReportDtos;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;
using System.Globalization;

namespace _3_GasTongz.Infrastructure.Repos
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DapperContext _context;

        // Inject DapperContext instead of IDbConnection  
        public TransactionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Transaction transaction)
        {
            using var db = _context.CreateConnection(); // ✅ Create connection here  
            db.Open();
            // 1) Insert the transaction header
            //    and capture the newly generated [Id]
            // todo: convert to sp
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

            var newTransactionId = await db.ExecuteScalarAsync<int>(sqlInsertTransaction, new
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
                await db.ExecuteAsync(sqlInsertDetail, new
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
            using var db = _context.CreateConnection(); // ✅ Create connection here  
            db.Open();

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

            var transaction = await db.QueryFirstOrDefaultAsync<Transaction>(sqlTransaction, new { Id = transactionId });
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

            var details = await db.QueryAsync<TransactionDetail>(sqlDetails, new { TransactionId = transactionId });

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

        public async Task<int> CreateTransactionWithInventoryUpdate(
            int shopId,
            PaymentMethod paymentMethod,
            string lineItemsJson,
            int? userId
        )
                {
                    using var db = _context.CreateConnection();
                    db.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@ShopId", shopId);
                    parameters.Add("@PaymentMethod", paymentMethod.ToString());
                    parameters.Add("@LineItems", lineItemsJson);
                    parameters.Add("@UserId", userId);

                    // Use Dapper's stored procedure execution
                    return await db.QuerySingleAsync<int>(
                        "CreateTransactionWithInventoryUpdate",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

        public async Task<SalesSummaryViewModel> GetSalesSummary()
        {
            using var db = _context.CreateConnection();
            db.Open();

            var sql = @"
            SELECT 
                SUM(TotalAmount) AS TotalSales, 
                AVG(TotalAmount) AS AverageTransaction 
            FROM Transactions";

            var result = await db.QueryFirstAsync<SalesSummaryDto>(sql);

            return new SalesSummaryViewModel
            {
                TotalSales = result.TotalSales,
                AverageTransaction = result.AverageTransaction
            };
        }

        public async Task<List<MonthlySalesViewModel>> GetMonthlySales()
        {
            using var db = _context.CreateConnection();
            db.Open();

            var sql = @"
                SELECT 
                    YEAR([Date]) AS [Year], 
                    MONTH([Date]) AS [Month], 
                    SUM([TotalAmount]) AS [SalesAmount]
                FROM [Transactions]
                GROUP BY YEAR([Date]), MONTH([Date])
                ORDER BY [Year], [Month]";

            var monthlySales = await db.QueryAsync<MonthlySalesDto>(sql);

            return monthlySales.Select(dto =>
            {
                var monthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(dto.Month);
                return new MonthlySalesViewModel
                {
                    Month = $"{monthName} {dto.Year}",
                    SalesAmount = dto.SalesAmount
                };
            }).ToList();
        }


        private string GetMonthName(string month)
        {
            int monthNumber = int.Parse(month.Split('-')[1]);
            return DateTimeFormatInfo.CurrentInfo.GetMonthName(monthNumber);
        }
    }


}
