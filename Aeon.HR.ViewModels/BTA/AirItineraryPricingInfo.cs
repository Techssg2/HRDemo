namespace Aeon.HR.ViewModels
{
    public class AirItineraryPricingInfo
    {
        public bool DivideInPartyIndicator { get; set; }
        public object FareInfoReferences { get; set; }
        public object ChildFare { get; set; }
        public object InfantFare { get; set; }
        public string FareSourceCode { get; set; }
        public string FareType { get; set; }
        public ItinTotalFareViewModel ItinTotalFare { get; set; }
        public AdultFare AdultFare { get; set; }
    }
}
