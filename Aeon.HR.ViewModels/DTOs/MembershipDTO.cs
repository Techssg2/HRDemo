using Newtonsoft.Json;
using System;

namespace Aeon.HR.ViewModels.DTOs
{
    public class BookingContactDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
    }
    public class MembershipDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("airlineCode")]
        public string AirlineCode { get; set; }
        [JsonProperty("airlineName")]
        public string AirlineName { get; set; }
        [JsonProperty("airlineId")]
        public string AirlineId { get; set; }
        [JsonProperty("idCard")]
        public string IdCard { get; set; }
    }
}
