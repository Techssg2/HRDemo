using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class TrainingRequestCostCenter : BaseEntity
    {
        public Guid TrainingRequestId { get; set; }
        [StringLength(20)]
        [Required]
        public string BudgetCode { get; set; }

        [StringLength(50)]
        [Required]
        public string CostCenterCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? VATPercentage { get; set; }
        public decimal? VAT { get; set; }
        public string Currency { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        public decimal? BudgetBalanced { get; set; }
        public decimal? TotalBudget { get; set; }
        public string BudgetPlan { get; set; }
        public decimal? RemainingBalance { get; set; }
        public decimal? TransferIn { get; set; }
        public decimal? TransferOut { get; set; }
        public decimal? Refund { get; set; }

    }
}
