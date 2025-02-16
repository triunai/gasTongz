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

namespace Queries.Transactions.Sales
{
    public record GetMonthlySalesQuery : IRequest<List<MonthlySalesViewModel>>;

    public class GetMonthlySalesQueryHandler : IRequestHandler<GetMonthlySalesQuery, List<MonthlySalesViewModel>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetMonthlySalesQueryHandler> _logger;

        public GetMonthlySalesQueryHandler(ITransactionRepository transactionRepository, ILogger<GetMonthlySalesQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<List<MonthlySalesViewModel>> Handle(GetMonthlySalesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _transactionRepository.GetMonthlySales();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly sales data.");
                return new List<MonthlySalesViewModel>();
            }
        }
    }
}



