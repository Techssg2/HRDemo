using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ActualTargetPlanArg
    {
        //public Guid TargetPlanId { get; set; }
        public Guid PeriodId { get; set; }
        public string ListSAPCode { get; set; }
        public List<Guid> NotInShiftExchange { get; set; }
    }
    public class SubmitDataArg
    {
        public Guid? DeptId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public Guid? DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset PeriodFromDate { get; set; }
        public DateTimeOffset PeriodToDate { get; set; }
        public string UserSAPCode { get; set; } // Submit Person SAP
        public string UserFullName { get; set; } // Submit Person SAP
        public string ListSAPCode { get; set; }
        public Guid? PendingTargetPlanId { get; set; }
    }
}
