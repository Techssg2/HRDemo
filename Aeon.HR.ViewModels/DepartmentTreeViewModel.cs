using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class DepartmentTreeViewModel
    {
        public DepartmentTreeViewModel()
        {
            Items = new HashSet<DepartmentTreeViewModel>();
        }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string SAPCode { get; set; }
        public Guid? RequestToHireId { get; set; }
        public string Name { get; set; }
        public string JobGradeCaption { get; set; }
        public int JobGradeGrade { get; set; }
        public bool Expanded { get; set; }
        public string UserCheckedHeadCount { get; set; } = string.Empty;
        public string UserCheckedHeadCountSapCode { get; set; }
        public bool IsStore { get; set; }
        public bool IsHr { get; set; }
        public bool IsCB { get; set; }
        public bool IsPerfomance { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFacility { get; set; }
        public Guid? HrDepartmentId { get; set; }
        public bool HasEmployees { get { return Items.Any(); } }
        public IEnumerable<DepartmentTreeViewModel> Items { get; set; }
        public bool? IsIncludeChildren { get; set; }
        public DepartmentType Type { get; set; }
        public Guid JobGradeId { get; set; }
        public Guid? RegionId { get; set; }
        public bool EnableForPromoteActing { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public IEnumerable<string> SubmitSAPCodes { get; set; }
        public bool IsMD { get; set; }
        public bool IsSM { get; set; }
        public bool IsAcademy { get; set; }
        public string Note { get; set; }
        public bool? HasTrackingLog { get; set; }
        public Guid? BusinessModelId { get; set; }
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
        public bool? IsEdoc1 { get; set; }
    }
}