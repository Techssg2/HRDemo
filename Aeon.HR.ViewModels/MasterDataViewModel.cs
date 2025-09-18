
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MasterDataViewModel
    {      
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        public dynamic RawData { get; set; }
        [JsonProperty("NameVN")]
        public string NameVN { get; set; }
        [JsonProperty("StartTime")]
        public string StartTime { get; set; }
        [JsonProperty("EndTime")]
        public string EndTime { get; set; }
        public string JobGradeCaption { get; set; }
        public string JobGradeTitle { get; set; }
    }
    
}

