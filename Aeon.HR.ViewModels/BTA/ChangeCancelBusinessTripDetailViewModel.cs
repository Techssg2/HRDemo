using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ChangeCancelBusinessTripDetailViewModel
    {
        public Guid Id { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public bool IsCancel { get; set; }
        public string Reason { get; set; }
        public string NewHotelCode { get; set; }
        public string NewHotelName { get; set; }
        public DateTimeOffset? NewCheckInHotelDate { get; set; }
        public DateTimeOffset? NewCheckOutHotelDate { get; set; }
        public string NewFlightNumberCode { get; set; }
        public string NewFlightNumberName { get; set; }
        public string NewAirlineCode { get; set; }
        public string NewAirlineName { get; set; }
        public string NewComebackFlightNumberCode { get; set; }
        public string NewComebackFlightNumberName { get; set; }
        public string NewComebackAirlineCode { get; set; }
        public string NewComebackAirlineName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public DateTimeOffset? NewFromDate { get; set; }
        public DateTimeOffset? NewToDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid BusinessTripApplicationId { get; set; }
        public Guid BusinessTripApplicationDetailId { get; set; }
        public Guid? UserId { get; set; }
        public Gender Gender { get; set; }
        //CR BTA ===============================================
        public string CancellationFeeObj { get; set; }
        public bool IsCancelInBoundFlight { get; set; }
        public bool IsCancelOutBoundFlight { get; set; }
        public string ReasonForInBoundFlight { get; set; }
        public string ReasonForOutBoundFlight { get; set; }
    }
}
