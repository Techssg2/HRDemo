using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class PricedItinerariesViewModel
    {
        public string CabinClassName { get; set; }
        public object ValidReturnCabinClasses { get; set; }
        public object BaggageItems { get; set; }
        public object MealItems { get; set; }
        public bool AllowHold { get; set; }
        public bool Refundable { get; set; }
        public bool OnlyPayLater { get; set; }
        public bool PassportMandatory { get; set; }
        public string SequenceNumber { get; set; }
        public string DirectionInd { get; set; }
        public string TicketType { get; set; }
        public string ValidatingAirlineCode { get; set; }
        public string ValidatingAirlineName { get; set; }
        public string FlightNo { get; set; }
        public AirItineraryPricingInfo AirItineraryPricingInfo { get; set; }
        public List<OriginDestinationOptionsViewModel> OriginDestinationOptions { get; set; }
    }
}
