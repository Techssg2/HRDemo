using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class ResignationApplication : WorkflowEntity, ICBEntity
    {
        public ResignationApplication()
        {
        }      
        public DateTimeOffset OfficialResignationDate { get; set; }
        public ShuiBooks SHUIBookCode { get; set; }  
        public string SHUIBookName { get; set; }
        public string ReasonForActionCode { get; set; }
        public string ReasonForActionName{ get; set; }
        public double UnusedLeaveDate { get; set; }
        public ContractType ContractTypeCode { get; set; }   
        public string ContractTypeName { get; set; }
        public DateTimeOffset? SuggestionForLastWorkingDay { get; set; }
        public bool? IsAgree { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public string UserSAPCode { get; set; }
        public bool IsNotifiedLastWorkingDate { get; set; }
        public string ReasonDescription { get; set; }
        public string PositionName { get; set; }
        public bool IsExpiredLaborContractDate { get; set; }
        public bool IsCancelResignation { get; set; }
        public DateTimeOffset? SubmitDate { get; set; }
        public int CountExitInterview { get; set; } // count so lan click exit interview
        public bool? IsInactiveUserOnResignationDateJob { get; set; } // count so lan click exit interview

        public Guid? DeptId { get; set; }
    }
}