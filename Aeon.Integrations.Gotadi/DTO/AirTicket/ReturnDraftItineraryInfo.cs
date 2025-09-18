using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ReturnDraftItineraryInfo
    {
        [JsonProperty("bookingDirection")]
        public string BookingDirection { get; set; }

        [JsonProperty("fareSourceCode")]
        public string FareSourceCode { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("itinTotalFare")]
        public ItinTotalFare ItinTotalFare { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }
    }
}
