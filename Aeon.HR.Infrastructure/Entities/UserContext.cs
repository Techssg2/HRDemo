using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Entities
{
    public class UserContext
    {
        public Guid CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentUserFullName { get; set; }
        public UserRole CurrentUserRole { get; set; }
        public bool IsHQ { get; set; }
        public string DeptCode { get; set; }
        public Guid DeptId { get; set; }
        public Guid? DeptG5Id { get; set; }
    }
}
