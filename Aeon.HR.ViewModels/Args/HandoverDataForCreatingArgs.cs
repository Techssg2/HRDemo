using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class HandoverDataForCreatingArgs
    {
        public Guid? Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string UserFullName { get; set; }
        public string UserDeptName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public string PositionName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string JobGradeCaption { get; set; }
        public string DepartmentType { get; set; }
        public string DeptDivision { get; set; }
        public string DeptDivisionCode { get; set; }
        public bool IsCancel { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public Guid PositionId { get; set; }
       
        public Guid ApplicantId { get; set; } // lưu cái hanover này thuộc về nhân viên nào // nhân viên được chọn trong dropdown "SAP Code"
        public ICollection<HandoverDetailItemViewModel> HandoverDetailItems { get; set; }
    }

    public class ExportHanvoderViewModel
    {
        public string ReferenceNumber { get; set; }
        public string ApplicantReferenceNumber { get; set; }
        public string ApplicantFullName { get; set; }
        public string Department { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public string CreatedDate { get; set; }
        public string IsCancel { get; set; }
    }

    public class HandoverItemDetailViewModel
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }
}