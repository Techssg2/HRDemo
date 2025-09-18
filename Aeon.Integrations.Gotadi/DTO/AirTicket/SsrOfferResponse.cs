using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class SsrOfferResponse
    {
        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("departSsrOfferItems")]
        public List<DepartSsrOfferItem> DepartSsrOfferItems { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("returnSsrOfferItems")]
        public List<ReturnSsrOfferItem> ReturnSsrOfferItems { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
    }
}
