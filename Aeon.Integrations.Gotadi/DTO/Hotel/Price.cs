using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class Price
    {
        [JsonProperty("from")]
        public double From { get; set; }

        [JsonProperty("to")]
        public double To { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }
    }
}
