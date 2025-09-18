using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportForHotelViewModel: IReportBTA
    {
        public ReportForHotelViewModel()
        {
            Items = new List<ReportForHotelDetailViewModel>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string TelePhone { get; set; }
        public double TotalRooms { get; set; }
        public double TotalDays { get; set; }
        public List<ReportForHotelDetailViewModel> Items { get; set; }

    }
}
