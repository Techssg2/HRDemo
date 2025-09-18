using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;

namespace Aeon.HR.Data.Models
{
    public class FlightNumber: SoftDeleteEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid AirlineId { get; set; }
        public bool IsPeakTime { get; set; }
        public virtual Airline Airline { get; set; }
        public string DepartureTime { get; set; }
    }
}
