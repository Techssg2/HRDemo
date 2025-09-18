using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class GroupIntineraryRequest
    {
        [JsonProperty("searchId")]
        public string SearchId { get; set; }
        [JsonProperty("filter")]
        public FilterGroupIntinerary Filter { get; set; }
        [JsonProperty("departureItinerary")]
        public Itinerary DepartureItinerary { get; set; }
        public class Itinerary
        {
            [JsonProperty("fareSourceCode")]
            public string FareSourceCode { get; set; }
        }
    }
}