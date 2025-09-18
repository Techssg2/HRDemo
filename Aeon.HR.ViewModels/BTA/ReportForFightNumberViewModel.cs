using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportForFightNumberViewModel: IReportBTA
    {
        public string FlightNumberCode { get; set; }
        public string FlightNumber { get; set; }
        public int TotalStaffs { get; set; }
    }
}
