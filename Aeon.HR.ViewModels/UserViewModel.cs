using Aeon.HR.Infrastructure.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.ViewModels.BTA;

namespace Aeon.HR.ViewModels
{
    public class UserListViewModel
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActivated { get; set; }
        public UserRole Role { get; set; }
        public LoginType Type { get; set; }
        // Ngày nhân viên bắt đầu vào làm AEON 
        public DateTime? StartDate { get; set; }
        // Extra Rest Day (Nghỉ bù thứ bảy) còn lại trong năm
        public double ErdRemain { get; set; }
        public Guid? ProfilePictureId { get; set; }
        public string ProfilePicture { get; set; }
        public string LoginType { get; set; }
        public string ModulePermission { get; set; }
        public string UserDepartmentMappingsDepartmentCode { get; set; }
        public string UserDepartmentMappingsDepartmentName { get; set; }
        public Guid? UserDepartmentMappingsJobGradeId { get; set; }
        public string UserDepartmentMappingsJobGradeGrade { get; set; }
        public string UserDepartmentMappingsJobGradeCaption { get; set; }
        public string UserDepartmentMappingsJobGradeTitle { get; set; }
        public string RedundantPRD { get; set; }
        public Gender Gender { get; set; }
        public bool CheckAuthorizationUSB { get; set; }
        public bool IsTargetPlan { get; set; }
        public bool IsNotTargetPlan { get; set; }
        public string UserPositionCode { get; set; }
        public string UserPositionName { get; set; }
        public bool? HasTrackingLog { get; set; }
        public bool IsEdoc { get; set; }
    }
}