using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class FlightNumberViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid AirlineId { get; set; }
        public string AirlineName { get; set; }
        public string AirlineCode { get; set; }
        public bool IsPeakTime { get; set; }
        public string DepartureTime { get; set; }
    }
}
