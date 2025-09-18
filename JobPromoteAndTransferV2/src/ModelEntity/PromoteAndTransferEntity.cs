
using JobPromoteAndTransferV2.src.SQLExcute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransfer.src.ModelEntity
{
    [JobPromoteAndTransferV2.src.SQLExcute.TableAttribute(TableName = "ResignationApplications")]
    public class PromoteAndTransferEntity : BaseEntity
    {
        public string TypeCode { get; set; } // master data
        public string TypeName { get; set; } // master data
        public string RequestFrom { get; set; } // trong endpoint để kiểu dữ liệu là string
        public Guid? UserId { get; set; } // tương ứng với sap code trong promote and tranfer form // nhằm lấy ra được các thông tin bị disable
        public string FullName { get; set; }
        public string ReasonOfPromotion { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public double NewSalaryOrBenefit { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? ActingPositionId { get; set; }
        public string PositionName { get; set; }
        public string NewTitleCode { get; set; } // master data     //position
        public string NewTitleName { get; set; } // master data     //position
        public Guid? NewJobGradeId { get; set; }
        public string NewJobGradeName { get; set; } // master data 
        public Guid? NewDeptOrLineId { get; set; } // id của phòng ban mới // tạm thởi chưa biết lưu vào model nào nên chưa tạo khoái ngoại
        public string NewDeptOrLineName { get; set; }
        public string NewDeptOrLineCode { get; set; }
        public bool IsStoreNewDepartment { get; set; }
        public string NewWorkLocationCode { get; set; } // master data 
        public string NewWorkLocationName { get; set; } // master data 
        public string CurrentTitle { get; set; }
        public string CurrentJobGrade { get; set; }
        public string CurrentDepartment { get; set; }
        public string CurrentWorkLocation { get; set; }
        //CR222 ================================================
        public string PersonnelArea { get; set; }
        public string PersonnelAreaText { get; set; }
        public string EmployeeGroup { get; set; }
        public string EmployeeGroupDescription { get; set; }
        public string EmployeeSubgroup { get; set; }
        public string EmployeeSubgroupDescription { get; set; }
        public string PayScaleArea { get; set; }
        //======================================================
        public Guid? CurrentDepartmentId { get; set; }
        public Guid? ReportToId { get; set; } // sẽ report đến người nào
        public DateTimeOffset StartingDate { get; set; } // Trong endpoint có starting date chưa rõ dùng để làm gì
        public string Documents { get; set; } // dành cho button attach document. sẽ lưu id của các file cách nhau bởi dấu ";" .Ví dụ xxxxxx;yyyyyy

        //-START-Phần dành cho workflow // Phần này viết cho vui thôi chứ chưa biết define sao, thấy chưa phù hợp lắm
        public string CurrentWorkFlow { get; set; } // Một json lưu lại Workflow đang chạy
        public string CurrentWorkFlowStep { get; set; } // bước hiện tại đang chạy
        public string WorkFlowHistory { get; set; } // Lưu lại lịch sử chạy workflow
        public string StatusCode { get; set; } // master data 
        public string StatusName { get; set; } // master data 
        public bool IsSameDepartment { get; set; }
        public int CurrentJobGradeValue { get; set; }
        public string ReportToFullName { get; set; }
        public string ReportToSAPCode { get; set; }

        [StringLength(100)]
        public string TargetDeptCode { get; set; }
        [StringLength(100)]
        public string TargetDeptName { get; set; }
        public string Status { get; set; }
        public override void Fill(SqlDataReader sqlDataReader)
        {
            base.Fill(sqlDataReader);
            this.Status = this.GetData<string>(sqlDataReader, "Status");
            this.EffectiveDate = this.GetData<DateTimeOffset>(sqlDataReader, "EffectiveDate");
            this.UserId = this.GetData<Guid>(sqlDataReader, "UserId");
            this.NewDeptOrLineId = this.GetData<Guid>(sqlDataReader, "NewDeptOrLineId");
            this.CurrentDepartmentId= this.GetData<Guid>(sqlDataReader, "CurrentDepartmentId");
        }
        public override void FillOut(SqlCommand sqlCommand)
        {
            base.FillOut(sqlCommand);
        }
    }
}
