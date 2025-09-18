using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class UpdateAssignToWorkflowArgs
    {
        public Guid? AssignFromId { get; set; }
        public int? AssignFromGroup { get; set; }
        public Guid? AssignToId { get; set; }
        public int? AssignToGroup { get; set; }
        public string Type { get; set; } // department or user
        public string StepNumber { get; set; }
        public Guid InstanceId { get; set; }
    }
}
