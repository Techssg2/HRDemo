using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.HR.Data.Models
{
    public class TargetPlanDetail : AuditableEntity
    {
        public TypeTargetPlan Type { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public float? ERDQuality { get; set; }
        public float? PRDQuality { get; set; }
        public float? ALHQuality { get; set; }
        public float? DOFLQuality { get; set; }
        public string JsonData { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public ResponseStatus? ResponseStatus { get; set; }    
        public Guid TargetPlanId { get; set; }
        //public bool IsSent { get; set; }
        //public bool IsSubmitted { get; set; }
        public virtual TargetPlan TargetPlan { get; set; }        
    }
}
