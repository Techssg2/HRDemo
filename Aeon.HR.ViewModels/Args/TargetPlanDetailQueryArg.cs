using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanDetailQueryArg
    {
        public Guid DepartmentId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public List<string> SAPCodes { get; set; }
    }
}
