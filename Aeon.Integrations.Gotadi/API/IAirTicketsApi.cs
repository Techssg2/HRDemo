using Aeon.HR.ViewModels;
using AEON.Integrations.Gotadi.DTO.AirTicket;
using AEON.Integrations.Gotadi.QueryParams.AirTicket;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.API
{
    public interface IAirTicketsApi
    {
        /// <summary>
        /// Tìm chuyến bay nội địa
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/api/air-tickets/low-fare-search-async")]
        Task<SearchResultResponse> LowFareSearchAsync([Query] SearchQueryParams queryParams);

        /// <summary>
        /// Tìm chuyến bay quốc tế
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/api/air-tickets/low-fare-search-internationalasync")]
        Task<SearchResultResponse> LowFareSearchInternationalAsync([Query] SearchQueryParams queryParams);

        /// <summary>
        /// Lấy danh sách các giá trị có thể áp dụng trên bộ lọc kết quả tìm kiếm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/filter-options")]
        Task<FilterOptionsResponse> FilterOptions([Body] FilterOptionsRequest request);

        /// <summary>
        /// Lọc và sắp xếp kết quả tìm kiếm chuyến bay
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/filter-availability")]
        Task<FilterAvailabilityResponse> FilterAvailability([Query] FilterAvailabilityQueryParams queryParams, [Body] FilterAvailabilityRequest request);

        /// <summary>
        /// Kiểm tra tình trạng vé còn hữu dụng trước khi đi booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/revalidate")]
        Task<RevalidateResponse> Revalidate([Body] ItineraryInfo request);

        /// <summary>
        /// Khởi tạo thông tin booking
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/draft-booking")]
        Task<DraftBookingResponse> DraftBooking([Body] DraftBookingRequest request);

        /// <summary>
        /// Lấy thông tin chi tiết của booking
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/api/products/booking-detail")]
        Task<BookingDetailResponse> GetBookingDetail([Query] BookingDetailQueryParams queryParams);

        /// <summary>
        /// Lấy thông tin tiện ích được mua kèm
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/api/air-tickets/ssr-offer/{bookingNumber}")]
        Task<SsrOfferResponse> GetSsrOffer(string bookingNumber);

        /// <summary>
        /// Cập nhật các thông tin: Hành khách, người liên hệ, thông tin xuất hóa đơn, … và yêu cầu giữ chỗ
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/add-booking-traveller")]
        Task<AddBookingTravellerResponse> AddBookingTraveller([Body] AddBookingTravellerResquest request);

        /// <summary>
        /// Lấy điều kiện vé của chuyến bay
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/farerules")]
        Task<FarerulesResponse> Farerules([Query] FarerulesQueryParams queryParams, [Body] FareRulesRequest request);

        /// <summary>
        /// Lấy thông tin danh sách mã sân bay
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/metasrv/api/_search/airports")]
        Task<List<AirportInfo>> SearchAirports([Query] SearchAirPortParams queryParams);

        /// <summary>
        /// Thanh toan sau khi booking
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Post("/api/partner/commit")]
        Task<object> CommitBooking([Body] CommitBookingRequest request);
        /// <summary>
        /// Lay chuyen quoc te tra ve
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Post("/api/air-tickets/group-itinerary/{groupId}")]
        Task<GroupItineraryResponse> GroupIntinerary(string groupId, [Query] GroupItineraryQueryParams queryParams, [Body] GroupIntineraryRequest request);

        /// <summary>
        /// Lấy thông tin danh sách quốc gia
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [Get("/metasrv/api/country-codes")]
        Task<List<CountryInfo>> SearchCountryCodes([Query] SearchCountryParams queryParams);
    }
}
