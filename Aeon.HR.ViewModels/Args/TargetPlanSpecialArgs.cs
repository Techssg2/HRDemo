using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanSpecialArgs
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsIncludeChildren { get; set; }
        public Guid DepartmentIdOld { get; set; }

    }
}
