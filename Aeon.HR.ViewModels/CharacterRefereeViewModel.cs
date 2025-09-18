using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class CharacterRefereeViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }      
        public string TelNo { get; set; }       
        public string Profession { get; set; }
        public int YearsKnown { get; set; }     
        public string Relationship { get; set; }
        public string RelationshipName { get; set; }
        public Guid? ApplicantId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
