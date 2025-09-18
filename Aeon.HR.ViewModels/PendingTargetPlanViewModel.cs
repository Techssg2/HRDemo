using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class PendingTargetPlanViewModel
    {
        public Guid Id { get; set; }
        public Guid? DeptId { get; set; }
        public Guid? DivisionId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset PeriodFromDate { get; set; }
        public DateTimeOffset PeriodToDate { get; set; }      
        public bool IsSent { get; set; }
        public string JsonData { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
