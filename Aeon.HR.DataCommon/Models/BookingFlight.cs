using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class BookingFlight :  Entity
    {
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
        public virtual FlightDetail FlightDetail { get; set; }
        public virtual BusinessTripApplicationDetail BTADetail { get; set; }
    }
}