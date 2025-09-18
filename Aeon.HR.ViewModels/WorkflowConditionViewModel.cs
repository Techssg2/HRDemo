using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aeon.HR.ViewModels
{
    public class WorkflowConditionViewModel
    {
        public string FieldName { get; set; }
        public IList<string> FieldValues { get; set; }
    }
}
