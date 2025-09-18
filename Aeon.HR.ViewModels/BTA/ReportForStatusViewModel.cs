using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportForStatusViewModel: IReportBTA
    {
        public string Status { get; set; }
        public int TotalRequest { get; set; }
    }
}
