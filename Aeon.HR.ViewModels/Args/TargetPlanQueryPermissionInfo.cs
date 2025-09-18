using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanQueryPermissionInfo
    {
        public TargetPlanQueryPermissionInfo()
        {
            ActiveUsers = new List<string>();
            ValidUsersSubmit = new List<string>();
        }
        public Guid PeriodId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid[] DivisionIds { get; set; }
        public List<string> ActiveUsers { get; set; }
        public List<string> ValidUsersSubmit { get; set; }
    }
}
