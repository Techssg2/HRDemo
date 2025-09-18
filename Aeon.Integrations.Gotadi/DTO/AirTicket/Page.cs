using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class Page
    {
        [JsonProperty("nextPageNumber")]
        public int NextPageNumber { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("previousPageNumber")]
        public int PreviousPageNumber { get; set; }

        [JsonProperty("totalElements")]
        public int TotalElements { get; set; }

        [JsonProperty("totalPage")]
        public int TotalPage { get; set; }
    }
}
