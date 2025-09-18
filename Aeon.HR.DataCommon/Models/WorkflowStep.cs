using Aeon.HR.Infrastructure.Entities;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class WorkflowStep
    {
        public string StepName { get; set; }
        public bool OverwriteRequestedDepartment { get; set; }
        public string RequestedDepartmentField { get; set; }
        public bool IsStatusFollowStepName { get; set; }
        public IList<WorkflowCondition> SkipStepConditions { get; set; }
        public IList<RestrictedProperty> RestrictedProperties { get; set; }
        public IList<StepCondition> StepConditions { get; set; }
        public int StepNumber { get; set; }
        public string OnSuccess { get; set; }
        public string OnFailure { get; set; }
        public string SuccessVote { get; set; }
        public string FailureVote { get; set; }
        public int DueDateNumber { get; set; }
        public ParticipantType ParticipantType { get; set; }
        public bool TraversingFromRoot { get; set; }
        public bool IsHRHQ {get;set;}
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public bool IgnoreIfNoParticipant { get; set; }
        public bool PreventAutoPopulate { get; set; }
        public string DataField { get; set; }
        public string MaxJobGrade { get; set; }
        public string JobGrade { get; set; }
        public bool ReverseJobGrade { get; set; }
        public bool IncludeCurrentNode { get; set; }
        public Group NextDepartmentType { get; set; }
        public Guid? UserId { get; set; }
        public string UserFullName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Group DepartmentType { get; set; }
        public bool AllowRequestToChange { get; set; }
        public bool IsCustomEvent { get; set; }
        public string CustomEventKey { get; set; }
        public int ReturnToStepNumber { get; set; }
        public Right RequestorPerm { get; set; }
        public Right ApproverPerm { get; set; }
        public bool IsTurnedOffSendNotification { get; set; }
        public bool IsAttachmentFile { get; set; }
        public bool IsCustomRequestToChange { get; set; }
        public bool IsStepWithConditions { get; set; }
    }
}