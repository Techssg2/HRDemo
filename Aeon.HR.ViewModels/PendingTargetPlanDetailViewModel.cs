using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class PendingTargetPlanDetailViewModel
    { 
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public float? ERDQuality { get; set; }
        public float? PRDQuality { get; set; }
        public float? ALHQuality { get; set; }
        public float? DOFLQuality { get; set; }
        public TypeTargetPlan Type { get; set; }
        public bool IsSent { get; set; }
        public bool IsSubmitted { get; set; }
        public string JsonData { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public ResponseStatus? ResponseStatus { get; set; }
        public Guid PeriodId { get; set; }
        public Guid PendingTargetPlanId { get; set; }
        public Guid? TargetPlanDetailId { get; set; } // Khi request to change se luu lai TargetPlanDetailId
        public string Comment { get; set; }
        
        public string PeriodName { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
