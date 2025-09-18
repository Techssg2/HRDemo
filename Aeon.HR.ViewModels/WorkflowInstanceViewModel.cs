using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class WorkflowInstanceViewModel
    {
        public string WorkflowName { get; set; }
        public Guid TemplateId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemReferenceNumber { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public bool IsCompleted { get; set; }
        public virtual Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public IList<WorkflowHistoryViewModel> Histories { get; set; }
        public WorkflowDataViewModel WorkflowData { get; set; }
    }
}
