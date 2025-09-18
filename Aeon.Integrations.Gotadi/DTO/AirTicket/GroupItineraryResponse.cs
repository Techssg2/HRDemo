using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class GroupItineraryResponse
    {
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("errors")]
        public object Errors { get; set; }
        [JsonProperty("GroupPricedItinerary")]
        public GroupPricedItinerary GroupPricedItinerary { get; set; }
        [JsonProperty("infos")]
        public object Infos { get; set; }
        [JsonProperty("searchId")]
        public string SearchId { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
        [JsonProperty("GroupPricedItineraries")]
        public List<GroupPricedItinerary> GroupPricedItineraries
        {
            get
            {
                List<GroupPricedItinerary> list = new List<GroupPricedItinerary>();
                if (GroupPricedItinerary != null)
                {
                    list.Add(GroupPricedItinerary);
                }
                return list;
            }
        }
    }
}
