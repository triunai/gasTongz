using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;

namespace Queries.Transactions
{
    public record GetMonthlyTransactionsQuery : IRequest<List<MonthlyTransactionsViewModel>>;

    public class GetMonthlyTransactionsQueryHandler : IRequestHandler<GetMonthlyTransactionsQuery, List<MonthlyTransactionsViewModel>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetMonthlyTransactionsQueryHandler> _logger;

        public GetMonthlyTransactionsQueryHandler(ITransactionRepository transactionRepository, ILogger<GetMonthlyTransactionsQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<List<MonthlyTransactionsViewModel>> Handle(GetMonthlyTransactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _transactionRepository.GetMonthlyTransactions();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly sales data.");
                return new List<MonthlyTransactionsViewModel>();
            }
        }
    }
}



