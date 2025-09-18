using Aeon.HR.Infrastructure.Enums;
using Newtonsoft.Json;
using System;

namespace Aeon.HR.ViewModels
{
    public class FlightDetailViewModel
    {
        public Guid Id { get; set; }
        public string SearchId { get; set; }
        public string DepartureSearchId { get; set; }
        public string ReturnSearchId { get; set; }
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
        public bool DirectFlight { get; set; }
        public string PricedItinerariesStr { get; set; }
        public PricedItinerariesViewModel PricedItineraries
        {
            get
            {
                PricedItinerariesViewModel returnValue = null;
                if (!string.IsNullOrEmpty(PricedItinerariesStr))
                {
                    returnValue = JsonConvert.DeserializeObject<PricedItinerariesViewModel>(PricedItinerariesStr);
                }
                return returnValue;
            }
            set
            {
                if (value is null || value.GetType() != typeof(PricedItinerariesViewModel))
                {
                    PricedItinerariesStr = string.Empty;
                }
                else
                {
                    PricedItinerariesStr = JsonConvert.SerializeObject(value);
                }
            }
        }
        public decimal IncludeEquivfare { get; set; }
        // Departure
        public string TitleDepartureFareRule { get; set; }
        public string DetailDepartureFareRule { get; set; }
        // Return
        public string TitleReturnFareRule { get; set; }
        public string DetailReturnFareRule { get; set; }
        public string FareSourceCode { get; set; }  
    }
}
