using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class UserInAllDepartmentViewModel
    {
        public UserInAllDepartmentViewModel()
        {
            Divisions = new List<DepartmentViewModel>();
        }
        public DepartmentViewModel DeptLine { get; set; }
        public List<DepartmentViewModel> Divisions { get; set; }
    }
}
