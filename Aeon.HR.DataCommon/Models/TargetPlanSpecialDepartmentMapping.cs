using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class TargetPlanSpecialDepartmentMapping : AuditableEntity
    {
        public Guid DepartmentId { get; set; }
        // Phía dưới là danh sách khóa ngoại
        public bool IsIncludeChildren { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }

        public virtual Department Department { get; set; }
    }
}
