using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanQuerySAPCodeArg
    {     
        public Guid? DeptId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public bool VisibleSubmit { get; set;}
        public List<string> SAPCodes { get; set; }
        public List<string> AllSAPCodes { get; set; }
    }
}
