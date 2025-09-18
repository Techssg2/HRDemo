using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AEON.Integrations.Gotadi.DTO.AirTicket
{
    public class BookingInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("orgCode")]
        public string OrgCode { get; set; }

        [JsonProperty("agencyCode")]
        public string AgencyCode { get; set; }

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

        [JsonProperty("agentId")]
        public int AgentId { get; set; }

        [JsonProperty("agentName")]
        public string AgentName { get; set; }

        [JsonProperty("branchCode")]
        public object BranchCode { get; set; }

        [JsonProperty("customerCode")]
        public string CustomerCode { get; set; }

        [JsonProperty("customerId")]
        public object CustomerId { get; set; }

        [JsonProperty("bookingNumber")]
        public string BookingNumber { get; set; }

        [JsonProperty("roundType")]
        public string RoundType { get; set; }

        [JsonProperty("fromLocationCode")]
        public string FromLocationCode { get; set; }

        [JsonProperty("fromLocationName")]
        public string FromLocationName { get; set; }

        [JsonProperty("fromCity")]
        public string FromCity { get; set; }

        [JsonProperty("toLocationCode")]
        public string ToLocationCode { get; set; }

        [JsonProperty("toLocationName")]
        public string ToLocationName { get; set; }

        [JsonProperty("toCity")]
        public string ToCity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("bookingDate")]
        public DateTime BookingDate { get; set; }

        [JsonProperty("departureDate")]
        public DateTime DepartureDate { get; set; }

        [JsonProperty("returnDate")]
        public object ReturnDate { get; set; }

        [JsonProperty("baseFare")]
        public double BaseFare { get; set; }

        [JsonProperty("equivFare")]
        public double EquivFare { get; set; }

        [JsonProperty("serviceTax")]
        public double ServiceTax { get; set; }

        [JsonProperty("vat")]
        public object Vat { get; set; }

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

        [JsonProperty("paymentTotalAmount")]
        public double PaymentTotalAmount { get; set; }

        [JsonProperty("paymentFee")]
        public double PaymentFee { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }

        [JsonProperty("paymentDate")]
        public object PaymentDate { get; set; }

        [JsonProperty("paymentRefNumber")]
        public object PaymentRefNumber { get; set; }

        [JsonProperty("issuedStatus")]
        public string IssuedStatus { get; set; }

        [JsonProperty("issuedDate")]
        public object IssuedDate { get; set; }

        [JsonProperty("customerFirstName")]
        public object CustomerFirstName { get; set; }

        [JsonProperty("customerLastName")]
        public object CustomerLastName { get; set; }

        [JsonProperty("customerPhoneNumber1")]
        public object CustomerPhoneNumber1 { get; set; }

        [JsonProperty("customerPhoneNumber2")]
        public object CustomerPhoneNumber2 { get; set; }

        [JsonProperty("customerEmail")]
        public object CustomerEmail { get; set; }

        [JsonProperty("taxReceiptRequest")]
        public object TaxReceiptRequest { get; set; }

        [JsonProperty("taxCompanyName")]
        public object TaxCompanyName { get; set; }

        [JsonProperty("taxAddress1")]
        public object TaxAddress1 { get; set; }

        [JsonProperty("taxAddress2")]
        public object TaxAddress2 { get; set; }

        [JsonProperty("taxNumber")]
        public object TaxNumber { get; set; }

        [JsonProperty("paymentBy")]
        public object PaymentBy { get; set; }

        [JsonProperty("paymentByCode")]
        public object PaymentByCode { get; set; }

        [JsonProperty("issuedByCode")]
        public object IssuedByCode { get; set; }

        [JsonProperty("refundBy")]
        public object RefundBy { get; set; }

        [JsonProperty("refundByCode")]
        public object RefundByCode { get; set; }

        [JsonProperty("bookBy")]
        public string BookBy { get; set; }

        [JsonProperty("bookByCode")]
        public string BookByCode { get; set; }

        [JsonProperty("displayPriceInfo")]
        public DisplayPriceInfo DisplayPriceInfo { get; set; }

        [JsonProperty("transactionInfos")]
        public List<TransactionInfo> TransactionInfos { get; set; }

        [JsonProperty("agencyMarkupInfos")]
        public List<AgencyMarkupInfo> AgencyMarkupInfos { get; set; }

        [JsonProperty("contactInfos")]
        public List<object> ContactInfos { get; set; }

        [JsonProperty("travelerInfos")]
        public List<object> TravelerInfos { get; set; }

        [JsonProperty("timeToLive")]
        public object TimeToLive { get; set; }

        [JsonProperty("supplierBookingStatus")]
        public string SupplierBookingStatus { get; set; }

        [JsonProperty("passengerNameRecords")]
        public string PassengerNameRecords { get; set; }

        [JsonProperty("etickets")]
        public string Etickets { get; set; }

        [JsonProperty("cancellationStatus")]
        public object CancellationStatus { get; set; }

        [JsonProperty("cancellationFee")]
        public double CancellationFee { get; set; }

        [JsonProperty("cancellationNotes")]
        public object CancellationNotes { get; set; }

        [JsonProperty("cancellationBy")]
        public object CancellationBy { get; set; }

        [JsonProperty("cancellationDate")]
        public object CancellationDate { get; set; }

        [JsonProperty("discountAmount")]
        public double DiscountAmount { get; set; }

        [JsonProperty("discountVoucherCode")]
        public object DiscountVoucherCode { get; set; }

        [JsonProperty("discountVoucherName")]
        public object DiscountVoucherName { get; set; }

        [JsonProperty("discountRedeemId")]
        public object DiscountRedeemId { get; set; }

        [JsonProperty("discountRedeemCode")]
        public object DiscountRedeemCode { get; set; }

        [JsonProperty("discountDate")]
        public object DiscountDate { get; set; }

        [JsonProperty("additionalFee")]
        public object AdditionalFee { get; set; }

        [JsonProperty("taxPersonalInfoContact")]
        public object TaxPersonalInfoContact { get; set; }

        [JsonProperty("bookingNote")]
        public object BookingNote { get; set; }

        [JsonProperty("internalBookingNote")]
        public object InternalBookingNote { get; set; }

        [JsonProperty("promotionID")]
        public object PromotionID { get; set; }

        [JsonProperty("reasonCodePaymentFailed")]
        public object ReasonCodePaymentFailed { get; set; }

        [JsonProperty("bookingFinalStatus")]
        public object BookingFinalStatus { get; set; }

        [JsonProperty("bookingIssuedType")]
        public object BookingIssuedType { get; set; }

        [JsonProperty("ownerBooking")]
        public bool OwnerBooking { get; set; }

        [JsonProperty("deleted")]
        public object Deleted { get; set; }

        [JsonProperty("allowHold")]
        public bool AllowHold { get; set; }

        [JsonProperty("refundable")]
        public bool Refundable { get; set; }

        [JsonProperty("onlyPayLater")]
        public bool OnlyPayLater { get; set; }

        [JsonProperty("showPayLaterOption")]
        public bool ShowPayLaterOption { get; set; }

        [JsonProperty("showPayNowOption")]
        public bool ShowPayNowOption { get; set; }
    }
}
