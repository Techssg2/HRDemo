using Refit;

namespace AEON.Integrations.Gotadi.QueryParams.Hotel
{
    public class SearchAllRatesQueryParams
    {
        [AliasAs("searchId")]
        public string SearchId { get; set; }

        [AliasAs("propertyId")]
        public string PropertyId { get; set; }

        [AliasAs("checkIn")]
        public string CheckIn { get; set; }

        [AliasAs("checkOut")]
        public string CheckOut { get; set; }

        [AliasAs("paxInfos")]
        public string PaxInfos { get; set; }

        [AliasAs("supplier")]
        public string Supplier { get; set; }
    }
}
