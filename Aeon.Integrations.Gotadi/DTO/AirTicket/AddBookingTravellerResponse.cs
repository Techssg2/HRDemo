using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AddBookingTravellerResponse
    {
        [JsonProperty("bookingCode")]
        public BookingCodeInfo BookingCode { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("isSuccess")]
        public object IsSuccess { get; set; }

        [JsonProperty("otpServiceRes")]
        public OtpServiceRes OtpServiceRes { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }
    }
}
