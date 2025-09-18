using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class DraftBookingResponse
    {
        [JsonProperty("bookingCode")]
        public BookingCodeInfo BookingInfo { get; set; }

        [JsonProperty("bookingType")]
        public string BookingType { get; set; }

        [JsonProperty("departDraftItineraryInfo")]
        public DepartDraftItineraryInfo DepartDraftItineraryInfo { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("isPerBookingType")]
        public bool IsPerBookingType { get; set; }

        [JsonProperty("isRoundTripType")]
        public bool IsRoundTripType { get; set; }

        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("markupType")]
        public string MarkupType { get; set; }

        [JsonProperty("returnDraftItineraryInfo")]
        public ReturnDraftItineraryInfo ReturnDraftItineraryInfo { get; set; }

        [JsonProperty("roundType")]
        public string RoundType { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
    }
}
