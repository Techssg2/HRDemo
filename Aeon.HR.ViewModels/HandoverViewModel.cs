using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class HandoverViewModel
    {
        public Guid? Id { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTimeOffset ReceivedDate { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid PositionId { get; set; }
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public string UserDeptName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public string PositionName { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string JobGradeCaption { get; set; }
        public string DepartmentType { get; set; }
        public string DeptDivision { get; set; }
        public string DeptDivisionCode { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid DeptDivisionJobGradeId { get; set; }
        public ICollection<HandoverDetailItemViewModel> HandoverDetailItems { get; set; }
        public UserDepartmentMappingViewModel Mapping { get; set; }
        public string ApplicantReferenceNumber { get; set; }
        public string ApplicantFullName { get; set; }
        public bool IsCancel { get; set; }

        //Upgrade
        public string CreatedByFullName { get; set; }
    }
}