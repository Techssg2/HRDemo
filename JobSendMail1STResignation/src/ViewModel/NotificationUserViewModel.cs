using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.ViewModel
{
    public class NotificationUserViewModel
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public Guid DepartmentId { get; set; }
        public Group DepartmentGroup { get; set; }
    }
}