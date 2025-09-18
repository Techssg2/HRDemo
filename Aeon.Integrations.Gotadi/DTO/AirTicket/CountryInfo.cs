using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class CountryInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sortname")]
        public string SisortnamesortnametyCode { get; set; }

        [JsonProperty("iso2Code")]
        public string iso2Code { get; set; }

        [JsonProperty("iso3Code")]
        public string iso3Code { get; set; }

        [JsonProperty("phoneCode")]
        public string PhoneCode { get; set; }

        [JsonProperty("iata")]
        public string Iata { get; set; }

        [JsonProperty("icao")]
        public object Icao { get; set; }

        [JsonProperty("continentCode")]
        public string ContinentCode { get; set; }

        [JsonProperty("continent")]
        public string Continent { get; set; }

        [JsonProperty("capital")]
        public string Capital { get; set; }

        [JsonProperty("tzCapital")]
        public string TzCapital { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("updatedAt")]
        public object UpdatedAt { get; set; }
    }
}
