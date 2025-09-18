using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class AirlineCodeInfo
    {
        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("airlineCode")]
        public string AirlineCode { get; set; }
        [JsonProperty("tripGroup")]
        public string TripGroup { get; set; }
    }
}
