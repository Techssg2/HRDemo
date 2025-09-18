using System;

namespace Aeon.HR.ViewModels
{
    public class WorkflowTemplateViewModel
    {
        public string WorkflowName { get; set; }
        public string ItemType { get; set; }
        public int Order { get; set; }
        public bool IsActivated { get; set; }
        public bool AllowMultipleFlow { get; set; }
        public string StartWorkflowButton { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public WorkflowDataViewModel WorkflowData
        {
            get;
            set;
        }

        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public bool? HasTrackingLog { get; set; }
    }
}