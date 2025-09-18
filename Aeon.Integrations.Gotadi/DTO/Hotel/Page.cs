using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class Page
    {
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("totalPage")]
        public int TotalPage { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }
    }

    
}
