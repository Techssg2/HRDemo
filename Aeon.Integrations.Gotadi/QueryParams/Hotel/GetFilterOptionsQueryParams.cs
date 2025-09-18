using Refit;

namespace AEON.Integrations.Gotadi.QueryParams.Hotel
{
    public class GetFilterOptionsQueryParams
    {
        [AliasAs("language")]
        public string Language { get; set; }
    }
}
