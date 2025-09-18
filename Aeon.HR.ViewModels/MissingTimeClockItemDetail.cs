using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MissingTimeClockItemDetail
    {
        public DateTime Date { get; set; }
        public string ShiftCode { get; set; }
        public string ActualTime { get; set; }       
        public string Reason { get; set; }
        public string Other { get; set; }
        public string Previous { get; set; }
        public int TypeActualTime { get; set; }
    }
}
