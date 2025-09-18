using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class DepartmentArgs
    {
        public Guid? Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Code { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        [Required]
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        [Required]
        public DepartmentType Type { get; set; } //là division hoặc department
        public Guid? JobGradeId { get; set; }
        public Guid? RegionId { get; set; }
        public Guid? ParentId { get; set; }
        [StringLength(50)]
        public string SAPCode { get; set; } // là code của phòng ban bên phía hệ thống SAP
        [StringLength(10)]
        public string Color { get; set; }
        public bool IsStore { get; set; }
        public bool IsHR { get; set; }
        public bool IsCB { get; set; }
        public bool IsPerfomance { get; set; }
        public bool IsAdmin { get; set; }
        public Guid? HrDepartmentId { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public bool IsFacility { get; set; }
        public bool EnableForPromoteActing { get; set; }
        public bool IsMD { get; set; }
        public bool IsSM { get; set; }
        public bool IsEdoc { get; set; }
        public string Note { get; set; }
        public bool IsAcademy { get; set; }
        public Guid? BusinessModelId { get; set; }
        public bool IsMMD { get; set; }
        public bool ApplyChildSMMDMMD { get; set; }
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