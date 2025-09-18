using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class TestDepartment: SoftDeleteEntity
    {
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
        public Guid? ParentId { get; set; }
        [StringLength(50)]
        public string SAPCode { get; set; } // là code của phòng ban bên phía hệ thống SAP
        [StringLength(10)]
        public string Color { get; set; } // mã màu của phòng ban

        public bool IsStore { get; set; }
        public bool IsHR { get; set; }
        public bool IsPerfomance { get; set; }     
        //--Phía dưới là danh sách khóa ngoại
        public virtual JobGrade JobGrade { get; set; }
        public virtual TestDepartment Parent { get; set; }
    }
}
