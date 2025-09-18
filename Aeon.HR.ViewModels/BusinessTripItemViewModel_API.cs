using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class BusinessTripItemViewModel_API
    {
        public string ReferenceNumber { get; set; }
        public BTAType Type { get; set; }
        public IEnumerable<BusinessTripDetail_API_DTO> BusinessTripDetails { get; set; }
        //public IEnumerable<ChangeCancelBusinessTripDTO> ChangeCancelBusinessTripDetails { get; set; }
        public string UrlBTA { get; set; }
    }
}
