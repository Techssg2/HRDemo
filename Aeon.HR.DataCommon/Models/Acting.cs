using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class Acting : WorkflowEntity, IRecruimentEntity
    {
        public Acting()
        {
            Periods = new HashSet<Period>();
        }
        //--START-BOSUNG Workflow và status
        //--END-BOSUNG
        //public string TitleInActingPeriodCode { get; set; } // master data 
        public string TitleInActingPeriodName { get; set; } // master data 
        public string TemplateGoal { get; set; }// Một json lưu lại nội dung của table được hiện thị trong group Goal. Ví dụ [{goal: 'Hoàn thành bộ chỉ tiêu đánh giá', weight: 10 }] // weight là trọng số

        public bool? IsCompletedTranning { get; set; }
        public string TableCompulsoryTraining { get; set; } // Dùng để lưu nội dung của table trong Group compulsory training. Vi dụ : [{courseName: 'Huấn luyện nhân giao tiếp', duration: 'nhập gì cũng được', actualResult: 'free text'}]
        public Guid UserId { get; set; } // tương ứng với dropdown sap code
        public Guid? FirstAppraiserId { get; set; }
        public Guid? SecondAppraiserId { get; set; }
        public Guid? FirstAppraiserDepartmentId { get; set; }
        public Guid? SecondAppraiserDepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public string PositionName { get; set; } // Chi dung de hien thi
        public Guid DepartmentId { get; set; }
        public Guid? DepartmentSAPId { get; set; }
        public string FullName { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string WorkLocationCode { get; set; }
        public string WorkLocationName { get; set; }


        public string NewPersonnelArea { get; set; }
        public string NewPersonnelAreaText { get; set; }
        public string NewWorkLocationCode { get; set; }
        public string NewWorkLocationName { get; set; }

        public string CurrentPosition { get; set; }
        public string OldCurrentJobGrade { get; set; }
        public string CurrentJobGrade { get; set; }
        public string UserSAPCode { get; set; }

        //CR222 ================================================
        public string PersonnelArea { get; set; }
        public string PersonnelAreaText { get; set; }
        public string EmployeeGroup { get; set; }
        public string EmployeeGroupDescription { get; set; }
        public string EmployeeSubgroup { get; set; }
        public string EmployeeSubgroupDescription { get; set; }
        public string PayScaleArea { get; set; }
        //======================================================
        //CR210=================================================
        public string FirstAppraiserNote { get; set; }
        public string SecondAppraiserNote { get; set; }
        //======================================================
        public DateTimeOffset? Period1From { get; set; }
        public DateTimeOffset? Period1To { get; set; }
        public DateTimeOffset? Period2From { get; set; }
        public DateTimeOffset? Period2To { get; set; }
        public DateTimeOffset? Period3From { get; set; }
        public DateTimeOffset? Period3To { get; set; }
        public DateTimeOffset? Period4From { get; set; }
        public DateTimeOffset? Period4To { get; set; }
        public DateTimeOffset? StartingDate { get; set; }
        public int JobGradeValue { get; set; }
        public int OldJobGradeValue { get; set; }
        public string JobGradeName { get; set; }
        public string DepartmentActingPeriodName { get; set; }
        public Guid UserDepartmentId { get; set; }

        // Phía bên dưới là danh sách khóa ngoại
        public virtual User User { get; set; }
        public virtual User FirstAppraiser { get; set; }
        public virtual User SecondAppraiser { get; set; }
        public virtual ICollection<Period> Periods { get; set; } // danh sách peroid
        public virtual Department Department { get; set; }
        public virtual Department DepartmentSAP { get; set; }
    }
}