using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class UserDataForCreatingArgs
    {
        public UserDataForCreatingArgs()
        {
            Permissions = new string[] { };
        }
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public UserRole Role { get; set; }
        public LoginType Type { get; set; }
        // Ngày nhân viên bắt đầu vào làm AEON 
        public DateTime? StartDate { get; set; }
        // Extra Rest Day (Nghỉ bù thứ bảy) còn lại trong năm
        public double ErdRemain { get; set; }
        public string[] Permissions { get; set; }
        //Profile
        public Guid? ProfilePictureId { get; set; }
        public Gender Gender { get; set; }
        public bool IsActivated { get; set; }
        public bool CheckAuthorizationUSB { get; set; }
        public bool IsTargetPlan { get; set; }
        public bool IsNotTargetPlan { get; set; }
        public bool IsEdoc { get; set; }
    }
}