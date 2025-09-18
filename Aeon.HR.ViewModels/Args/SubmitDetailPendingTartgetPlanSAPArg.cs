using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class SubmitDetailPendingTartgetPlanSAPArg
    {
        public Guid PeriodId { get; set; }
        public string ListSAPCode { get; set; }
    }
}
