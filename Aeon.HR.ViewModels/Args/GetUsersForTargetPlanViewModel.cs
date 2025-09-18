using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class GetUsersForTargetPlanViewModel
    {
        public Guid[] Ids { get; set;}
        public Guid? PeriodId { get; set; }
        public QueryArgs Args { get; set; }
    }
}
