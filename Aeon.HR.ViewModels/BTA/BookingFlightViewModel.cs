using Aeon.HR.ViewModels.BTA;
using System;

namespace Aeon.HR.ViewModels
{
    public class BookingFlightViewModel
    {
        public Guid Id { get; set; }
        public Guid FlightDetailId { get; set; }
        public Guid BTADetailId { get; set; }
        public string BookingCode { get; set; }
        public string BookingNumber { get; set; }
        public string Status { get; set; }
        public decimal PenaltyFee { get; set; }
        public string Comments { get; set; }
        public bool IsCancel { get; set; }
        public string GroupId { get; set; }
        public bool DirectFlight { get; set; }
        public FlightDetailViewModel FlightDetail { get; set; }
        public BtaDetailViewModel BTADetail { get; set; }
    }
}
