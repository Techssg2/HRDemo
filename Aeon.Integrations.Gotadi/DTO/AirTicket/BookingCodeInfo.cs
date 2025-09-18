using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BookingCodeInfo
    {
        [JsonProperty("bookingCode")]
        public string BookingCode { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }
    }
}
