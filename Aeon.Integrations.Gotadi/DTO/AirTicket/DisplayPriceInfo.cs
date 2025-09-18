using Newtonsoft.Json;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class DisplayPriceInfo
    {
        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

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

        [JsonProperty("agencyMarkupValue")]
        public double AgencyMarkupValue { get; set; }

        [JsonProperty("markupValue")]
        public double MarkupValue { get; set; }

        [JsonProperty("totalSsrValue")]
        public double TotalSsrValue { get; set; }

        [JsonProperty("cancellationFee")]
        public double CancellationFee { get; set; }

        [JsonProperty("paymentFee")]
        public double PaymentFee { get; set; }

        [JsonProperty("discountAmount")]
        public double DiscountAmount { get; set; }

        [JsonProperty("additionalFee")]
        public double AdditionalFee { get; set; }

        [JsonProperty("additionalTaxPerTraveler")]
        public double AdditionalTaxPerTraveler { get; set; }

        [JsonProperty("vat")]
        public object Vat { get; set; }
    }
}
