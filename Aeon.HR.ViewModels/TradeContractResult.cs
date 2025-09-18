using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TradeContractResult<T>
    {
        public TradeContractResult()
        {
            Results = new List<T>();
        }
        public bool IsSuccess { get; set; }
        public string ErrorCodes { get; set; }
        public string Messages { get; set; }
        public List<T> Results { get; set; }
    }
}
