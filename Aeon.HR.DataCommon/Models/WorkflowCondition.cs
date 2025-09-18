using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class WorkflowCondition
    {
        public string FieldName { get; set; }
        public IList<string> FieldValues { get; set; }
    }
}
