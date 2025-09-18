using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class DivisionSearchByDeptLine
    {
        public Guid DeptId { get; set; }
        public List<Guid> DivisionIds { get; set; }
        public string Filter { get; set; }
    }
}
