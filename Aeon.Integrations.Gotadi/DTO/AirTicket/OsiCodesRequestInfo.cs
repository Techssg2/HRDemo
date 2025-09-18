using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
	public class OsiCodesRequestInfo
	{
		[JsonProperty("direction")]
		public string Direction { get; set; }

		[JsonProperty("osiCode")]
		public string OsiCode { get; set; }
		[JsonProperty("tourCode")]
		public string TourCode { get; set; }
	}
}
