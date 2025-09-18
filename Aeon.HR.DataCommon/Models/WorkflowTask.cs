using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class WorkflowTask : AuditableEntity, IPermission
    {
        public string Title { get; set; }
        [Index]
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset DueDate { get; set; }
        [Index]
        public Guid? AssignedToDepartmentId { get; set; }
        public Group AssignedToDepartmentGroup { get; set; }
        [Index]
        public Guid? AssignedToId { get; set; }
        public Guid? RequestedDepartmentId { get; set; }
        public string RequestedDepartmentCode { get; set; }
        public string RequestedDepartmentName { get; set; }
        public string Status { get; set; }
        public VoteType Vote { get; set; }
        public Guid? RequestorId { get; set; }
        public string RequestorUserName { get; set; }
        public string RequestorFullName { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsTurnedOffSendNotification { get; set; }
        public bool IsAttachmentFile { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public virtual WorkflowInstance WorkflowInstance { get; set; }
    }
}
