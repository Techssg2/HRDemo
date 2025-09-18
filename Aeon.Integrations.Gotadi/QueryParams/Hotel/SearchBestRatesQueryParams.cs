using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.QueryParams.Hotel
{
    public class SearchBestRatesQueryParams
    {
        [AliasAs("searchCode")]
        public string SearchCode { get; set; }

        [AliasAs("searchType")]
        public string SearchType { get; set; }

        [AliasAs("language")]
        public string Language { get; set; }

        [AliasAs("currency")]
        public string Currency { get; set; }

        [AliasAs("checkIn")]
        public string CheckIn { get; set; }

        [AliasAs("checkOut")]
        public string CheckOut { get; set; }

        [AliasAs("paxInfos")]
        public string PaxInfos { get; set; }

        [AliasAs("supplier")]
        public string Supplier { get; set; }

        [AliasAs("pageNumber")]
        public int PageNumber { get; set; }

        [AliasAs("pageSize")]
        public int PageSize { get; set; }
    }
}
