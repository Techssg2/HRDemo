using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class AgencyMarkupInfo
    {
        [JsonProperty("agencyCode")]
        public string AgencyCode { get; set; }

        [JsonProperty("baseFare")]
        public double BaseFare { get; set; }

        [JsonProperty("equivFare")]
        public double EquivFare { get; set; }

        [JsonProperty("serviceTax")]
        public double ServiceTax { get; set; }

        [JsonProperty("totalFare")]
        public double TotalFare { get; set; }

        [JsonProperty("totalTax")]
        public double TotalTax { get; set; }

        [JsonProperty("markupValue")]
        public double MarkupValue { get; set; }

        [JsonProperty("agencyMarkupValue")]
        public double AgencyMarkupValue { get; set; }
    }
}
