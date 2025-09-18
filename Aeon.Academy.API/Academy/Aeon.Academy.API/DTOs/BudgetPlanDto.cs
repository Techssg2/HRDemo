namespace Aeon.Academy.Common.Entities
{
    public class BudgetPlanDto
    {
        public string BudgetPlan { get; set; }
        public string BudgetName { get; set; }
        public string BudgetCode { get; set; }
        public string CostCenterCode { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal RemainingBalance { get; set; }
        public double? TransferIn { get; set; }
        public double? TransferOut { get; set; }
        public double? Refund { get; set; }
    }
}
