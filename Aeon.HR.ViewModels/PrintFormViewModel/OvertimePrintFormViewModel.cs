using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.PrintFormViewModel
{
    public class OvertimePrintFormViewModel
    {
        public string Department { get; set; }
        public string DepartmentCode { get; set; }
        public string DeptLine { get; set; }
        public string Location { get; set; }
        public string Reason { get; set; }
        public string PreparedBy { get; set; }
        public string CheckByHR { get; set; }
        public string ApprovedDepartmentHead { get; set; }
        public string ApprovedSeniorGeneralManager { get; set; }
        public bool IsHQ { get; set; }
        public int Grade { get; set; }

    }
    public class OvertimeApplicationDetaiForPrintlViewModel
    {      
        public DateTimeOffset Date { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string DetailReason { get; set; }
        public string ProposalHoursFrom { get; set; }       
        public string ProposalHoursTo { get; set; }
        public string ProposalHours { get; set; }
        public string ActualHoursFrom { get; set; }
        public string ActualHoursTo { get; set; }
        public string ActualHours { get; set; }
        public bool DateOffInLieu { get; set; }
        public bool IsNoOT { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }     
    }

}
