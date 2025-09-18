using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class FamilyMemberInCompany: Entity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string JobPosition { get; set; }
        [StringLength(50)]
        public string Relationship { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(50)]
        public string Department { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
