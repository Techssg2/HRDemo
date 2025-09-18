using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UpdateApprovalWorkflowViewModel
    {
        public Guid? AssignFromId { get; set; }
        public string AssignFromName { get; set; }
        public string AssignFromCode { get; set; }
        public string AssignFromGroup { get; set; }
        public Guid? AssignToId { get; set; }
        public string AssignToName { get; set; }
        public string AssignToCode { get; set; }
        public string AssignToGroup { get; set; }
        public string Type { get; set; }
        public string StepNumber { get; set; }
        public Guid? InstanceId { get; set; }
        public List<AttachmentDetail> Documents { get; set; }

        public class AttachmentDetail
        {
            public Guid id { get; set; }
            public string fileDisplayName { get; set; }
        }
    }
}
