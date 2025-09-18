using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class Surcharge
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("indicator")]
        public string Indicator { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
