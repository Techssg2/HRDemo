using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.HR.Data.Models
{
    public class UserDepartmentMapping: SoftDeleteEntity
    {
        public Guid? DepartmentId { get; set; }
        public Guid? UserId { get; set; }
        public Group Role { get; set; } // là role hod hoặc checked
        public bool IsHeadCount { get; set; } // là người đứng đầu
        // Phía dưới là danh sách khóa ngoại
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public string Note { get; set; }
        public bool? Authorizated { get; set; }
        public bool IsFromIT { get; set; }
        public bool IsEdoc { get; set; }
        public bool IsPrepareDelete { get; set; }
    }
}