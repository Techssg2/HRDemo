using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class PermissionMappingViewModel
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}