using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class ItemListDepartmentViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public DepartmentType Type { get; set; } //là division hoặc department
        public string JobGradeId { get; set; }
        public int? JobGradeGrade { get; set; }
        public string JobGradeCaption { get; set; }
        public string JobGradeTitle { get; set; }
        public int JobGradeExpiredDayPosition { get; set; }
        public string ParentName { get; set; }
        public DepartmentType? ParentDepartmentType { get; set; }
        public Guid? ParentId { get; set; } 
        public string SAPCode { get; set; } // là code của phòng ban bên phía hệ thống SAP
        public string Color { get; set; }
        public int UserCount { get; set; }
        public bool HasUserPrepareDelete { get; set; }
        public int HeadCountQuantity { get; set; }
        public bool IsStore { get; set; }
        public bool IsHr { get; set; }
        public bool IsCB { get; set; }
        public bool IsPerfomance { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFacility { get; set; }
        public bool EnableForPromoteActing { get; set; }
        public bool IsMD { get; set; }
        public bool IsSM { get; set; }
        public bool IsAcademy { get; set; }
        public Guid? RegionId { get; set; }
        public string RegionName { get; set; }
        public Guid? HrDepartmentId { get; set; }
        public List<ItemListDepartmentViewModel> Items { get; set; }
        public string TypeName { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public string CostCenterRecruitmentCode { get; set; }
        public string CostCenterRecruitmentDescription { get; set; }
        public Guid? RequestToHireId { get; set; }
        public string RTHReferenceNumber { get; set; }
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