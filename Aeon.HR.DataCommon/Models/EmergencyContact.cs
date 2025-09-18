using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class EmergencyContact: IEntity
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual Applicant Applicant { get; set; }
    }
}
