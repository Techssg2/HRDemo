using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ItineraryFilter
    {
        [JsonProperty("airlineOptions")]
        public List<object> AirlineOptions { get; set; }

        [JsonProperty("arrivalDateTimeOptions")]
        public object ArrivalDateTimeOptions { get; set; }

        [JsonProperty("arrivalDateTimeReturnOptions")]
        public object ArrivalDateTimeReturnOptions { get; set; }

        [JsonProperty("cabinClassOptions")]
        public List<object> CabinClassOptions { get; set; }

        [JsonProperty("departureDateTimeOptions")]
        public object DepartureDateTimeOptions { get; set; }

        [JsonProperty("departureDateTimeReturnOptions")]
        public object DepartureDateTimeReturnOptions { get; set; }

        [JsonProperty("flightType")]
        public object FlightType { get; set; }

        [JsonProperty("groupId")]
        public object GroupId { get; set; }

        [JsonProperty("loadMore")]
        public object LoadMore { get; set; }

        [JsonProperty("minPrice")]
        public object MinPrice { get; set; }

        [JsonProperty("filterToPrice")]
        public double FilterToPrice { get; set; }

        [JsonProperty("filterFromPrice")]
        public double FilterFromPrice { get; set; }

        [JsonProperty("priceItineraryId")]
        public object PriceItineraryId { get; set; }

        [JsonProperty("step")]
        public object Step { get; set; }

        [JsonProperty("stopOptions")]
        public List<object> StopOptions { get; set; }

        [JsonProperty("ticketPolicyOptions")]
        public object TicketPolicyOptions { get; set; }
    }
}
