using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class PricedItinerary
    {
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("directionInd")]
        public string DirectionInd { get; set; }

        [JsonProperty("ticketType")]
        public string TicketType { get; set; }

        [JsonProperty("validatingAirlineCode")]
        public string ValidatingAirlineCode { get; set; }

        [JsonProperty("validatingAirlineName")]
        public string ValidatingAirlineName { get; set; }

        [JsonProperty("flightNo")]
        public string FlightNo { get; set; }

        [JsonProperty("airItineraryPricingInfo")]
        public AirItineraryPricingInfo AirItineraryPricingInfo { get; set; }

        [JsonProperty("originDestinationOptions")]
        public List<OriginDestinationOption> OriginDestinationOptions { get; set; }

        [JsonProperty("cabinClassName")]
        public string CabinClassName { get; set; }

        [JsonProperty("validReturnCabinClasses")]
        public object ValidReturnCabinClasses { get; set; }

        [JsonProperty("baggageItems")]
        public List<BaggageItem> BaggageItems { get; set; }

        [JsonProperty("mealItems")]
        public object MealItems { get; set; }

        [JsonProperty("allowHold")]
        public bool AllowHold { get; set; }

        [JsonProperty("refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("onlyPayLater")]
        public bool OnlyPayLater { get; set; }

        [JsonProperty("passportMandatory")]
        public bool PassportMandatory { get; set; }
    }
}
