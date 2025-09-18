using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.Hotel
{
    public class SearchKeyWordQueryParams
    {
        [AliasAs("keyword")]
        public string Keyword { get; set; }

        [AliasAs("language")]
        public string Language { get; set; }

        [AliasAs("pageNumber")]
        public int PageNumber { get; set; }

        [AliasAs("pageSize")]
        public int PageSize { get; set; }
    }
}
