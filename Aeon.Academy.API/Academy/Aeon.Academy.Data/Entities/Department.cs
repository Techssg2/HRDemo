using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class Department : BaseEntity
    {
        [StringLength(500)]
        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        [Required]
        public string Code { get; set; }

        [StringLength(50)]
        public string SAPCode { get; set; }

        [StringLength(500)]
        [Required]
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public Guid? ParentId { get; set; }
        public int Type { get; set; }

        public Guid? RegionId { get; set; }
        public Guid? CostCenterRecruitmentId { get; set; }
        public bool IsStore { get; set; }
        public Guid JobGradeId { get; set; }
        public bool? IsAcademy { get; set; }
    }
}
