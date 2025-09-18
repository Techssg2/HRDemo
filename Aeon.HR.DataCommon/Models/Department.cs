using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.Data.Models
{
    public class Department : SoftDeleteEntity
    {
        public Department()
        {
            UserDepartmentMappings = new HashSet<UserDepartmentMapping>();
            UserSubmitPersonDeparmentMappings = new HashSet<UserSubmitPersonDeparmentMapping>();
            TargetPlanSpecialDeparmentMappings = new HashSet<TargetPlanSpecialDepartmentMapping>();
        }
        [StringLength(50)]
        [Required]
        public string Code { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        [Required]
        //public string Caption { get; set; }
        //public Guid PositionId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public DepartmentType Type { get; set; } //là division hoặc department
        [Required]
        public Guid JobGradeId { get; set; }
        public Guid? BusinessModelId { get; set; }
        public Guid? ParentId { get; set; }
        [StringLength(50)]
        public string SAPCode { get; set; } // là code của phòng ban bên phía hệ thống SAP
        [StringLength(10)]
        public string Color { get; set; } // mã màu của phòng ban
        public bool IsStore { get; set; }
        public bool IsHR { get; set; }
        public bool IsCB { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPerfomance { get; set; }
        public bool IsFacility { get; set; }
        public Guid? HrDepartmentId { get; set; }
        public Guid? RequestToHireId { get; set; }
        public string RTHReferenceNumber { get; set; }
        public Guid? RegionId { get; set; }
        public bool EnableForPromoteActing { get; set; }
        public bool IsMD { get; set; }
        public bool IsSM { get; set; }
        //--Phía dưới là danh sách khóa ngoại
        public virtual JobGrade JobGrade { get; set; }
        public virtual BusinessModel BusinessModel { get; set; }
        public virtual Region Region { get; set; }
        public virtual Department Parent { get; set; }
        public virtual ICollection<Acting> Actings { get; set; }
        public virtual ICollection<UserDepartmentMapping> UserDepartmentMappings { get; set; } // Danh sách các nhân viên thuộc phòng ban này
        public virtual ICollection<UserSubmitPersonDeparmentMapping> UserSubmitPersonDeparmentMappings { get; set; } // Danh sách các nhân viên thuộc phòng ban này
        public virtual ICollection<TargetPlanSpecialDepartmentMapping> TargetPlanSpecialDeparmentMappings { get; set; } // Danh sách các nhân viên thuộc phòng ban này
        public Guid? CostCenterRecruitmentId { get; set; }
        public virtual CostCenterRecruitment CostCenterRecruitment { get; set; }
        public string Note { get; set; }
        public bool IsAcademy { get; set; }
        public bool? HasTrackingLog { get; set; }
        public bool IsFromIT { get; set; }
        public bool IsMMD { get; set; }
        public bool ApplyChildSMMDMMD { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsPrepareDelete { get; set; }
        #region Colum SAP
        [StringLength(50)]
        public string SAPObjectId { get; set; }
        [StringLength(10)]
        public string SAPObjectType { get; set; }
        [StringLength(255)]
        public string SAPDepartmentParentName { get; set; }
        [StringLength(50)]
        public string SAPDepartmentParentId { get; set; }
        [StringLength(10)]
        public string SAPLevel { get; set; }
        [StringLength(20)]
        public string SAPValidFrom { get; set; }
        [StringLength(20)]
        public string SAPValidTo { get; set; }
        #endregion
    }
}