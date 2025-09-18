using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TargetPlanExportViewModel
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string SubmitterSAPCode { get; set; }
        public string SubmitterFullName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string DeptLine { get; set; }
        public string DivisionGroup { get; set; }
        public string Period { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

    }
}
