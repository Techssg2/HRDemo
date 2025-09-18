using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ShiftPlanSubmitPersonViewModel
    {
        public Guid DepartmentId { get; set; }
        public bool? IsIncludeChildren { get; set; }
        public string DepartmentName { get; set; }
        public List<Guid> UserIds { get; set; }
        public List<string> UserNames { get; set; }
        public List<UserListViewModel> UserListViews { get; set; }
    }
}
