using System.Collections.Generic;

namespace Aeon.Academy.Common.Workflow
{
    public class WorkflowCondition
    {
        public string FieldName { get; set; }
        public IList<string> FieldValues { get; set; }
    }
}
