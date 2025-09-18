using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class RequestToHire : WorkflowEntity, IRecruimentEntity
    {
        public RequestToHire()
        {
        }
        public string PositionCode { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionName { get; set; }
        public Guid? DeptDivisionId { get; set; }        //lấy từ Department ở Setting => từ field này lấy giá trị Job Grade
        public string DeptDivisionName { get; set; }
        public string DeptDivisionCode { get; set; }
        public Guid? ReplacementForId { get; set; }
        public string ReplacementForName { get; set; }
        public string ReplacementForCode { get; set; }
        public string DeptDivisionGrade { get; set; }
        public int? ReplacementForGrade { get; set; }
        public string Caption { get; set; }
        public TypeOfNeed ReplacementFor { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public double EstimateSalaryStart { get; set; }
        public double EstimateSalaryEnd { get; set; }
        public DateTimeOffset? StartingDateRequire { get; set; }
        public CheckBudgetOption HasBudget { get; set; }
        public int? CurrentBalance { get; set; }
        public Guid? JobGradeId { get; set; }
        public string JobGradeCaption { get; set; }
        public int? JobGradeGrade { get; set; }
        public int Quantity { get; set; }
        public bool IsStore { get; set; }
        public Guid? WorkingTimeId { get; set; }
        public string WorkingTimeCode { get; set; }
        public string WorkingTimeName { get; set; }
        public string ContractTypeCode { get; set; }       // có 2 giá trị : Full time / Part time 
        public string ContractTypeName { get; set; }
        public int? WorkingHoursPerWeerk { get; set; }
        public double? WagePerHour { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool NewPosition { get; set; }
        public string JobDescription { get; set; }
        public string JobRequirement { get; set; }
        public int ExpiredDayPosition { get; set; }
        public Guid? RecruitmentStaffId { get; set; }
        public Guid? RecruitmentLeaderId { get; set; }
        public Guid? RecruitmentManagerId { get; set; }
        public Guid? ReplacementUserId { get; set; }
        public string ReplacementUserCode { get; set; }
        public string ReplacementUserName { get; set; }
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy
        public Guid? AssignToId { get; set; }
        public string DepartmentName { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public Guid? ReplacementForUserId { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string MassLocationCode { get; set; }
        public string MassLocationName { get; set; }
        public ReasonOptions Reason { get; set; }
        public string OtherReason { get; set; }
        public Guid? WorkingAddressRecruitmentId { get; set; }
        public OperationOptions Operation { get; set; }
        public Guid? ResignationId { get; set; }
        public string ResignationNumber { get; set; }
        public bool IsImport { get; set; }
        public string Preventive1 { get; set; } // Du phong 1
        public string Preventive2 { get; set; } // Du phong 2
        public string Preventive3 { get; set; } // Du phong 3
        public string Preventive4 { get; set; } // Du phong 4
        public string Preventive5 { get; set; } // Du phong 5
        public string DepartmentSAPCode { get; set; } // dung cho truong hop import department
        public string ListDepartmentSAPCode { get; set; } // dung cho truong hop import department
        public virtual Department RecruitmentStaff { get; set; }
        public JobGrade JobGrade { get; set; }
        public virtual Department RecruitmentLeader { get; set; }
        public virtual Department RecruitmentManager { get; set; }
        public virtual CostCenterRecruitment CostCenterRecruitment { get; set; }
        public virtual User AssignTo { get; set; }
        public virtual User ReplacementForUser { get; set; }
        public virtual WorkingAddressRecruitment WorkingAddressRecruitment { get; set; }
        public virtual RecruitmentCategory RecruitmentCategory { get; set; }
        public ICollection<AttachmentFile> AttachmentFiles { get; set; }
        //end
    }
}