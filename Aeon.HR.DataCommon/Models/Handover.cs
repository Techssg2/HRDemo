using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class Handover : SoftDeleteEntity, IAutoNumber, IPermission
    {
        public Handover() {
            HandoverDetailItems = new HashSet<HandoverDetailItem>();
        }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }       
        public Guid ApplicantId { get; set; } // lưu cái hanover này thuộc về nhân viên nào // nhân viên được chọn trong dropdown "SAP Code"
        public Guid PositionId { get; set; }
        public string UserFullName { get; set; }
        public string UserDeptName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public string PositionName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string OldJobGradeCaption { get; set; }
        public string JobGradeCaption { get; set; }
        public string DepartmentType { get; set; }
        public string DeptDivision { get; set; }
        public string DeptDivisionCode { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public bool IsCancel { get; set; }
        // Phía dưới là danh sách khóa ngoại
        public virtual Applicant Applicant { get; set; }
        public virtual ICollection<HandoverDetailItem> HandoverDetailItems { get; set; }
    }

}