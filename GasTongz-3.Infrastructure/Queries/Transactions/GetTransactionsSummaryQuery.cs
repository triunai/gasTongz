using _2_GasTongz.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _2_GasTongz.Application.DTOs.Reports.ReportDtos;
using static _2_GasTongz.Application.DTOs.ViewModels.ViewModels;

namespace Queries.Transactions
{
    public record GetTransactionsSummaryQuery : IRequest<TransactionsSummaryViewModel>;
    public class GetTransactionsSummaryQueryHandler : IRequestHandler<GetTransactionsSummaryQuery, TransactionsSummaryViewModel>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetTransactionsSummaryQueryHandler> _logger;

        public GetTransactionsSummaryQueryHandler(ITransactionRepository transactionRepository, ILogger<GetTransactionsSummaryQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<TransactionsSummaryViewModel> Handle(
            GetTransactionsSummaryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _transactionRepository.GetTransactionsSummary();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales summary.");
                return new TransactionsSummaryViewModel();
            }
        }
    }
}
