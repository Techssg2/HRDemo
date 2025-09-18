using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FilterGroupIntinerary
    {
        [JsonProperty("airlineOptions")]
        public List<object> AirlineOptions { get; set; }

        [JsonProperty("arrivalDateTimeOptions")]
        public List<object> ArrivalDateTimeOptions { get; set; }

        [JsonProperty("arrivalDateTimeReturnOptions")]
        public List<object> ArrivalDateTimeReturnOptions { get; set; }

        [JsonProperty("cabinClassOptions")]
        public List<object> CabinClassOptions { get; set; }

        [JsonProperty("departureDateTimeOptions")]
        public List<object> DepartureDateTimeOptions { get; set; }

        [JsonProperty("departureDateTimeReturnOptions")]
        public List<object> DepartureDateTimeReturnOptions { get; set; }

        [JsonProperty("flightType")]
        public string FlightType { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("priceItineraryId")]
        public string PriceItineraryId { get; set; }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("stopOptions")]
        public List<object> StopOptions { get; set; }

        [JsonProperty("ticketPolicyOptions")]
        public List<object> TicketPolicyOptions { get; set; }
    }
}
