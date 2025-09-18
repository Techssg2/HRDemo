using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class MasterActingArgs
    {
        public ActingArgs Acting { get; set; }
        public List<PeriodArgs> Periods { get; set; }
    }
}