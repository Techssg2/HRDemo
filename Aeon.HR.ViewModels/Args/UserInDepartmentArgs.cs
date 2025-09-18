using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Enums;
namespace Aeon.HR.ViewModels.Args
{
    public class UserInDepartmentArgs
    {
        public Guid DepartmentId { get; set; }
        public Guid UserId { get; set; }
        public Group? Role { get; set; } // là role hod hoặc checked
        public bool IsHeadCount { get; set; }
        public bool IsEdoc { get; set; }
        public string Note { get; set; }
    }
}