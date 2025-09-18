using Aeon.HR.Infrastructure.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class UserSubmitPersonDeparmentMapping: AuditableEntity
    {
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsSubmitPerson { get; set; }
        // Phía dưới là danh sách khóa ngoại
        public bool IsIncludeChildren { get; set; }
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
    }
}
