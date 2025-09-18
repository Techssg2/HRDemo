using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class LeaveKindQuotaMapping
    {
        public List<string> LeaveKinds { get; set; }
        public List<string> QuotaKinds { get; set; }
        public string Key { get; set; }
    }
}
