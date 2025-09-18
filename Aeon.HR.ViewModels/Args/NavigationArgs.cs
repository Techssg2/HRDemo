using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class NavigationArgs
    {
        public string Predicate { get; set; }
        public string Limit { get; set; }

        public class GetListArgs
        {
            public Guid? UserId { get; set; }
            public Guid? DepartmentId { get; set; }
        }
    }
}
