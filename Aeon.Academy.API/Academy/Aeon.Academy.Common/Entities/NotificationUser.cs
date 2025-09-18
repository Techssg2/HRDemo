using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.Academy.Common.Workflow;

namespace Aeon.Academy.Common.Entities
{
    public class NotificationUser
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public Guid DepartmentId { get; set; }
        public Group DepartmentGroup { get; set; }
    }
}
