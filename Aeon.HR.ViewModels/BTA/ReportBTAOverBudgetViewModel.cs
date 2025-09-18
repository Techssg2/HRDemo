using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportBTAOverBudgetViewModel : IReportBTA
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string BTAStatus { get; set; }
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string ReferenceNumber { get; set; }

        public DateTime OverBudgetDate { get; set; }
        public string BTA_ReferenceNumber { get; set; }
        public string DeptName { get; set; }
        public string DeptDivisionName { get; set; }
        public Guid? BTAId { get; set; }
        public decimal OldBudget { get; set; }
        public decimal NewBudget { get; set; }
        public string DepartureName { get; set; }
        public string ArrivalName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}
