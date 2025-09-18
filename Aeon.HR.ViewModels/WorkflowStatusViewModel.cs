using Aeon.HR.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class WorkflowStatusViewModel
    {
        public string WorkflowName { get; set; }
        public bool AllowToVote { get; set; }
        public bool IgnoreValidation { get; set; }
        public bool AllowToStartWorflow { get; set; }
        public string ApproveFieldText { get; set; }
        public string RejectFieldText { get; set; }
        public bool AllowToCancel { get; set; }
        public bool AllowToDelete { get; set; }
        public bool AllowRequestToChange { get; set; }
        public bool IsCustomEvent { get; set; }
        public string CustomEventKey { get; set; }
        public bool IsCustomRequestToChange { get; set; }
        public string CurrentStatus { get; set; }
        public IList<RestrictedProperty> RestrictedProperties { get; set; }
        public WorkflowHistoryViewModel LastHistory { get; set; }
        public IList<WorkflowInstanceViewModel> WorkflowInstances { get; set; }
        public string WorkflowComment { get; set; }
        public IList<WorkflowButton> WorkflowButtons { get; set; }
        public bool IsAttachmentFile { get; set; }
    }
    public class WorkflowButton
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
