using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BaggageItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("fareCode")]
        public object FareCode { get; set; }

        [JsonProperty("direction")]
        public object Direction { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
