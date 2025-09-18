using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class EducationViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public string Level { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Certificate { get; set; }
        public string SchoolName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}