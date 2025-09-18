using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class WorkflowHistoryViewModel
    {
        public Guid InstanceId { get; set; }
        public Guid? ApproverId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string AssignedToUserCode { get; set; }
        public string AssignedToUserFullName { get; set; }
        public Guid? AssignedToDepartmentId { get; set; }
        public string AssignedToDepartmentCode { get; set; }
        public Group AssignedToDepartmentType { get; set; }
        public string AssignedToDepartmentName { get; set; }
        public string Approver { get; set; }
        public string ApproverFullName { get; set; }
        public string Outcome { get; set; }
        public string Comment { get; set; }
        public VoteType VoteType { get; set; }
        public int StepNumber { get; set; }
        public bool IsStepCompleted { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool? CurrentRound { get; set; }
    }
}
