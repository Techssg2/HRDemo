using AEON.Integrations.Gotadi.DTO.Hotel;
using AEON.Integrations.Gotadi.QueryParams;
using AEON.Integrations.Gotadi.QueryParams.Hotel;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.API
{
    public interface IHotelApi
    {
        [Get("/api/v3/hotel/search-keyword")]
        Task<SearchKeyWordResponse> SearchKeyWord(SearchKeyWordQueryParams queryParams);

        [Get("/api/v3/hotel/search-best-rates")]
        Task<object> SearchBestRates(SearchBestRatesQueryParams queryParams);

        [Get("/api/v3/hotel/filter-options")]
        Task<GetFilterOptionsResponse> GetFilterOptions(GetFilterOptionsQueryParams queryParams);

        [Get("/api/v3/hotel/search-all-rates")]
        Task<object> SearchAllRates(SearchAllRatesQueryParams queryParams);
    }
}
