using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{

    public class DraftBookingRequest
    {
        [JsonProperty("itineraryInfos")]
        public List<ItineraryInfo> ItineraryInfos { get; set; }
    }

}
