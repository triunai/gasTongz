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

namespace Queries.Transactions.Sales
{
    public record GetSalesSummaryQuery : IRequest<SalesSummaryViewModel>;
    public class GetSalesSummaryQueryHandler : IRequestHandler<GetSalesSummaryQuery, SalesSummaryViewModel>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetSalesSummaryQueryHandler> _logger;

        public GetSalesSummaryQueryHandler(ITransactionRepository transactionRepository, ILogger<GetSalesSummaryQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<SalesSummaryViewModel> Handle(
            GetSalesSummaryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _transactionRepository.GetSalesSummary();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales summary.");
                return new SalesSummaryViewModel();
            }
        }
    }
}
