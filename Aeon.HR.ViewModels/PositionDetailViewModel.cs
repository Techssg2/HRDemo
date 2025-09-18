using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels
{
    public class PositionDetailViewModel
    {
        public Guid Id { get; set; }

        public Guid PositionId { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid ApplicantAppreciationId { get; set; }
        public bool ApplicantIsApproved { get; set; }

        public Priority Priority { get; set; }

        public string PositionAssignToSAPCode { get; set; }
        public string PositionAssignToFullName { get; set; }

        public Guid? ApplicantStatusId { get; set; }
        public string ApplicantFullName { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantMobile { get; set; }
        public string ApplicantApplicantStatusName { get; set; }
        public string ApplicantAppreciationName { get; set; }
        public string SAPReviewStatus { get; set; }
        //public string ApplicantCreatedDate { get; set; }
    }
}