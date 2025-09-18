using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanArg
    {
        public Guid Id { get; set; }
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
        public List<TargetPlanArgDetail> List { get; set; }
    }
    public class TargetPlanArgDetail
    {
        public Guid PenDetailId { get; set; }
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
        public Guid? PendingTargetPlanId { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? ModifiedById { get; set; }
        public Guid? PeriodId { get; set; }
        public Guid? TargetPlanId { get; set; }
        public bool IsSent { get; set; }
        public bool IsSubmitted { get; set; }
        public string ReferenceNumber { get; set; }
        //public Guid? CreatedById { get; set; }
        public float? CFRemain { get; set; }
    }

    public class CreateTargetPlan_APIArgs
    {
        public Guid RequestorId { get; set; }
        public Guid DeptLineId { get; set; }
        public Guid DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public List<TargetPlanArgDetailCreateTargetPlan_APIArgs> List { get; set; }
    }


    public class TargetPlanArgDetailCreateTargetPlan_APIArgs
    {
        public TypeTargetPlan Type { get; set; }
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public string JsonData { get; set; }
        public List<TargetPlanFromImportDetailItemDTO> Targets { get; set; }
    }

}
