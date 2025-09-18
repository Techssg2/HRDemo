using Aeon.HR.Data.Models;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class CommitBooking
    {
        public byte[] secret_key { get; set; }
        public string access_code { get; set; }
        public string booking_number { get; set; }
        public string partner_trans_id { get; set; }
        public string signatureData
        {
            get
            {
                return $"{access_code}|{booking_number}|{partner_trans_id}|AIR";
            }
        }
        //public string originalData { get; set; }
        public string key { get; set; }
        public string data { get; set; }
        public List<BusinessTripApplicationDetail> btaDetailItem { get; set; }
        public string booking_code { get; set; }
    }
}
