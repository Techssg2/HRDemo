namespace Aeon.HR.ViewModels
{
    public class ItinTotalFareViewModel
    {
        public PriceInfo BaseFare { get; set; }
        public object EquivFare { get; set; }
        public PriceInfo ServiceTax { get; set; }
        public PriceInfo TotalFare { get; set; }
        public PriceInfo TotalTax { get; set; }
        public object TotalPaxFee { get; set; }
    }
}
