using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class FarerulesQueryParams
    {
        [AliasAs("language")]
        public string Language { get; set; }
    }
}
