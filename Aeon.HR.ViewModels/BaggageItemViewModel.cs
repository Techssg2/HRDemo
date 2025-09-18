using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class BaggageItemViewModel
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public string Direction { get; set; }
        public string FareCode { get; set; }
        public string Name { get; set; }
        public string ServiceType { get; set; }
    }
}
