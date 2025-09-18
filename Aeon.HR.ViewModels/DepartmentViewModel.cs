using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class DepartmentViewModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public string ParentCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string JobGradeCaption { get; set; }
        public Guid? JobGradeId { get; set; }
        public int JobGradeGrade { get; set; }
        public bool Expanded { get; set; }
        public string UserCheckedHeadCount { get; set; } = string.Empty;
        public string UserCheckedHeadCountSAPCode { get; set; }
        public Guid? UserCheckedHeadCountSAPId { get; set; }
        public IEnumerable<string> SubmitSAPCodes { get; set; }
        public bool IsStore { get; set; }
        public bool IsHr { get; set; }
        public bool IsCB { get; set; }
        public bool IsPerfomance { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsHR { get; set; }
        public bool IsFacility { get; set; }
        public bool IsMD { get; set; }
        public bool IsSM { get; set; }
        public DepartmentType? DepartmentType { get; set; }
        public StorePositionType? StorePosition { get; set; }
        public HQPositionType? HQPosition { get; set; }
        public bool? IsIncludeChildren { get; set; }
        public Guid? HrDepartmentId { get; set; }
        public string HrDepartmentName { get; set; }
        public string HrDepartmentCode { get; set; }
        public bool HasEmployees { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string SAPCode { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public bool EnableForPromoteActing { get; set; }
        public DepartmentType Type { get; set; }
        public string Note { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public string CostCenterRecruitmentCode { get; set; }
        public string CostCenterRecruitmentDescripion { get; set; }
        public Guid? BusinessModelId { get; set; }
        public string BusinessModelCode { get; set; }
        public string BusinessModelName { get; set; }
        public bool IsMMD { get; set; }
        public bool ApplyChildSMMDMMD { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsPrepareDelete { get; set; }
        #region Colum SAP
        public string SAPObjectId { get; set; }
        public string SAPObjectType { get; set; }
        public string SAPDepartmentParentName { get; set; }
        public string SAPDepartmentParentId { get; set; }
        public string SAPLevel { get; set; }
        public string SAPValidFrom { get; set; }
        public string SAPValidTo { get; set; }
        #endregion
    }
}
