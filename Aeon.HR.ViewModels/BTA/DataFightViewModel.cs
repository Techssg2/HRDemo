using System;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class OriginDestinationOptionsViewModel
    {
        public string OriginLocationCode { get; set; }
        public string OriginLocationName { get; set; }
        public string OriginCity { get; set; }
        public DateTime OriginDateTime { get; set; }
        public string DestinationLocationCode { get; set; }
        public string DestinationLocationName { get; set; }
        public string DestinationCity { get; set; }
        public DateTime DestinationDateTime { get; set; }
        public string FlightDirection { get; set; }
        public string JourneyDuration { get; set; }
        public string CabinClassName { get; set; }
        public List<FlightSegmentViewModel> FlightSegments { get; set; }
    }
}
