using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class SubmitTargetPlanArg
    {
        public string Period { get; set; }
        public string ReferenceNumber { get; set; }
        public string SapCode { get; set; }
        public SubmitTargetPlanArgDataValue Values { get; set; }
    }
    public class SubmitTargetPlanArgDataValue
    {
        public List<DateValueArgs> Target1 { get; set; }
        public List<DateValueArgs> Target2 { get; set; }
    }
}
