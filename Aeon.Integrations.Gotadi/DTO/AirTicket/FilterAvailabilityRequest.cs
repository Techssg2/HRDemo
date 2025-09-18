using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FilterAvailabilityRequest
    {
        [JsonProperty("searchId")]
        public string SearchId { get; set; }
        /*[JsonProperty("extractLowPrice")]
        public bool ExtractLowPrice { get; set; }*/
        [JsonProperty("filter")]
        public Filter Filter { get; set; }
        [JsonProperty("departureItinerary")]
        public object DepartureItinerary { get; set; }
    }
}
