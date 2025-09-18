using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "ADDLeaveBalanceSet")]
    public class LeaveApplicationInfo: ISAPEntity
    {   
        public string EmployeeCode { get; set; }      
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string LeaveKind { get; set; }     
        public string UserEdoc { get; set; }
    }
}
