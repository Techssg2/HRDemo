using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "AddMissingTimeclockSet")]
    public class MissingTimeClockInfo: ISAPEntity
    {
        public string RequestFrom { get; set; }
        public string EmployeeCode { get; set; }
        public string Date { get; set; }
        public string ActualTimeIn { get; set; }
    }
}
