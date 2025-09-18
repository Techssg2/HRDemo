using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AirItineraryPricingInfo
    {
        [JsonProperty("fareSourceCode")]
        public string FareSourceCode { get; set; }

        [JsonProperty("fareType")]
        public string FareType { get; set; }

        [JsonProperty("divideInPartyIndicator")]
        public bool DivideInPartyIndicator { get; set; }

        [JsonProperty("fareInfoReferences")]
        public object FareInfoReferences { get; set; }

        [JsonProperty("itinTotalFare")]
        public ItinTotalFare ItinTotalFare { get; set; }

        [JsonProperty("adultFare")]
        public AdultFare AdultFare { get; set; }

        [JsonProperty("childFare")]
        public object ChildFare { get; set; }

        [JsonProperty("infantFare")]
        public object InfantFare { get; set; }
    }
}
