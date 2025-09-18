using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class Filter
    {
        [JsonProperty("cabinClassOptions")]
        public List<object> CabinClassOptions { get; set; }

        [JsonProperty("ticketPolicyOptions")]
        public List<object> TicketPolicyOptions { get; set; }

        [JsonProperty("airlineOptions")]
        public List<object> AirlineOptions { get; set; }

        [JsonProperty("stopOptions")]
        public List<object> StopOptions { get; set; }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("flightType")]
        public string FlightType { get; set; }

        [JsonProperty("departureDateTimeOptions")]
        public List<object> DepartureDateTimeOptions { get; set; }

        [JsonProperty("arrivalDateTimeReturnOptions")]
        public List<object> ArrivalDateTimeReturnOptions { get; set; }

        [JsonProperty("arrivalDateTimeOptions")]
        public List<object> ArrivalDateTimeOptions { get; set; }

        [JsonProperty("priceItineraryId")]
        public string PriceItineraryId { get; set; }

        [JsonProperty("loadMore")]
        public bool LoadMore { get; set; }

        [JsonProperty("departureDateTimeReturnOptions")]
        public List<object> DepartureDateTimeReturnOptions { get; set; }
    }
}
