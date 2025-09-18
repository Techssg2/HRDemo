using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class SearchAirPortParams
    {
        public string query { get; set; }
        public string country { get; set; }
        public string page { get; set; }
        public string size { get; set; }
        public string sort { get; set; }
    }
}
