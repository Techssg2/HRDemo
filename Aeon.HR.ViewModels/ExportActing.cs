using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class ExportActing
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string SapCode { get; set; }
        public string FullName { get; set; }
        public string DeptLine { get; set; }
        public string Division { get; set; }
        public string WorkLocation { get; set; }
        public string PositionInActingPeriod { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByFullName { get; set; }
    }

    public class ActingExportViewModel
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserSAPCode { get; set; }
        public string FullName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string TitleInActingPeriodName { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public DateTimeOffset Created { get; set; }

        public string CreatedByFullName { get; set; }
    }
}
