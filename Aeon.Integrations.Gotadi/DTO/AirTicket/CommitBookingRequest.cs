using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
	public class CommitBookingRequest
	{
		[JsonProperty("data")]
		public string data { get; set; }

		[JsonProperty("key")]
		public string key { get; set; }
	}
}
