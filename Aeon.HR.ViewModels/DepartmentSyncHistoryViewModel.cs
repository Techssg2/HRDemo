using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class DepartmentSyncHistoryViewModel
    {
        public Guid Id { get; set; }
        [StringLength(10)]
        public string Action { get; set; }
        [StringLength(100)]
        public string DeptCode { get; set; }
        [StringLength(255)]
        public string DeptName { get; set; }
        [StringLength(255)]
        public string SAPCode { get; set; }
        [StringLength(255)]
        public string PositionCode { get; set; }
        [StringLength(255)]
        public string PositionName { get; set; }
        public Guid GradeId { get; set; }
        public int Grade { get; set; }
        [StringLength(255)]
        public string GradeTitle { get; set; }
        public Guid? RegionId { get; set; }
        [StringLength(255)]
        public string RegionName { get; set; }
        public string ErrorList { get; set; }
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
        [StringLength(100)]
        public string ParentCode { get; set; }
        [StringLength(255)]
        public string ParentSapCode { get; set; }
        [StringLength(255)]
        public string ParentName { get; set; }
        [StringLength(100)]
        public string PersonalArea { get; set; }
        [StringLength(100)]
        public string SubArea { get; set; }
        public Guid? BusinessModelId { get; set; }
        [StringLength(100)]
        public string BusinessModelCode { get; set; }
        public string BusinessModelName { get; set; }
        public bool IsStore { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}
