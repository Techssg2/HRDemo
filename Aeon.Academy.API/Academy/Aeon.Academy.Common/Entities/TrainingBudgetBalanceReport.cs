using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class TrainingBudgetBalanceReport
    {
        public string RequestFor { get; set; }
        public Guid RequestForDeptId { get; set; }
        public List<RequestedDepartment> RequestedDepartments { get; set; }
        public string CourseName { get; set; }
        public string BudgetGroup { get; set; }
        public decimal PlannedBudget { get; set; }
        public decimal ActualUsedBudget { get; set; }
        public string SupplierName { get; set; }
        public DateTimeOffset RequestedDate { get; set; }
    }
    public class RequestedDepartment
    {
        public string DepartmentName { get; set; }
        public Guid DepartmentId { get; set; }
        public List<TrainingBudgetParticipant> Participants { get; set; }
        public decimal ActualUsedBudgetByDepartment { get; set; }
    }
    public class TrainingBudgetParticipant
    {
        public string Jobgrade { get; set; }
        public int NoParticipant { get; set; }
    }
}
