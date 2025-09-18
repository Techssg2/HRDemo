using Aeon.HR.Infrastructure.Enums;
using System;

namespace Aeon.HR.ViewModels
{
    public class PositionViewModel 
    {
        public Guid Id { get; set; }

        public string PositionName { get; set; }
        public Guid DeptDivisionId { get; set; }
        public string DeptDivisionName { get; set; }
        public string DeptDivisionCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int ExpiredDay { get; set; }
        public bool HasBudget { get; set; }
        public int Quantity { get; set; }
        public Guid AssignToId { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid? ApplicantId { get; set; }
        public PositionStatus Status { get; set; }
        public int ApplicantsCount { get; set; }
        public int HiredApplicantsCount { get; set; }
        public string AssignToFullName { get; set; }
        public string AssignToSAPCode { get; set; }
        public string AssignToEmail { get; set; }
        public string DeptDivisionJobGradeCaption { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public Guid RequestToHireId { get; set; }
        public string RequestToHireReferenceNumber { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public string AssignToDepartmentCode { get; set; }
        public string AssignToDepartmentName { get; set; }
        public Guid? AssignToJobGradeId { get; set; }
        public string AssignToJobGradeGrade { get; set; }
        public string AssignToJobGradeCaption { get; set; }
        //
        public Guid? RequestToHireDeptDivisionId { get; set; }
        public string RequestToHireDeptDivisionName { get; set; }
        public string RequestToHireDeptDivisionCode { get; set; }
        public string RequestToHireDeptDivisionGrade { get; set; }
        public Guid? RequestToHireJobGradeId { get; set; }
        public string RequestToHireJobGradeCaption { get; set; }
        public string RequestToHireJobGradeTitle { get; set; } //===== CR11.2 =====
        public string RequestToHireLocationCode { get; set; }
        public string RequestToHireLocationName { get; set; }

    }
}