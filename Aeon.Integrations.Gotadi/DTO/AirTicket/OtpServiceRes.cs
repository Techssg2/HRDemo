using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class OtpServiceRes
    {
        [JsonProperty("duration")]
        public object Duration { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }

        [JsonProperty("expDate")]
        public object ExpDate { get; set; }

        [JsonProperty("expired")]
        public object Expired { get; set; }

        [JsonProperty("fullQuota")]
        public object FullQuota { get; set; }

        [JsonProperty("infos")]
        public object Infos { get; set; }

        [JsonProperty("isSuccess")]
        public object IsSuccess { get; set; }

        [JsonProperty("lifeTimeInMin")]
        public object LifeTimeInMin { get; set; }

        [JsonProperty("matched")]
        public object Matched { get; set; }

        [JsonProperty("notFound")]
        public bool NotFound { get; set; }

        [JsonProperty("outOfSlot")]
        public object OutOfSlot { get; set; }

        [JsonProperty("phoneNumber")]
        public object PhoneNumber { get; set; }

        [JsonProperty("serviceID")]
        public object ServiceID { get; set; }

        [JsonProperty("smsServiceAvailable")]
        public object SmsServiceAvailable { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("tag")]
        public object Tag { get; set; }

        [JsonProperty("textMessage")]
        public object TextMessage { get; set; }

        [JsonProperty("used")]
        public object Used { get; set; }

        [JsonProperty("verificationCode")]
        public object VerificationCode { get; set; }
    }
}
