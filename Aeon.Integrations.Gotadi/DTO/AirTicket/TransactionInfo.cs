using Newtonsoft.Json;
using System;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class TransactionInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("saleChannel")]
        public string SaleChannel { get; set; }

        [JsonProperty("channelType")]
        public string ChannelType { get; set; }

        [JsonProperty("supplierType")]
        public string SupplierType { get; set; }

        [JsonProperty("bookingCode")]
        public string BookingCode { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("bookingDate")]
        public DateTime BookingDate { get; set; }

        [JsonProperty("supplierCode")]
        public string SupplierCode { get; set; }

        [JsonProperty("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("bookingRefNo")]
        public object BookingRefNo { get; set; }

        [JsonProperty("passengerNameRecord")]
        public object PassengerNameRecord { get; set; }

        [JsonProperty("timeToLive")]
        public object TimeToLive { get; set; }

        [JsonProperty("signature")]
        public object Signature { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("originLocationCode")]
        public string OriginLocationCode { get; set; }

        [JsonProperty("destinationLocationCode")]
        public string DestinationLocationCode { get; set; }

        [JsonProperty("carrierNo")]
        public string CarrierNo { get; set; }

        [JsonProperty("checkIn")]
        public DateTime CheckIn { get; set; }

        [JsonProperty("checkOut")]
        public DateTime CheckOut { get; set; }

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
        public object MarkupValue { get; set; }

        [JsonProperty("totalSsrValue")]
        public double TotalSsrValue { get; set; }

        [JsonProperty("markupKey")]
        public string MarkupKey { get; set; }

        [JsonProperty("markupCode")]
        public object MarkupCode { get; set; }

        [JsonProperty("markupFormula")]
        public object MarkupFormula { get; set; }

        [JsonProperty("paymentAmount")]
        public object PaymentAmount { get; set; }

        [JsonProperty("issuedStatus")]
        public string IssuedStatus { get; set; }

        [JsonProperty("issuedDate")]
        public object IssuedDate { get; set; }

        [JsonProperty("etickets")]
        public object Etickets { get; set; }

        [JsonProperty("listETickets")]
        public object ListETickets { get; set; }

        [JsonProperty("productSeqNumber")]
        public string ProductSeqNumber { get; set; }

        [JsonProperty("productClass")]
        public string ProductClass { get; set; }

        [JsonProperty("bookingDirection")]
        public string BookingDirection { get; set; }

        [JsonProperty("noAdult")]
        public int NoAdult { get; set; }

        [JsonProperty("noChild")]
        public int NoChild { get; set; }

        [JsonProperty("noInfant")]
        public int NoInfant { get; set; }

        [JsonProperty("supplierBookingStatus")]
        public string SupplierBookingStatus { get; set; }

        [JsonProperty("supplierPaymentStatus")]
        public object SupplierPaymentStatus { get; set; }

        [JsonProperty("allowHold")]
        public bool AllowHold { get; set; }

        [JsonProperty("refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("onlyPayLater")]
        public bool OnlyPayLater { get; set; }
    }
}
