using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class SalaryDayConfigurationArg
    {
        // public Guid Id { get; set; }
        public int SalaryPeriodFrom { get; set; }
        public int SalaryPeriodTo { get; set; }
        public int NewSalaryPeriod { get; set; }
        public int DeadlineOfSubmittingCABApplication { get; set; }
        public int CreatedNewPeriodDate { get; set; }
        public int DeadlineOfSubmittingCABHQ { get; set; }
        public int DeadlineOfSubmittingCABStore { get; set; }
        public int TimeOfSubmittingCABHQ { get; set; }
        public int TimeOfSubmittingCABStore { get; set; }
        public string DayConfigurationType { get; set; }
    }
}
