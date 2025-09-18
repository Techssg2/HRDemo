using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class WorkflowDataViewModel
    {
        public bool OverwriteRequestedDepartment { get; set; }
        public bool IgnoreValidation { get; set; }
        public string RequestedDepartmentField { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public string OnCancelled { get; set; }
        public string onRequestToChange { get; set; }
        public IList<WorkflowConditionViewModel> StartWorkflowConditions { get; set; }
        public IList<WorkflowStepViewModel> Steps { get; set; }
    }
}
