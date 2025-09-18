using Newtonsoft.Json;
using System;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
	public class ArrivalAirport
	{
		[JsonProperty("airport")]
		public string Airport { get; set; }

		[JsonProperty("scheduledTime")]
		public string ScheduledTime { get; set; }

		[JsonProperty("utcOffset")]
		public UTCOffset UtcOffset { get; set; }
	}

	public class UTCOffset {
		[JsonProperty("hours")]
		public string hours { get; set; }

		[JsonProperty("minutes")]
		public string minutes { get; set; }
	}
}