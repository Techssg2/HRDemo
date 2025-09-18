using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class CurrencyModel
    {
        public string Symbol { get; set; }
    }
    public class BudgetBallanceModel
    {
        public int Year { get; set; }
        public string DepartmentInChargeCode { get; set; }
        public string CFRCode { get; set; }
        public string BudgetCodeCode { get; set; }
        public string BudgetPlan { get; set; }
    }
}
