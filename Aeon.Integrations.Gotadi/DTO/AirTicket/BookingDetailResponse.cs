using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BookingDetailResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("cacheType")]
        public string CacheType { get; set; }

        [JsonProperty("orgCode")]
        public string OrgCode { get; set; }

        [JsonProperty("agencyCode")]
        public string AgencyCode { get; set; }

        [JsonProperty("branchCode")]
        public object BranchCode { get; set; }

        [JsonProperty("saleChannel")]
        public string SaleChannel { get; set; }

        [JsonProperty("channelType")]
        public string ChannelType { get; set; }

        [JsonProperty("supplierType")]
        public string SupplierType { get; set; }

        [JsonProperty("bookingCode")]
        public string BookingCode { get; set; }

        [JsonProperty("bookingType")]
        public string BookingType { get; set; }

        [JsonProperty("agentCode")]
        public string AgentCode { get; set; }

        [JsonProperty("customerCode")]
        public string CustomerCode { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("bookingDate")]
        public DateTime BookingDate { get; set; }

        [JsonProperty("markupType")]
        public string MarkupType { get; set; }

        [JsonProperty("bookingInfo")]
        public BookingInfo BookingInfo { get; set; }

        [JsonProperty("groupPricedItineraries")]
        public List<GroupPricedItinerary> GroupPricedItineraries { get; set; }

        [JsonProperty("hotelAvailability")]
        public object HotelAvailability { get; set; }

        [JsonProperty("hotelProductPayload")]
        public object HotelProductPayload { get; set; }

        [JsonProperty("hotelProduct")]
        public object HotelProduct { get; set; }

        [JsonProperty("offlineBooking")]
        public object OfflineBooking { get; set; }

        [JsonProperty("travelerInfo")]
        public object TravelerInfo { get; set; }

        [JsonProperty("isPerBookingType")]
        public bool IsPerBookingType { get; set; }
    }
}
