using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
	public class DepartureAirport
	{
		public string Airport { get; set; }
		public string ScheduledTime { get; set; }
		public UTCOffset UtcOffset { get; set; }
	}
}
