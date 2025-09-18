using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.Hotel
{
    public class Address
    {
        [JsonProperty("lineOne")]
        public string LineOne { get; set; }

        [JsonProperty("lineTow")]
        public string LineTow { get; set; }

        [JsonProperty("stateProvinceCode")]
        public string StateProvinceCode { get; set; }

        [JsonProperty("stateProvinceName")]
        public string StateProvinceName { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }
    }

    
}
