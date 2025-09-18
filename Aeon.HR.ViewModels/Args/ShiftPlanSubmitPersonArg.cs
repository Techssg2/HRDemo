using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ShiftPlanSubmitPersonArg
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public List<Guid> UserIds { get; set; }
        public Guid DepartmentIdOld { get; set; }
        public bool IsIncludeChildren { get; set; }
    }
}
