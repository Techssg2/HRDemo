using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ServiceRequest
    {
        [JsonProperty("bookingDirection")]
        public string BookingDirection { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("fareCode")]
        public string FareCode { get; set; }

        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("ssrAmount")]
        public int SsrAmount { get; set; }

        [JsonProperty("ssrCode")]
        public string SsrCode { get; set; }

        [JsonProperty("ssrId")]
        public string SsrId { get; set; }

        [JsonProperty("ssrName")]
        public string SsrName { get; set; }
    }
}
