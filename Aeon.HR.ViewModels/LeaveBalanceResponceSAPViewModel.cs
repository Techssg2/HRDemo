using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class LeaveBalanceResponceSAPViewModel
    {
        public string EmployeeCode { get; set; }
        public string AbsenceQuotaType { get; set; }
        public string AbsenceQuotaName { get; set; }
        public double CurrentYearBalance { get; set; }
        public double Deduction { get; set; }
        public double Remain { get { return CurrentYearBalance - Deduction; } }
        public string Year { get; set; }
        public string NewRemain { get; set; }
        public double EdocInUsed { get; set; }
        public double OTRemain { get; set; }
    }
}
