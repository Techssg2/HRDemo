using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Workflow
{
    public class WorkflowData
    {
        public bool OverwriteRequestedDepartment { get; set; }
        public bool IgnoreValidation { get; set; }
        public string RequestedDepartmentField { get; set; }
        public string OnCancelled { get; set; }
        public IList<WorkflowCondition> StartWorkflowConditions { get; set; }
        public IList<WorkflowStep> Steps { get; set; }
    }
}
