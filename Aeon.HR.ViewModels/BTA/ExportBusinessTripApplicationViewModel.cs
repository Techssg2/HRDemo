using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ExportBusinessTripApplicationViewModel
    {
        public string Hotel { get; set; }
        public string RoomType { get; set; }
        public string FlightNumber { get; set; }
        public string Status { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string DepartmentName { get; set; }
        public string HotelName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool? IsCancel { get; set; }
        public string Reason { get; set; }
        public string DetailReason { get; set; }
        public double TotalDays { get; set; }
        public string ReferenceNumber { get; set; }
        public string FlightNumberName { get; set; }
        public string CheckInHotelDate { get; set; }
        public string CheckOutHotelDate { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string IDCard { get; set; }
        public string Passport { get; set; }
        public string DepartFlight { get; set; }
        public string ReturnFlight { get; set; }
        public string HasBudget { get; set; }
        public string OnTime { get; set; }
        public string DeptLine { get; set; }
        public string BTAApprovedDay { get; set; }
        public string BusinessTripType { get; set; }
    }
}
