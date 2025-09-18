using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.API.ExternalItem
{
    [MyClass(APIName = "Promote_TransferSet")]
    public class PromoteAndTransferInfo: ISAPEntity
    {
        public string RequestFrom { get; set; }
        public string EmployeeCode { get; set; }
        public string EffectiveDate { get; set; }
        public string NewTitle { get; set; }
        public string NewDeptLine { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string Position { get; set; }
        public string EmployeeGroupCode { get; set; }
        public string EmployeeSubGroupCode { get; set; }
        public string PersonelSubarea { get; set; }
        public string PersonelArea { get; set; }
    }
}
