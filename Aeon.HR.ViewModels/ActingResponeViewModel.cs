using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ActingResponeViewModel
    {
        public ActingViewModel Acting { get; set; }
        public IEnumerable<PeriodViewModel> Period { get; set; }
    }
}