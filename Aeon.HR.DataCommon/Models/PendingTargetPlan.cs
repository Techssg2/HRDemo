using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class PendingTargetPlan : AuditableEntity
    {
        // update this model: must update: TargetPlanArg + ShiftPlan
        //[Index("IX_PendingTargetPlan", 1, IsUnique = true)]
        public Guid? DeptId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        //[Index("IX_PendingTargetPlan", 2, IsUnique = true)]
        public Guid? DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        //[Index("IX_PendingTargetPlan", 3, IsUnique = true)]
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset PeriodFromDate { get; set; }
        public DateTimeOffset PeriodToDate { get; set; }
        public bool IsSent { get; set; } // Sent to submit person       
        public virtual IEnumerable<PendingTargetPlanDetail> PendingTargetPlanDetails { get; set; }
    }
}
