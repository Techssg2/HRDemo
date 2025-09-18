using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ErrorCodeInfo
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
