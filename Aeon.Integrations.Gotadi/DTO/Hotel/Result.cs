using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class Result
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }
    }

    
}
