using System;
using System.Collections.Generic;
using Aeon.HR.Infrastructure.Enums;

namespace SyncOrgchartJob.Models.eDocHR
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
        public bool IsActivated { get; set; }
        public bool IsDeleted { get; set; }
        public LoginType Type { get; set; }
        public UserRole Role { get; set; }
        public Gender Gender { get; set; }
        public bool CheckAuthorizationUSB { get; set; }
        public bool IsTargetPlan { get; set; }
        public bool IsNotTargetPlan { get; set; }
        public DateTime? StartDate { get; set; }
        public string QuotaDataJson { get; set; }
        public string RedundantPRD { get; set; } // PRD còn dư của tháng trước tháng hiện tại
        public bool? HasTrackingLog { get; set; }
        public bool IsFromIT { get; set; }
        public int CountLogin { get; set; }
        public bool IsEdoc { get; set; }
        public NewDataUser NewData { get; set; }
    }
    public class NewDataUser
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string LoginName { get; set; }
        public string SAPCode { get; set; }
        public string Email { get; set; }
        public bool IsActivated { get; set; }
        public LoginType Type { get; set; }
        public UserRole Role { get; set; }
        public DateTime? StartDate { get; set; }
        public bool Status { get; set; }
        public List<string> ErrorList { get; set; } = new List<string>();
    }
}