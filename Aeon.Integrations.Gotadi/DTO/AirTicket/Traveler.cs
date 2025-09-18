using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class Traveler
    {
        [JsonProperty("adultType")]
        public string AdultType { get; set; }

        [JsonProperty("documentType")]
        public object DocumentType { get; set; }

        [JsonProperty("documentNumber")]
        public object DocumentNumber { get; set; }

        [JsonProperty("documentExpiredDate")]
        public object DocumentExpiredDate { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("memberCard")]
        public bool MemberCard { get; set; }

        [JsonProperty("memberCardType")]
        public object MemberCardType { get; set; }

        [JsonProperty("memberCardNumber")]
        public object MemberCardNumber { get; set; }

        [JsonProperty("surName")]
        public string SurName { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("dob")]
        public object Dob { get; set; }

        [JsonProperty("documentIssuingCountry")]
        public string DocumentIssuingCountry { get; set; }
    }
}
