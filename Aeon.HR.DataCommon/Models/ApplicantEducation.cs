using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class ApplicantEducation: Entity
    {
        public Guid ApplicantId { get; set; }

        public string SchoolName { get; set; }
        //Ngan
        public string SchoolCode { get; set; }
        //end
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string Major { get; set; }

        public virtual Applicant Applicant { get; set; }
    }
}