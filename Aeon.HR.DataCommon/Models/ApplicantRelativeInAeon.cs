using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class ApplicantRelativeInAeon : Entity
    {
        public Guid ApplicantId { get; set; }
        public Guid? DepartmentId { get; set; }

        public string FullName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string RelationCode { get; set; }
        public string RelationName { get; set; }
        public string WorkingPlacesCode {get;set;}
        public string WorkingPlacesName {get;set;}

        public virtual Applicant Applicant { get; set; }
        public virtual Department Department { get; set; }
        // Link To Department
        // Link To Position

    }
}