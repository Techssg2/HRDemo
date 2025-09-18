using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ActingGoalViewModel
    {
        public string Goal { get; set; }
        public double Weight { get; set; }
    }
    public class PeriodAppraisingViewModel
    {
        public string Goal { get; set; }
        public string Target { get; set; }
        public string Actual { get; set; }
    }
}
