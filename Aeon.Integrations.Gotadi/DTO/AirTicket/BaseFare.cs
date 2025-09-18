using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BaseFare
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("currencyCode")]
        public object CurrencyCode { get; set; }

        [JsonProperty("decimalPlaces")]
        public int DecimalPlaces { get; set; }
    }
}
