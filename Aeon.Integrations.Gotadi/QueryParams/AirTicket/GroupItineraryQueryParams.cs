using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.AirTicket
{
    public class GroupItineraryQueryParams
    {
        [AliasAs("groupId")]
        public int GroupId { get; set; }
        [AliasAs("page")]
        public int Page { get; set; }

        [AliasAs("size")]
        public int Size { get; set; }

        [AliasAs("sort")]
        public string Sort { get; set; }
    }
}
