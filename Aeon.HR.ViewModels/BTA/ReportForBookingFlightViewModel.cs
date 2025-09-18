using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportForBookingFlightViewModel : IReportBTA
    {
        public string Status { get; set; }
        public string BTAStatus { get; set; }
        public string CancellationFee { get; set; }
        public string FlightInformation { get; set; }
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public decimal Cost { get; set; }
        public decimal ServiceFee { get; set; }
        public DateTime BookingDate { get; set; }
        public string ReasonforBusinessTrip { get; set; }
        public string ReasonforBusinessTripDetail { get; set; }
        public string ReasonforCancel { get; set; }
        public string ReferenceNumber { get; set; }
        public string DeptName { get; set; }
        public Guid? BTAId { get; set; }
    }
}
