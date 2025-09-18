using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class RequestToHireViewModel : AuditableEntity
    { 
        public string Caption { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public TypeOfNeed ReplacementFor { get; set; }
        public string Type { get; set; }
        public Guid? DeptDivisionId { get; set; }        //lấy từ Department ở Setting => từ field này lấy giá trị Job Grade
        public string DeptDivisionCode { get; set; }
        public string DeptDivisionName { get; set; }
        public Guid? ReplacementForId { get; set; } // Department
        public string ReplacementForName { get; set; }
        public string ReplacementForCode { get; set; }
        public string DeptDivisionGrade { get; set; }
        public int ReplacementForGrade { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Status { get; set; }
        public int ExpiredDayPosition { get; set; }
        public double EstimateSalaryStart { get; set; }
        public double EstimateSalaryEnd { get; set; }
        public DateTimeOffset? StartingDateRequire { get; set; }
        public CheckBudgetOption HasBudget { get; set; }
        public int? CurrentBalance { get; set; }
        public Guid? JobGradeId { get; set; }
        public int JobGradeGrade { get; set; }
        public string JobGradeCaption { get; set; }
        public string JobGradeTitle { get; set; }
        public int Quantity { get; set; }
        public Guid? WorkingTimeId { get; set; }
        public string WorkingTimeCode { get; set; }
        public string WorkingTimeName { get; set; }
        public string ContractTypeCode { get; set; }       // có 2 giá trị : Full time / Part time 
        public string ContractTypeName { get; set; }
        public int? WorkingHoursPerWeerk { get; set; }
        public double? WagePerHour { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }  ///
        public string JobDescription { get; set; }
        public string JobRequirement { get; set; }
        public string ReferenceNumber { get; set; }     //khi form được save thì hệ thống sẽ tạo ra Reference Number   
        public bool IsStore { get; set; }               
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public Guid? AssignToId { get; set; }
        public string DepartmentName { get; set; }
        public string AssignToFullName { get; set; }
        public string AssignToSAPCode { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public string CostCenterRecruitmentCode { get; set; }
        public string CostCenterRecruitmentDescription { get; set; }
        //UI ticket 432
        public Guid? ReplacementForUserId { get; set; }
        public string ReplacementForUserSAPCode { get; set; }
        public string ReplacementForUserFullName { get; set; }
        public ReasonOptions Reason { get; set; }
        public string OtherReason { get; set; }
        //end
        public OperationOptions Operation { get; set; }
        public Guid? WorkingAddressRecruitmentId { get; set; }
        public string CategoryName { get; set; }
        public Guid? CategoryId { get; set; }
        public string MassLocationCode { get; set; }
        public string MassLocationName { get; set; }
        public string WorkingAddressRecruitmentCode { get; set; }
        public string WorkingAddressRecruitmentAddress { get; set; }
        public Guid? ResignationId { get; set; }
        public string ResignationNumber { get; set; }

        public DateTimeOffset? SignedDate { get; set; }
        public string AssigneeUserName { get; set; }
        public string ReplacementForUserName { get; set; }
        public string RegionName { get; set; }
    }
    public class RequestToHireForPrintViewModel
    {
        public RequestToHireForPrintViewModel()
        {
            CheckedBoxs = new List<string>();
        }
        // Common Fields
        public string Position { get; set; }
        public string DepartmentName { get; set; }
        public string ReplacementForName { get; set; }
        public string TotalCurrentBalance { get; set; }
        public string WorkingTimeCode { get; set; }
        public string WorkLocation { get; set; }
        public int JobGrade { get; set; }
        public string JobGradeTitle { get; set; }
        public string Quantity { get; set; }
        public string SalaryFromTo { get; set; }     
        public string StartingDate { get; set; }
        public string WHPerWeek { get; set; }
        public string WagePerHours { get; set; }
        public string RequiredTime { get; set; }        
        // Proposer Information
        public string ApproverStep1 { get; set; }
        public string ApproverStep1NotSigned { get; set; }
        public string ApproverStep1Position { get; set; }
        public string StepSigned1Date { get; set; }

        public string PositionOfApproverStep1 { get; set; }
        public string DepartmentOfOfApproverStep1 { get; set; }
        public string CompletedDateStep1 { get; set; }
        // Approval For Proposal
        public string ApproverStep2 { get; set; }
        public string StepSigned2Date { get; set; }
        public string ApproverStep2Position { get; set; }
        public string ApproverStep3 { get; set; }
        public string StepSigned3Date { get; set; }
        public string ApproverStep3Position { get; set; }
        public string ApproverStep4 { get; set; }
        public string StepSigned4Date { get; set; }
        public string ApproverStep4Position { get; set; }
        public string ApproverStep5 { get; set; }
        public string StepSigned5Date { get; set; }
        public string ApproverStep5Position { get; set; }
        public string HrManager { get; set; }
        public string HRSignedDate { get; set; }
        public string HRManagerPosition { get; set; }
        // CheckBox
        public List<string> CheckedBoxs { get; set; }
        public string ReplacementForUserFullName { get; set; }
        public string ContractTypeCode { get; set; }
        public CheckBudgetOption HasBudget { get; set; }
        public double EstimateSalaryEnd { get; set; }
        public double EstimateSalaryStart { get; set; }
        public TypeOfNeed ReplacementFor { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public string HQOperation { get; set; }
        public string CostCenter { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? JobGradeId { get; set; }
    }
}