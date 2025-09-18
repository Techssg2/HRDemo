using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ApplicantEducationViewModel
    {
        public ApplicantEducationViewModel()
        {
        }

        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public string SchoolName { get; set; }
        //Ngan
        public string SchoolCode { get; set; }
        //end
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string Major { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}