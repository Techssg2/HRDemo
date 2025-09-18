using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class DepartmentTreeAPIViewModel
    {
        public DepartmentTreeAPIViewModel()
        {
            Items = new HashSet<DepartmentTreeAPIViewModel>();
        }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string SAPCode { get; set; }
        public string Name { get; set; }
        public string JobGradeCaption { get; set; }
        public int JobGradeGrade { get; set; }
        public bool IsStore { get; set; }
        public bool IsHr { get; set; }
        public bool IsCB { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsFacility { get; set; }
        public bool IsSM { get; set; }
        public bool IsMD { get; set; }
        public bool IsAcademy { get; set; }
        public string Note { get; set; }
        public Guid? BusinessModelId { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public IEnumerable<DepartmentTreeAPIViewModel> Items { get; set; }
        public DepartmentType Type { get; set; }
        public IEnumerable<UserDepartmentMappingAPI_DTO> UserDepartmentMappings { get; set; }
        public bool IsMMD { get; set; }
        public bool ApplyChildSMMDMMD { get; set; }
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
