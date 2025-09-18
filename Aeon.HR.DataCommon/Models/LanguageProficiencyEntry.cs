using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class LanguageProficiencyEntry: Entity
    {
        [StringLength(50)]
        public string Language { get; set; }
        [StringLength(50)]
        public string LanguageName { get; set; }
        [StringLength(20)]
        public string Spoken { get; set; }
        [StringLength(20)]
        public string Writen { get; set; }
        [StringLength(20)]
        public string Understand { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
