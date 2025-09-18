using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class BookingFlightArg
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
    }
}