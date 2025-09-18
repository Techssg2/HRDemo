using JobSendMailApproverNotificationsV2.src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Model
{
    public class Edoc1TaskModelV2
    {
        public Guid Id { get; set; }
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
        public DateTimeOffset? Modified { get; set; }
        public string Link { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public string Module { get; set; }
        public Guid? CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
        public bool? IsParallelApprove { get; set; }
        public int? ParallelStep { get; set; }
        public bool IsSignOff { get; set; }
        public bool IsMultibudget { get; set; }
        public bool IsManual { get; set; }
        public bool? IsConfidentialContract { get; set; }

        public int? DocumentSetPurpose { get; set; }
    }
}
