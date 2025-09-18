using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class CurrentShiftArg
    {
        public DateTimeOffset ShiftExchangeDate { get; set; }
        public string SAPCode { get; set; }
        public Guid? DeptId { get; set; }
        public Guid? DivisionId { get; set; }
    }
}
