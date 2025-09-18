using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AddBookingTravellerResquest
    {
        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("bookingContacts")]
        public List<BookingContact> BookingContacts { get; set; }

        [JsonProperty("bookingTravelerInfos")]
        public List<BookingTravelerInfo> BookingTravelerInfos { get; set; }

        [JsonProperty("osiCodes")]
        public List<OsiCodesRequestInfo> OsiCodes { get; set; }

        [JsonProperty("taxReceiptRequest")]
        public TaxReceiptRequestInfo TaxReceiptRequest { get; set; }
    }
}
