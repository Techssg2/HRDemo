using Newtonsoft.Json;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class PassengerFare
    {
        [JsonProperty("baseFare")]
        public BaseFare BaseFare { get; set; }

        [JsonProperty("equivFare")]
        public object EquivFare { get; set; }

        [JsonProperty("serviceTax")]
        public ServiceTax ServiceTax { get; set; }

        [JsonProperty("taxes")]
        public object Taxes { get; set; }

        [JsonProperty("totalFare")]
        public TotalFare TotalFare { get; set; }

        [JsonProperty("totalPaxFee")]
        public TotalPaxFee TotalPaxFee { get; set; }

        [JsonProperty("surcharges")]
        public List<Surcharge> Surcharges { get; set; }
    }
}
