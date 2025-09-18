using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class OvertimeApplicationViewModel:CBUserInfoViewModel
    {
        public OvertimeApplicationViewModel()
        {
            OvertimeItems = new List<OvertimeApplicationDetailViewModel>();
        }
        public Guid Id { get; set; }     
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string ContentOfOtherReason { get; set; }
        public OverTimeType Type { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset? ApplyDate { get; set; }
        //public List<OvertimeApplicationDetailViewModel> OvertimeItems { get; set; }
        public ICollection<OvertimeApplicationDetailViewModel> OvertimeItems { get; set; }
        public Guid DivisionId { get; set; }
        public string TimeInRound { get; set; }
        public string TimeOutRound { get; set; }
    }
    public class OvertimeApplicationDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid OvertimeApplicationId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string DetailReason { get; set; }
        public string ProposalHoursFrom { get; set; }
        public string ProposalHoursTo { get; set; }
        public string ActualHoursFrom { get; set; }
        public string ActualHoursTo { get; set; }
        public string CalculatedActualHoursFrom { get; set; }
        public string CalculatedActualHoursTo { get; set; }
        public bool DateOffInLieu { get; set; }
        public bool IsNoOT { get; set; }
        /// Manager Apply for Emloyee
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public string SAPCode { get; set; }
        public string Department { get; set; }
        public string ContentOfOtherReason { get; set; }
        public bool? IsStore { get; set; }
    }
    public class ExportOvertimeApplicationDetailViewModel
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string ActualOtHours { get; set; }
        public string OtProposalHours { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string DetailReason { get; set; }
        public string ProposalHoursFrom { get; set; }
        public string ProposalHoursTo { get; set; }
        public string ActualHoursFrom { get; set; }
        public string ActualHoursTo { get; set; }
        public bool DateOffInLieu { get; set; }
        public bool IsNoOT { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public string SubmitFullName { get; set; }
        public string SubmitSAPCode { get; set; }
        public string SAPCodeUserOT { get; set; }
        public string FullNameUserOT { get; set; }
        public string ContentOfOtherReason { get; set; }

    }
}