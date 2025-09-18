using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class EmploymentHistory: Entity
    {
        //public string Date { get; set; }
        //public DateTimeOffset? FromDate { get; set; }
        //public DateTimeOffset? ToDate { get; set; }
        public DateTimeOffset? Date { get; set; }
        [StringLength(200)]
        public string NameOfEmployer { get; set; }

        [StringLength(200)]
        public string JobPosition { get; set; }

        public int GlossSalary { get; set; }
        [StringLength(500)]
        public string ReasonsForLeaving { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
