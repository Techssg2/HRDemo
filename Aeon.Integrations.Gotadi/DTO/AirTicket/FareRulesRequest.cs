using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FareRulesRequest
    {
        [JsonProperty("fareSourceCode")]
        public string FareSourceCode { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }
    }
}
