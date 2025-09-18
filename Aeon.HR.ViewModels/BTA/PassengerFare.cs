using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class PassengerFare
    {
        public PriceInfo baseFare { get; set; }
        public object equivFare { get; set; }
        public PriceInfo serviceTax { get; set; }
        public object taxes { get; set; }
        public PriceInfo totalFare { get; set; }
        public object totalPaxFee { get; set; }
        public List<Surcharge> surcharges { get; set; }
    }
}
