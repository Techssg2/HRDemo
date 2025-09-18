using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class PropertyCategory
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
