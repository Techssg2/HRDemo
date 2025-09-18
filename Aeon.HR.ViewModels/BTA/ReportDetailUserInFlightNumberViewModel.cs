using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.BTA
{
    public class ReportDetailUserInFlightNumberViewModel : IReportDetailBTA
    {
        public Guid? Id { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public Guid? TrackId { get; set; }
    }
}
