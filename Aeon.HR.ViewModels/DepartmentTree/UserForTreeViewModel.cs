using Aeon.HR.Infrastructure.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class UserForTreeViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public Guid? JobGradeId { get; set; }
        public string JobGrade { get; set; }
        public int? JobGradeValue { get; set; }
        public string JobGradeTitle { get; set; }
        public string Position { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string PeriodName { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsStore { get; set; }
        public bool? IsNotTargetPlan { get; set; }
        public bool? IsTargetPlan { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? OfficialResignationDate { get; set; }
        public double? ERDRemain { get; set; }
        public double? ALRemain { get; set; }
        public double? DOFLRemain { get; set; }

        #region DWS
        public bool IsHasTargetPlan { get; set; } = false;
        #endregion
    }
}