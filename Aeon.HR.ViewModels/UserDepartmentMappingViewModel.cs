using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Enums;
namespace Aeon.HR.ViewModels
{
    public class UserDepartmentMappingViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public string UserEmail { get; set; }
        public Group Role { get; set; } 
        public bool IsHeadCount { get; set; }
        public string UserJobGradeCaption { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public int UserJobGradeGrade { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTimeOffset? OfficialResignationDate { get; set; }
        public string Note { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsPrepareDelete { get; set; }
        public bool IsDeleted { get; set; }
    }
}