using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class FareRulesRequestArgs
    {
        [JsonProperty("fareSourceCode")]
        public string FareSourceCode { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }
    }
}
