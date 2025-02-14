using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_GasTongz.Application.DTOs.Reports
{
    public  class ReportDtos
    {
        public class SalesSummaryDto
        {
            public decimal TotalSales { get; set; }
            public decimal AverageTransaction { get; set; }
        }

        // DTOs/Reports/MonthlySalesDto.cs
        public class MonthlySalesDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal SalesAmount { get; set; }
        }
    }
}
