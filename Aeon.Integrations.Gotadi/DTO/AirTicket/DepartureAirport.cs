using Newtonsoft.Json;
using System;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
	public class DepartureAirport
	{
		[JsonProperty("airport")]
		public string Airport { get; set; }

		[JsonProperty("scheduledTime")]
		public string ScheduledTime { get; set; }

		[JsonProperty("utcOffset")]
		public UTCOffset UtcOffset { get; set; }
	}
}