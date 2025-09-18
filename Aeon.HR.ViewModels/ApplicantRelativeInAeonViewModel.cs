using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ApplicantRelativeInAeonViewModel
    {
        public ApplicantRelativeInAeonViewModel()
        {
        }

        public virtual Guid Id { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid? DepartmentId { get; set; }

        public string FullName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string RelationCode { get; set; }
        public string RelationName { get; set; }
        public string WorkingPlacesCode { get; set; }
        public string WorkingPlacesName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}