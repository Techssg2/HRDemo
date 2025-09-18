using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ValidateERDShiftExchangeViewModel
    {
        public string SAPCode { get; set; }
        public float QuotaERD { get; set; }
        public float SumNewShiftERD { get; set; }
    }
}
