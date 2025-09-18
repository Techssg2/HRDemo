using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class TaxReceiptRequestInfo
    {
        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("taxReceiptRequest")]
        public bool TaxReceiptRequest { get; set; }
    }
}
