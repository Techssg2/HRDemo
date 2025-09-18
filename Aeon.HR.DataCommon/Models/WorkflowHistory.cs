using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Aeon.HR.Data.Models
{
    public class WorkflowHistory : Entity
    {
        public Guid InstanceId { get; set; }
        public WorkflowInstance Instance { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? AssignedToDepartmentId { get; set; }
        public Group AssignedToDepartmentType { get; set; }
        public string Approver { get; set; }
        public string ApproverFullName { get; set; }
        public string Outcome { get; set; }
        public string Comment { get; set; }
        public VoteType VoteType { get; set; }
        public int StepNumber { get; set; }
        public bool IsStepCompleted { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public virtual Department AssignedToDepartment { get; set; }
        public virtual User AssignedToUser { get; set; }
    }
}