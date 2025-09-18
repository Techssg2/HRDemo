using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class NewStaffOnBoardViewModel
    {
        public Guid Id { get; set; }
        public string ApplicantReferenceNumber { get; set; }
        public string ApplicantFullName { get; set; }
        public string ApplicantIDCard9Number { get; set; }
        public string ApplicantIDCard12Number { get; set; }
        public DateTimeOffset ApplicantDateOfBirth { get; set; }
        public string PositionName { get; set; }
        public string positionDeptDivisionName { get; set; }
        public string ApplicantStatus { get; set; }
        public bool ApplicantIsApproved { get; set; }
    }
}