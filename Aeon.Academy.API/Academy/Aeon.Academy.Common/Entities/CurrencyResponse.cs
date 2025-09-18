using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class CurrencyResponse
    {
        public Currency Currency { get; set; }
    }
    public class Currency
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Month { get; set; }
        public double AmountInVND { get; set; }
        public int? Year { get; set; }
        public int? Day { get; set; }
        public Boolean? IsDeleted { get; set; }
    }
}
