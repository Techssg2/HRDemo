using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class EmergencyContactViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicantId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}