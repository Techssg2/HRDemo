using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.PrintFormViewModel
{
    public class ActingPrintFormViewModel
    {
        public string UserSAPCode { get; set; }
        public string FullName { get; set; }
        public string CurrentTitle { get; set; }
        public string TitleInActingPeriod { get; set; }
        public string DeptLineName { get; set; }
        public string Appraiser1FullName { get; set; }
        public string Appraiser2FullName { get; set; }
        public string WorkLocation { get; set; }
        public string ActingPeriodTime { get; set; }
        public string FirstAppraiserConfirmation { get; set; }
        public string SecondAppraiserConfirmation { get; set; }
        public string HRManager { get; set; }
        public string FirstAppraiserComment { get; set; }
        public string SecondAppraiserComment { get; set; }
        public string FirstAppraiserNote { get; set; }
        public string SecondAppraiserNote { get; set; }
    }
}
