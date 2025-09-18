using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class GroupPricedItineraryViewModel
    {
        public string AirSupplier { get; set; }
        public string Aircraft { get; set; }
        public string VnaArea { get; set; }
        public string Airline { get; set; }
        public string AirlineName { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCountry { get; set; }
        public string DestinationCountryCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string DestinationLocationName { get; set; }
        public string FlightNo { get; set; }
        public string FlightType { get; set; }
        public Guid GroupId { get; set; }
        public string OriginCity { get; set; }
        public string OriginCountry { get; set; }
        public string OriginCountryCode { get; set; }
        public string OriginLocationCode { get; set; }
        public string OriginLocationName { get; set; }
        public string RequiredFields { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public string RoundType { get; set; }
        public int totalPricedItinerary { get; set; }
        public List<PricedItineraryViewModel> PricedItineraries { get; set; }
    }
}
