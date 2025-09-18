using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "AddShiftExchangeDataSet")]
    public class ShiftExchangeDataInfo: ISAPEntity
    {
        public string RequestFrom { get; set; }
        public string EmployeeCode { get; set; }
        public string Date { get; set; }
        public string CurrentShift { get; set; }
        public string NewShift { get; set; }
        public string IsCheckedERD { get; set; }
        public string Del_flag { get; set; }
    }
}
