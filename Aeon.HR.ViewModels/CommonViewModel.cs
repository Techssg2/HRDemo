using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.ExternalItem;
using System;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels
{
    public class CommonViewModel
    {
        public class BusinessTripApplication
        {
            public class FareRuleViewModel
            {
                public string Title { get; set; }
                public string Detail { get; set; }
            }
        }
        public class PayloadTrackingRequest
        {
            public string RefNum { get; set; }
            public string Pernr { get; set; }
            public string Zyear { get; set; }
            public string Period { get; set; }
            public string EdocUser { get; set; }
            public List<DateValueItem> TargetData01Set { get; set; }
            public List<DateValueItem> TargetData02Set { get; set; }
        }

        public class TrackingHistoricalViewModel
        {
            public class Workflow
            {
                public string AssignFrom { get; set; }
                public string AssignFromName { get; set; }
                public string AssignFromGroup { get; set; }
                public string AssignTo { get; set; }
                public string AssignToName { get; set; }
                public string AssignToGroup { get; set; }
                public string Type { get; set; }
                public string StepNumber { get; set; }
                public Guid? InstanceId { get; set; }
                public string Comment { get; set; }
                public string Documents { get; set; }
            }
        }

        public class LogHistories
        {
            public class UserLogViewModel
            {
                public Guid Id { get; set; }
                public string LoginName { get; set; }
                public string SAPCode { get; set; }
                public string FullName { get; set; }
                public string Email { get; set; }
                public bool IsActivated { get; set; }
                public UserRole Role { get; set; }
                public LoginType Type { get; set; }
                public DateTime? StartDate { get; set; }
                public double ErdRemain { get; set; }
                public Guid? ProfilePictureId { get; set; }
                public string ProfilePicture { get; set; }
                public bool IsNotTargetPlan { get; set; }
                public DateTimeOffset? Created { get; set; }
                public DateTimeOffset? Modified { get; set; }
            }

            public class UserDepartmentMappingViewModel
            {
                public Guid Id { get; set; }
                public Guid? DepartmentId { get; set; }
                public Guid? UserId { get; set; }
                public string UserSAPCode { get; set; }
                public string FullName { get; set; }
                public Group Role { get; set; }
                public bool IsHeadCount { get; set; }
                public bool IsEdoc { get; set; }
                public string Note { get; set; }
                public bool? Authorizated { get; set; }
                public DateTimeOffset? Created { get; set; }
                public DateTimeOffset? Modified { get; set; }
                public Guid? BusinessModelId { get; set; }
            }

            public class UpdateInformation
            {
                public class LeaveApplication
                {
                    public Guid WorkingAddressRecruitmentId;
                    public string WorkingAddressRecruitmentCode;
                    public string WorkingAddressRecruitmentName;
                    public CheckBudgetOption HasBudget { get; set; }
                }
            }
            public class ValidationShiftExchangePRD
            {
                public string SAPCode { get; set; }
                public Guid TargetPeriod { get; set; }
                public int PRD { get; set; }
            }
        }
        public class ValidationShiftExchangePRD
        {
            public string SAPCode { get; set; }
            public Guid TargetPeriod { get; set; }
            public int PRD { get; set; }
        }
    }
}
