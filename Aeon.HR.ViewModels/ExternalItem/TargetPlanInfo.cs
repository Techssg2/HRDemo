using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels.Args;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "TargetPlansSet")]
    public class TargetPlanInfo : ISAPEntity
    {
        public string ReferenceNumber { get; set; } // RefNum
        public string SapCode { get; set; } // Perm
        public string PeriodYear { get; set; } // Perm
        public string PeriodMonth { get; set; } // Perm
        public string RequestUser { get; set; }
        //public SubmitTargetPlanArgDataValue Values { get; set; }
        [JsonProperty("TargetData01Set")]
        public List<DateValueItem> Target1 { get; set; }
        [JsonProperty("TargetData02Set")]
        public List<DateValueItem> Target2 { get; set; }
    }
    public class DateValueItem { 
        public string Zdate { get; set; }
        public string Tprog { get; set; }
    }
}
