using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class PricedItineraryViewModel
    {
        public object AirItineraryPricingInfo { get; set; }
        public bool AllowHold { get; set; }
        public string CabinClassName { get; set; }
        public string DirectionInd { get; set; }
        public string FlightNo { get; set; }
        public string MealItems { get; set; }
        public bool OnlyPayLater { get; set; }
        public bool PassportMandatory { get; set; }
        public bool Refundable { get; set; }
        public string sequenceNumber { get; set; }
        public string TicketType { get; set; }
        public string ValidReturnCabinClasses { get; set; }
        public string ValidatingAirlineCode { get; set; }
        public string ValidatingAirlineName { get; set; } 
        public List<BaggageItemViewModel> BaggageItems { get; set; }
        public List<OriginDestinationViewModel> OriginDestinationOptions { get; set; }
    }
}
