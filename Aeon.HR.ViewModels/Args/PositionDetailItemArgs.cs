using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class PositionDetailItemArgs
    {
        public Guid Id { get; set; }

        public Guid ApplicantId { get; set; }
        public Guid ApplicantStatusId { get; set; }
        public Guid? AppreciationId { get; set; }

        public bool IsSignedOffer { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public string EmployeeGroupCode { get; set; }
        public string EmployeeSybGroupCode { get; set; }
        public string ReasonHiringCode { get; set; }
        public string AdditionalPositionCode { get; set; }
        public string AdditionalPositionName { get; set; }
        public bool HasCreateF2Form { get; set; }
    }
}