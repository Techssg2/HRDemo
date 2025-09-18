using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class ResignationApplicationArgs
    {
        public Guid Id { get; set; }     
        public string UserSAPCode { get; set; }
        public DateTimeOffset OfficialResignationDate { get; set; }
        public int SHUIBookCode { get; set; }   // lấy từ masterData
        public string SHUIBookName { get; set; }
        public string ReasonForActionCode { get; set; }
        public string ReasonForActionName { get; set; }
        public double UnusedLeaveDate { get; set; }
        public int ContractTypeCode { get; set; }   // lấy từ masterData
        public string ContractTypeName { get; set; }
        public DateTimeOffset? SuggestionForLastWorkingDay { get; set; }
        public bool? IsAgree { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public bool IsNotifiedLastWorkingDate { get; set; }
        public string ReasonDescription { get; set; }
        public string PositionName { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }
        public Guid? DeptId { get; set; }
    }
}