using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.Queries.Transactions
{
    public record GetRecentTransactionsQuery : IRequest<List<TransactionSummaryDto>>;

    public class GetRecentTransactionsQueryHandler : IRequestHandler<GetRecentTransactionsQuery, List<TransactionSummaryDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetRecentTransactionsQueryHandler> _logger;

        public GetRecentTransactionsQueryHandler(
            ITransactionRepository transactionRepository,
            ILogger<GetRecentTransactionsQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<List<TransactionSummaryDto>> Handle(
            GetRecentTransactionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _transactionRepository.GetRecentTransactions();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent transactions");
                return new List<TransactionSummaryDto>();
            }
        }
    }
}
