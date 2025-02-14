﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_GasTongz.Application.DTOs.ViewModels
{
    public class ViewModels
    {
        // ViewModels/SalesSummaryViewModel.cs
        public class SalesSummaryViewModel
        {
            public decimal TotalSales { get; set; }
            public decimal AverageTransaction { get; set; }
        }

        // ViewModels/MonthlySalesViewModel.cs
        public class MonthlySalesViewModel
        {
            public string Month { get; set; }
            public decimal SalesAmount { get; set; }
        }

        // ViewModels/LowStockInventoryViewModel.cs
        public class LowStockInventoryViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }
    }
}
