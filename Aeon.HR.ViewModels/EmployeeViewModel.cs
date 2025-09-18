using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        public string SAPCode { get; set; }
        public string LoginName { get; set; }
        public string FullName { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public DateTime? StartDate { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }
        public string JobTitle { get; set; }
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string JobGradeCode { get; set; }
        public string JobGradeName { get; set; }
        public string JobGradeTitle { get; set; } //===== CR11.2 =====
        public int JobGradeValue { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public double? MaxWFH { get; set; }
        public StorePositionType? StorePosition { get; set; }
        public HQPositionType? HQPosition { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid? DeptId { get; set; }
        public Guid? DeptG5Id { get; set; }
        public Guid? DivisionId { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string ProfilePicture { get; set; }
        public LoginType Type { get; set; }
        public bool IsStore { get; set; }
        public bool IsHR { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsITHelpDesk { get; set; }
        public bool IsHRAdmin { get; set; }
        public bool IsFacility { get; set; }
        public bool IsCB { get; set; }
        public bool IsNotTargetPlan { get; set; }
        public bool IsTargetPlan { get; set; }
        //ngan them
        public Guid? ProfilePictureId { get; set; }
        public IEnumerable<string> SubmitPersons { get; set; }
        public List<ExportLeaveApplicationViewModel> LeaveApplications { get; set; }
        public List<ExportOvertimeApplicationDetailViewModel> OvertimeApplications { get; set; }
        public List<MissingTimelockViewModel> MissingTimeClocks { get; set; }
        public List<ExportShiftExchangeDetaiViewModel> ShiftExchangeApplications { get; set; }
        //end

        public double ErdRemain { get; set; }
        public double AlRemain { get; set; }
        public double DoflRemain { get; set; }
        public string RedundantPRD { get; set; }
        public string QuotaDataJson { get; set; }
        public Guid? ActingDepartmentId { get; set; }
        public DateTimeOffset? OfficialResignationDate { get; set; }
        public List<PromoteAndTransferViewModel> PromoteAndTransfers { get; set; }
    }
}
