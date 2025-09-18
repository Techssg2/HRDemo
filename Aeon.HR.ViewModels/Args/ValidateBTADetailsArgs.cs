using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ValidateBTADetailsArgs
    {
        public List<BTADetailModels> BTADetails { get; set; }
    }
    public class BTADetailModels
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; }
        public bool CheckBookingCompleted { get; set; }
        public bool IsCommitBooking { get; set; }
    }
}
