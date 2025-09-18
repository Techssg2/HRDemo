using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class CharacterReferee: Entity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(20)]
        public string TelNo { get; set; }
        [StringLength(50)]
        public string Profession { get; set; }
        public int YearsKnown { get; set; }
        [StringLength(50)]
        public string Relationship { get; set; }
        public string RelationshipName { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
