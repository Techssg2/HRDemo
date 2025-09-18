using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class GroupPricedItinerary
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("airline")]
        public string Airline { get; set; }

        [JsonProperty("airlineName")]
        public string AirlineName { get; set; }

        [JsonProperty("airSupplier")]
        public string AirSupplier { get; set; }

        [JsonProperty("flightNo")]
        public string FlightNo { get; set; }

        [JsonProperty("flightType")]
        public string FlightType { get; set; }

        [JsonProperty("roundType")]
        public string RoundType { get; set; }

        [JsonProperty("originLocationCode")]
        public string OriginLocationCode { get; set; }

        [JsonProperty("originLocationName")]
        public string OriginLocationName { get; set; }

        [JsonProperty("originCity")]
        public string OriginCity { get; set; }

        [JsonProperty("originCountryCode")]
        public object OriginCountryCode { get; set; }

        [JsonProperty("originCountry")]
        public object OriginCountry { get; set; }

        [JsonProperty("destinationLocationCode")]
        public string DestinationLocationCode { get; set; }

        [JsonProperty("destinationLocationName")]
        public string DestinationLocationName { get; set; }

        [JsonProperty("destinationCity")]
        public string DestinationCity { get; set; }

        [JsonProperty("destinationCountryCode")]
        public object DestinationCountryCode { get; set; }

        [JsonProperty("destinationCountry")]
        public object DestinationCountry { get; set; }

        [JsonProperty("requiredFields")]
        public object RequiredFields { get; set; }

        [JsonProperty("aircraft")]
        public string Aircraft { get; set; }

        [JsonProperty("vnaArea")]
        public object VnaArea { get; set; }

        [JsonProperty("arrivalDateTime")]
        public DateTime ArrivalDateTime { get; set; }

        [JsonProperty("returnDateTime")]
        public DateTime? ReturnDateTime { get; set; }

        [JsonProperty("departureDateTime")]
        public DateTime DepartureDateTime { get; set; }

        [JsonProperty("totalPricedItinerary")]
        public int TotalPricedItinerary { get; set; }

        [JsonProperty("pricedItineraries")]
        public List<PricedItinerary> PricedItineraries { get; set; }

        [JsonProperty("tourCode")]
        public object TourCode { get; set; }

        [JsonProperty("osiCode")]
        public object OsiCode { get; set; }
    }
}
