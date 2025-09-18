using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BookingContact
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("phoneCode1")]
        public string PhoneCode1 { get; set; }

        [JsonProperty("phoneNumber1")]
        public string PhoneNumber1 { get; set; }

        [JsonProperty("surName")]
        public string SurName { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }
    }
}
