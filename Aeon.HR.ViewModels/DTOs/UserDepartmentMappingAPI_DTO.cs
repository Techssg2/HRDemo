using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class UserDepartmentMappingAPI_DTO
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserSAPCode { get; set; }
        public string UserEmail { get; set; }
        public Group Role { get; set; }
        public bool IsHeadCount { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsEdoc { get; set; }
    }
}
