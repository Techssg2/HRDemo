using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class Content
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("searchCode")]
        public string SearchCode { get; set; }

        [JsonProperty("searchType")]
        public string SearchType { get; set; }

        [JsonProperty("supplier")]
        public string Supplier { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("propertyCount")]
        public int PropertyCount { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }

    
}
