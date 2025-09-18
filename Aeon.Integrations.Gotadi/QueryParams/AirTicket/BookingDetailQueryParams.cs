using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class BookingDetailQueryParams
    {
        [AliasAs("booking_number")]
        public string BookingNumber { get; set; }
    }
}
