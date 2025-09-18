using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TargetPlanRequestToChangeArg
    {
        public TargetPlanRequestToChangeArg()
        {
            TargetPlanDetails = new List<RequestToChangeTargetDTO>();
        }
        public Guid PeriodId { get; set; }
        public List<RequestToChangeTargetDTO> TargetPlanDetails { get; set; }
    }
}
