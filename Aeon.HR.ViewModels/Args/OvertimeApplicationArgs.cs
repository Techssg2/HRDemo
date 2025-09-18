using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class OvertimeApplicationArgs
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserSAPCode { get; set; }
        public string Status { get; set; }
        public Guid? DeptId { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonName { get; set; }
        public string ContentOfOtherReason { get; set; }
        public OverTimeType  Type { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public DateTimeOffset? ApplyDate { get; set; }
        public List<OvertimeApplicationDetailArgs> OvertimeList { get; set; }
        public Guid DivisionId { get; set; }
        public string TimeInRound { get; set; }
        public string TimeOutRound { get; set; }

    }
    public class OvertimeApplicationDetailArgs
    {
        public Guid Id { get; set; }
        public Guid OvertimeApplicationId { get; set; }
        public string Date { get; set; }
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
        public string SAPCode { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public bool? IsStore { get; set; }
    }
}