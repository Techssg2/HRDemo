using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ItineraryInfo
    {
        [JsonProperty("fareSourceCode")]
        public string FareSourceCode { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("searchId")]
        public string SearchId { get; set; }
    }

}
