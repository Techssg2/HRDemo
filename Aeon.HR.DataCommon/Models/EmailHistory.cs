using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Data.Models
{
    public class EmailHistory : SoftDeleteEntity
    {
        public Guid ItemId { get; set; }
        public string ReferenceNumber { get; set; }
        public string ItemType { get; set; }
        public string UserSent { get; set; }  /*UserInfo*/
        public Guid? DepartmentId { get; set; }
        public Group DepartmentType { get; set; }
        public bool IsSent { get; set; }
        public class UserInfo
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; }
        }
    }
}
