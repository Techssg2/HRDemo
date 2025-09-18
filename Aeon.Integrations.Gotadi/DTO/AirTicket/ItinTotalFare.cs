using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class ItinTotalFare
    {
        [JsonProperty("baseFare")]
        public BaseFare BaseFare { get; set; }

        [JsonProperty("equivFare")]
        public EquivFare EquivFare { get; set; }

        [JsonProperty("serviceTax")]
        public ServiceTax ServiceTax { get; set; }

        [JsonProperty("totalFare")]
        public TotalFare TotalFare { get; set; }

        [JsonProperty("totalTax")]
        public TotalTax TotalTax { get; set; }

        [JsonProperty("totalPaxFee")]
        public TotalPaxFee TotalPaxFee { get; set; }
    }
}
