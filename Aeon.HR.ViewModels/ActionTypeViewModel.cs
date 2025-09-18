using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
   public class ActionTypeViewModel
    {
        [JsonProperty("Massn")]
        public string ActionTypeCode { get; set; }
        [JsonProperty("Mntxt")]
        public string ActionTypeName { get; set; }
        [JsonProperty("Massg")]
        public string ReasonTypeCode { get; set; }
        [JsonProperty("Mgtxt")]
        public string ReasonTypeName { get; set; }
    }
}
