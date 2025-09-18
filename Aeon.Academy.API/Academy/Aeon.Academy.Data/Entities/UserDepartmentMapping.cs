using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Entities
{
    public class UserDepartmentMapping : BaseTrackingEntity
    {
        public Guid? DepartmentId { get; set; }
        public Guid? UserId { get; set; }
        public int Role { get; set; }
        public bool IsHeadCount { get; set; }
    }
}
