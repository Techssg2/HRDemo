using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FareRuleItem
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
