using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class OperatingAirline
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("equipment")]
        public object Equipment { get; set; }

        [JsonProperty("flightNumber")]
        public string FlightNumber { get; set; }
    }
}
