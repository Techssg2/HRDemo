using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class OriginDestinationOption
    {
        [JsonProperty("originLocationCode")]
        public string OriginLocationCode { get; set; }

        [JsonProperty("originLocationName")]
        public string OriginLocationName { get; set; }

        [JsonProperty("originCity")]
        public string OriginCity { get; set; }

        [JsonProperty("originDateTime")]
        public DateTime OriginDateTime { get; set; }

        [JsonProperty("destinationLocationCode")]
        public string DestinationLocationCode { get; set; }

        [JsonProperty("destinationLocationName")]
        public string DestinationLocationName { get; set; }

        [JsonProperty("destinationCity")]
        public string DestinationCity { get; set; }

        [JsonProperty("destinationDateTime")]
        public DateTime DestinationDateTime { get; set; }

        [JsonProperty("flightDirection")]
        public string FlightDirection { get; set; }

        [JsonProperty("journeyDuration")]
        public int JourneyDuration { get; set; }

        [JsonProperty("flightSegments")]
        public List<FlightSegment> FlightSegments { get; set; }

        [JsonProperty("cabinClassName")]
        public string CabinClassName { get; set; }
    }
}
