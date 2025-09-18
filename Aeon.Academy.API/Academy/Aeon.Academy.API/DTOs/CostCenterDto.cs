using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Academy.API.DTOs
{
    public class CostCenterDto
    {
        public Guid Id { get; set; }
        public string BudgetPlan { get; set; }
        public string BudgetCode { get; set; }
        public string CostCenterCode { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? Vat { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public decimal? RemainingBalance { get; set; }
        public decimal? BudgetBalance { get; set; }
        public decimal? TotalBudget { get; set; }
        public decimal? TransferIn { get; set; }
        public decimal? TransferOut { get; set; }
        public decimal? Refund { get; set; }
    }
}