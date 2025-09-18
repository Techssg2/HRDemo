using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TargetPlanDetailViewModel
    {
        public Guid Id { get; set; }
        public TypeTargetPlan Type { get; set; }
        public string SubmitterSAPCode { get; set; }
        public string SubmitterFullName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public float? ERDQuality { get; set; }
        public float? PRDQuality { get; set; }
        public float? ALHQuality { get; set; }
        public float? DOFLQuality { get; set; }
        public string JsonData { get; set; }
        public string DeptLine { get; set; }
        public string DivisionGroup { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public ResponseStatus? ResponseStatus { get; set; }
        public Guid TargetPlanId { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTimeOffset Created { get; set; }      
        public bool IsSent { get; set; }
        public bool IsSubmitted { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string Period { get; set; }
        public DateTimeOffset? PeriodFrom { get; set; }
        public DateTimeOffset? PeriodTo { get; set; }
        public Guid CreatedById { get; set; }

    }
}
