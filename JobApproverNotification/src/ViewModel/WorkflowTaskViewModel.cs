using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ViewModel
{
    public class WorkflowTaskViewModel
    {
        public string Title { get; set; }
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string Status { get; set; }
        public VoteType Vote { get; set; }
        public Guid? RequestedDepartmentId { get; set; }
        public string RequestedDepartmentCode { get; set; }
        public string RequestedDepartmentName { get; set; }
        public Guid? RequestorId { get; set; }
        public string RequestorUserName { get; set; }
        public string RequestorFullName { get; set; }
        public bool IsCompleted { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public DateTimeOffset? Created { get; set; }
        public string Link { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public string Module { get; set; }
    }

    public enum VoteType
    {
        None = 0,
        Approve = 1,
        Reject = 2,
        RequestToChange = 3,
        Cancel = 4,
        OutOfPeriod = 5
    }

    public enum Group
    {
        HOD = 1, // Head of Department
        Checker = 2, // Checker 
        Member = 4, // Normal Employee,
        Assistance = 8,
        All = HOD | Checker | Member
    }
}
