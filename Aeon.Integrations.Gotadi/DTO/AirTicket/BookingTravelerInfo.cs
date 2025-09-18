using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BookingTravelerInfo
    {
        [JsonProperty("traveler")]
        public Traveler Traveler { get; set; }

        [JsonProperty("serviceRequests")]
        public List<ServiceRequest> ServiceRequests { get; set; }
    }
}
