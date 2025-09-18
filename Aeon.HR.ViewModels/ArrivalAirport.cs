using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
	public class ArrivalAirport
	{
		public string Airport { get; set; }
		public string ScheduledTime { get; set; }
		public UTCOffset UtcOffset { get; set; }
	}

	public class UTCOffset
	{
		public string hours { get; set; }
		public string minutes { get; set; }
	}
}
