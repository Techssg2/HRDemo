using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanQueryArg
    {
        public TargetPlanQueryArg()
        {
            SAPCodes = new List<string>();
            ActiveUsers = new List<string>();
            ValidUsersSubmit = new List<string>();
        }
        public Guid DepartmentId { get; set; }
        public Guid? DivisionId { get; set; }
        public Guid PeriodId { get; set; }
        public List<string> SAPCodes { get; set; }
        public List<string> ActiveUsers { get; set; }
        public List<string> ValidUsersSubmit { get; set; }
    }
}
