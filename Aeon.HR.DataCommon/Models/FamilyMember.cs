using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class FamilyMember: Entity
    {        
        
        public string RelationShip { get; set; }
        public string RelationShipName { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public string PlaceOfOccupation { get; set; }
        public string ContactNumber { get; set; }
        public Guid ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}
