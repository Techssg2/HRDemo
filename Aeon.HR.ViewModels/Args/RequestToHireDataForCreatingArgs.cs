using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class RequestToHireDataForCreatingArgs: AuditableEntity
    {
        public Guid? Id { get; set; }
        public string Caption { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public TypeOfNeed ReplacementFor { get; set; }
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
        public double EstimateSalaryStart { get; set; }
        public double EstimateSalaryEnd { get; set; }
        public DateTimeOffset? StartingDateRequire { get; set; }
        public CheckBudgetOption HasBudget { get; set; }
        public int? CurrentBalance { get; set; }
        public Guid? JobGradeId { get; set; }
        public int JobGradeGrade { get; set; }
        public string JobGradeCaption { get; set; }
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
        public int ExpiredDayPosition { get; set; }
        public Guid? ReplacementUserId { get; set; }
        public string ReplacementUserCode { get; set; }
        public string ReplacementUserName { get; set; }
        public string Status { get; set; }
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public Guid? AssignToId { get; set; }

        public string DepartmentName { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        //UI ticket 432
        public Guid? ReplacementForUserId { get; set; }
        public ReasonOptions Reason { get; set; }
        public string OtherReason { get; set; }
        //end
        public OperationOptions Operation { get; set; }
        public Guid? WorkingAddressRecruitmentId { get; set; }
        public string CategoryName { get; set; }
        public Guid? CategoryId { get; set; }
        public string MassLocationCode { get; set; }
        public string MassLocationName { get; set; }
        public Guid? ResignationId { get; set; }
        public string ResignationNumber { get; set; }
    }
}