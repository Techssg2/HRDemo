using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class FlightDetailDTO
    {
        public string GroupId { get; set; }
        public string Airline { get; set; }
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string FlightType { get; set; }
        public string RoundType { get; set; }
        public string OriginLocationCode { get; set; }
        public string OriginLocationName { get; set; }
        public string OriginCity { get; set; }
        public string OriginCountryCode { get; set; }
        public string OriginCountry { get; set; }
        public string DestinationLocationCode { get; set; }
        public string DestinationLocationName { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCountryCode { get; set; }
        public string DestinationCountry { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string ImgAirLogo { get; set; }
        public string PricedItineraries { get; set; }
        public string DataFight { get; set; }
    }
}
