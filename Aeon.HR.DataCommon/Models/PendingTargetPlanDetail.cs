using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class PendingTargetPlanDetail : AuditableEntity
    {
        [Index("IX_PendingTargetPlan", 2, IsUnique = true)]
        public TypeTargetPlan Type { get; set; }
        [Index("IX_PendingTargetPlan", 1, IsUnique = true)]
        [StringLength(20)]
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public float? ERDQuality { get; set; }
        public float? PRDQuality { get; set; }
        public float? ALHQuality { get; set; }
        public float? DOFLQuality { get; set; }
        public string JsonData { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public ResponseStatus? ResponseStatus { get; set; }
        public Guid PendingTargetPlanId { get; set; }
        public bool IsSent { get; set; }
        public bool IsSubmitted { get; set; }
        [Index("IX_PendingTargetPlan", 3, IsUnique = true)]
        public Guid PeriodId { get; set; }
        public Guid? TargetPlanDetailId { get; set; } // Khi request to change se luu lai TargetPlanDetailId
        public string Comment { get; set; }
        public virtual PendingTargetPlan PendingTargetPlan { get; set; }
    }
}
