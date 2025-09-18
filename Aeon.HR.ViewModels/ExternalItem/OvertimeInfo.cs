using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aeon.HR.ViewModels.ExternalItem
{
 
    [MyClass(APIName = "AddOverTimeSet")]
    public class OvertimeInfo: ISAPEntity
    {
        public string RequestFrom { get; set; }
        public string EmployeeCode { get; set; }
        public string Date { get; set; }
        public string ActualHours { get; set; }
        public string ActualHoursFrom { get; set; }
        public string ActualHoursTo { get; set; }
        public string DateOffInLieuL { get; set; }
        public string DateOffInLieuL_1_2 { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
