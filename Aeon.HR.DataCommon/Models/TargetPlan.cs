using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class TargetPlan : WorkflowEntity, ICBEntity
    {
        // update this model: must update: TargetPlanArg + ShiftPlan
        public Guid? DeptId { get; set; }
        public Guid? DivisionId { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public Guid PeriodId { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset PeriodFromDate { get; set; }
        public DateTimeOffset PeriodToDate { get; set; }
        public bool IsSent { get; set; }
        public string UserSAPCode { get; set; } // Submit Person SAP
        public string UserFullName { get; set; } // Submit Person SAP
        public bool IsStore { get; set; }
        //public Guid? PendingTargetPlanId { get; set; }
        public virtual PendingTargetPlan PendingTargetPlan { get; set; }
        public virtual ICollection<TargetPlanDetail> TargetPlanDetails { get; set; }
    }
}
