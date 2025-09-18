using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class FilterAvailabilityQueryParams
    {
        [AliasAs("include-equivfare")]
        public bool IncludeEquivfare { get; set; }

        [AliasAs("page")]
        public int Page { get; set; }

        [AliasAs("size")]
        public int Size { get; set; }

        [AliasAs("sort")]
        public string Sort { get; set; }
    }
}
