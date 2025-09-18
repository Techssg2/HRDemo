using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ResignationApplicantGridViewModel: CBUserInfoViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserFullName { get; set; }
        public UserDepartmentMappingViewModel Mapping { get; set; }
        public string UserWorkLocationName { get; set; }   
        public DateTimeOffset Created { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }

        //còn thiếu 2 field dept/line và division/group và work location
    }
    public class ExportResignationApplicantGridViewModel
    {
        public string Status { get; set; }
        public string ReferenceNumber { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public DateTimeOffset JoiningDate { get; set; }
        public DateTimeOffset OfficialResignationDate { get; set; }
        public ShuiBooks SHUIBookCode { get; set; }   // lấy từ masterData
        public string SHUIBookName { get; set; }
        public string ReasonForActionCode { get; set; }
        public string ReasonForActionName { get; set; }
        public ContractType ContractTypeCode { get; set; }
        public string ContractTypeName { get; set; }
        public DateTimeOffset? SuggestionForLastWorkingDay { get; set; }
        public bool? IsAgree { get; set; }
        public bool IsNotifiedLastWorkingDate { get; set; }
        public string ReasonDescription { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }
    }
}