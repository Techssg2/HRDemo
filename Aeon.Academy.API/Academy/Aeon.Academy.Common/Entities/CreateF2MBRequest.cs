using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Aeon.Academy.Common.Entities
{
    public class CreateF2MBRequest
    {
        public string GoodService { get; set; }
        public string EmployeeCode { get; set; }
        public string PurposeReason { get; set; }
        public string MethodOfChoosingContractor { get; set; }
        public string TheProposalFor { get; set; }
        public object RequestedDepartment { get; set; }
        public DepartmentInChargeModel OrderingDepartment { get; set; }
        public DepartmentInChargeModel DepartmentInCharge { get; set; }
        public List<BudgetInformationModel> BudgetInformations { get; set; }
        public string ContractorName { get; set; }
        public string ContractorCode { get; set; }
        public decimal Amount { get; set; }
        public int Year { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AcademyURL { get; set; }
        public string ATRReferenceNumber { get; set; }
        public string Reference { get; set; }
        public string CurrencyJson { get; set; } // CR9.3 BR12
        public Guid? RequestedDepartmentId { get; set; }
    }
    public class BudgetInformationModel
    {
        public string BudgetPlan { get; set; }
        public string BudgetCode { get; set; }
        public decimal Amount { get; set; }
        public string CostCenterCode { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal CurrentBudget { get; set; }
        public decimal VATPercent { get; set; }
    }
}
