using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class GetFilterOptionsResponse
    {
        [JsonProperty("result")]
        public FilterOptions Result { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
