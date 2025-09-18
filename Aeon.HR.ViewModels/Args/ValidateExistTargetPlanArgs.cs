using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ValidateExistTargetPlanArgs
    {
        public Guid PeriodId { get; set; }
        public List<string> SapCodes { get; set; }
    }
}
