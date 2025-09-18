using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class NavigationListUserDepartmentViewModel
    {
        public class User
        {
            public Guid id;
            public string FullName;
            public string SAPCode;
            public string name => (FullName + " - [" + SAPCode + "]");

        }
        public class Department
        {
            public Guid id;
            public string Code;
            public string Name;
        }
    }
}
