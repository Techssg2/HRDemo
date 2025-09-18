using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "ResignationSet")]
    public class ResignationDataInfo: ISAPEntity
    {
        public string RequestFrom { get; set; }
        public string EmployeeCode { get; set; }
        public string OfficialResignationDate { get; set; }
        public string ActionType { get; set; }
        public string Reason { get; set; }
        public string SubmitDate { get; set; }
    }
}
