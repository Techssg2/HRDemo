using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FarerulesResponse
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("fareRules")]
        public List<FareRule> FareRules { get; set; }

        [JsonProperty("fareSourceCode")]
        public object FareSourceCode { get; set; }

        [JsonProperty("groupId")]
        public object GroupId { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("searchId")]
        public object SearchId { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
    }
}
