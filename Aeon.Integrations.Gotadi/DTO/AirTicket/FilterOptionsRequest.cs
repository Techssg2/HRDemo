using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FilterOptionsRequest
    {
        [JsonProperty("searchId")]
        public string SearchId { get; set; }

        [JsonProperty("departureItinerary")]
        public Itinerary DepartureItinerary { get; set; }
    }
}
