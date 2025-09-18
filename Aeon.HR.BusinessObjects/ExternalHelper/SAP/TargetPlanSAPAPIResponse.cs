using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.ExternalHelper.SAP
{
    public class TargetPlanSAPAPIResponse
    {
        public DModel d { get; set; }
        public class DModel
        {
            public string Pernr { get; set; }
		    public string Zyear { get; set; }
            public string Period { get; set; }
            public string RefNum { get; set; }
            public string EdocUser { get; set; }
            public string Status { get; set; }
            public string Err_log { get; set; }
        }
    }
}
