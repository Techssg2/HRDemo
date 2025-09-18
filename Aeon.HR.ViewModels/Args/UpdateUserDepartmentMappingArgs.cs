using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.ViewModels.Args
{
    public class UpdateUserDepartmentMappingArgs
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public Group Role { get; set; }  
        public bool IsHeadCount { get; set; }
        public bool IsEdoc { get; set; }
        public string Note { get; set; }
    }
}