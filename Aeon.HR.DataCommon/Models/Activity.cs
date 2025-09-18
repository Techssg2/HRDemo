using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class Activity: Entity
    {
      
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Responsibilty { get; set; }
        public int ApproximateDays { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
