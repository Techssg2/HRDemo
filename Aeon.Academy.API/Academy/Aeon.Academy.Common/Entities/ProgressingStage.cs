using System;
using System.Collections.Generic;
using Aeon.Academy.Common.Workflow;

namespace Aeon.Academy.Common.Entities
{
    public class ProgressingStage<T>
    {
        public ProgressingStage()
        {
            Histories = new List<T>();
        }
        public WorkflowData WorkflowData { get; set; }
        public List<T> Histories { get; set; }
    }
}
