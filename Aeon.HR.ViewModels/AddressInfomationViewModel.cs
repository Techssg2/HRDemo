using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class AddressInfomationViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public string AddressType { get; set; }
        public string Address { get; set; }
        public string WardCode { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string CountryCode { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}