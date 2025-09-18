using Newtonsoft.Json;
using System;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class FlightSegment
    {
        [JsonProperty("departureAirportLocationCode")]
        public string DepartureAirportLocationCode { get; set; }

        [JsonProperty("departureAirportLocationName")]
        public string DepartureAirportLocationName { get; set; }

        [JsonProperty("departureCity")]
        public string DepartureCity { get; set; }

        [JsonProperty("departureDateTime")]
        public DateTime DepartureDateTime { get; set; }

        [JsonProperty("arrivalAirportLocationCode")]
        public string ArrivalAirportLocationCode { get; set; }

        [JsonProperty("arrivalAirportLocationName")]
        public string ArrivalAirportLocationName { get; set; }

        [JsonProperty("arrivalCity")]
        public string ArrivalCity { get; set; }

        [JsonProperty("arrivalDateTime")]
        public DateTime ArrivalDateTime { get; set; }

        [JsonProperty("cabinClassCode")]
        public string CabinClassCode { get; set; }

        [JsonProperty("cabinClassName")]
        public string CabinClassName { get; set; }

        [JsonProperty("cabinClassText")]
        public string CabinClassText { get; set; }

        [JsonProperty("eticket")]
        public bool Eticket { get; set; }

        [JsonProperty("flightNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("journeyDuration")]
        public int JourneyDuration { get; set; }

        [JsonProperty("marketingAirlineCode")]
        public string MarketingAirlineCode { get; set; }

        [JsonProperty("marriageGroup")]
        public object MarriageGroup { get; set; }

        [JsonProperty("mealCode")]
        public object MealCode { get; set; }

        [JsonProperty("adultBaggage")]
        public object AdultBaggage { get; set; }

        [JsonProperty("childBaggage")]
        public object ChildBaggage { get; set; }

        [JsonProperty("infantBaggage")]
        public object InfantBaggage { get; set; }

        [JsonProperty("operatingAirline")]
        public OperatingAirline OperatingAirline { get; set; }

        [JsonProperty("resBookDesignCode")]
        public string ResBookDesignCode { get; set; }

        [JsonProperty("seatsRemaining")]
        public object SeatsRemaining { get; set; }

        [JsonProperty("stopQuantity")]
        public int StopQuantity { get; set; }

        [JsonProperty("stopQuantityInfo")]
        public object StopQuantityInfo { get; set; }

        [JsonProperty("flightDirection")]
        public string FlightDirection { get; set; }

        [JsonProperty("fareCode")]
        public object FareCode { get; set; }

        [JsonProperty("fareBasicCode")]
        public object FareBasicCode { get; set; }

        [JsonProperty("supplierJourneyKey")]
        public string SupplierJourneyKey { get; set; }

        [JsonProperty("supplierFareKey")]
        public object SupplierFareKey { get; set; }

        [JsonProperty("aircraft")]
        public string Aircraft { get; set; }

        [JsonProperty("arrivalAirport")]
        public ArrivalAirport arrivalAirport { get; set; }

        [JsonProperty("departureAirport")]
        public DepartureAirport departureAirport { get; set; }
    }
}
