using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class FlightSegmentViewModel
    {
        public string AdultBaggage { get; set; }
        public string Aircraft { get; set; }
        public string ArrivalAirportLocationCode { get; set; }
        public string ArrivalAirportLocationName { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string CabinClassCode { get; set; }
        public string CabinClassName { get; set; }
        public string CabinClassText { get; set; }
        public string ChildBaggage { get; set; }
        public string DepartureAirportLocationCode { get; set; }
        public string DepartureAirportLocationName { get; set; }
        public string DepartureCity { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public bool Eticket { get; set; }
        public string FareBasicCode { get; set; }
        public string FareCode { get; set; }
        public string FlightDirection { get; set; }
        public string FlightNumber { get; set; }
        public string InfantBaggage { get; set; }
        public string JourneyDuration { get; set; }
        public string MarketingAirlineCode { get; set; }
        public string MarriageGroup { get; set; }
        public string MealCode { get; set; }
        public OperatingAirlineViewModel OperatingAirline { get; set; }
        public string ResBookDesignCode { get; set; }
        public string SeatsRemaining { get; set; }
        public int StopQuantity { get; set; }
        public StopQuantityInfoViewModel StopQuantityInfo { get; set; }
        public string SupplierFareKey { get; set; }
        public string SupplierJourneyKey { get; set; }
        public ArrivalAirport arrivalAirport { get; set; }
        public DepartureAirport departureAirport { get; set; }
    }
}
