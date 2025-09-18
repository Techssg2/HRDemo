using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class PropertyRating
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
