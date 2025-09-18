using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Abstracts
{
    public class WorkflowEntity : AuditableEntity, IWorkflowEntity, IAutoNumber, IPermission, IIntegrationEntity
    {
        [Index]
        [StringLength(100)]
        public string Status { get; set; }
        public DateTimeOffset SignedDate { get; set; }
        public string SignedBy { get; set; }
        [Index]
        [StringLength(100)]
        public string ReferenceNumber { get; set; }
        public string WorkflowComment { get; set; }
        [Index]
        [StringLength(100)]
        public string DeptCode { get; set; }
        [Index]
        [StringLength(255)]
        public string DeptName { get; set; }
        // use for workflow
        [NotMapped]
        public bool? LessThan10Day { get; set; }
        [NotMapped]
        public bool? LargerThanOrEqual10Day { get; set; }
    }
}