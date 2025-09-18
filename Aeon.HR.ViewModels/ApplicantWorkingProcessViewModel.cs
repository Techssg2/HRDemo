using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ApplicantWorkingProcessViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Salary { get; set; }
        public string LeaveReason { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}