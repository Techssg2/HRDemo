using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.ViewModels
{
    public class ResignationApplicationViewModel: CBUserInfoViewModel
    {
        public Guid Id { get; set; }     
        public DateTimeOffset OfficialResignationDate { get; set; }
        public ShuiBooks SHUIBookCode { get; set; }   // lấy từ masterData
        public string SHUIBookName { get; set; }
        public string ReasonForActionCode { get; set; }
        public string ReasonForActionName { get; set; }
        public double UnusedLeaveDate { get; set; }
        public ContractType ContractTypeCode { get; set; }   
        public string ContractTypeName { get; set; }
        public DateTimeOffset? SuggestionForLastWorkingDay { get; set; }
        public bool? IsAgree { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool IsNotifiedLastWorkingDate { get; set; }
        public string ReasonDescription { get; set; }
        public string PositionName { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTimeOffset? SubmitDate { get; set; }
        public int? CountExitInterview { get; set; }
        public bool? IsInactiveUserOnResignationDateJob { get; set; }
    }
    public class ResignationApplicationPrintFormViewModel
    {
        public ResignationApplicationPrintFormViewModel()
        {
            CheckedBoxs = new List<string>();
        }
        public string UserSAPCode { get; set; }
        public string CreatedByFullName { get; set; }
        public string DeptName { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationName { get; set; }
        public string PositionName { get; set; }
        public string ReasonDescription { get; set; } // Ly do nghi viec
        public string UnusedLeaveDate { get; set; }
        public List<string> CheckedBoxs { get; set; }
        public string StartingDate { get; set; }
        public string OffResDate { get; set; }
        public string CreatedDate { get; set; }
        public string SuggestionForLastWorkingDay { get; set; }
        public string ReasonForActionName { get; set; }
        // Addition Field
        public string HeadLineManager { get; set; }
        public string SignedDateHeadLineManager { get; set; }
        public string StoreManager { get; set; }
        public string SignedDateStoreManager { get; set; }
        public string HrConfirmName { get; set; }
        public string SignedDateHrConfirm { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }
        public string ConfirmDate { get; set; }
    }
}