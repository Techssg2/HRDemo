using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class BusinessTripDetail_API_DTO
    {
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string DepartureName { get; set; }
        public string ArrivalName { get; set; }
    }
}
