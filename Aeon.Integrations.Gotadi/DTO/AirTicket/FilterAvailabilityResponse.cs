using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FilterAvailabilityResponse
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("groupPricedItineraries")]
        public List<GroupPricedItinerary> GroupPricedItineraries { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
    }
}
