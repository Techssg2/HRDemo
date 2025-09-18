using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class CancelPendingTargetArg
    {
        public Guid PendingTargetId { get; set; }
        public Guid DeptId { get; set; }
        public Guid PeriodId { get; set; }
        public string SAPCode { get; set; }

    }
}
