using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AirportInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cityCode")]
        public string CityCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("iata")]
        public string Iata { get; set; }

        [JsonProperty("icao")]
        public object Icao { get; set; }

        [JsonProperty("latitude")]
        public object Latitude { get; set; }

        [JsonProperty("longitude")]
        public object Longitude { get; set; }

        [JsonProperty("altitude")]
        public object Altitude { get; set; }

        [JsonProperty("timezone")]
        public double Timezone { get; set; }

        [JsonProperty("dts")]
        public object Dts { get; set; }

        [JsonProperty("tzTimezone")]
        public object TzTimezone { get; set; }

        [JsonProperty("updatedAt")]
        public object UpdatedAt { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("name2")]
        public string Name2 { get; set; }

        [JsonProperty("city2")]
        public string City2 { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("connectedPorts")]
        public object ConnectedPorts { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("stateCode")]
        public object StateCode { get; set; }

        [JsonProperty("status")]
        public object Status { get; set; }

        [JsonProperty("keywords")]
        public object Keywords { get; set; }
    }
}
