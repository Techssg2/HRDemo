using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FareRule
    {
        [JsonProperty("arrivalAirportLocationCode")]
        public string ArrivalAirportLocationCode { get; set; }

        [JsonProperty("arrivalAirportLocationName")]
        public object ArrivalAirportLocationName { get; set; }

        [JsonProperty("arrivalCity")]
        public object ArrivalCity { get; set; }

        [JsonProperty("arrivalDateTime")]
        public DateTime ArrivalDateTime { get; set; }

        [JsonProperty("departureAirportLocationCode")]
        public string DepartureAirportLocationCode { get; set; }

        [JsonProperty("departureAirportLocationName")]
        public object DepartureAirportLocationName { get; set; }

        [JsonProperty("departureCity")]
        public object DepartureCity { get; set; }

        [JsonProperty("departureDateTime")]
        public DateTime DepartureDateTime { get; set; }

        [JsonProperty("fareRuleItems")]
        public List<FareRuleItem> FareRuleItems { get; set; }

        [JsonProperty("operatingAirline")]
        public OperatingAirline OperatingAirline { get; set; }
    }
}
