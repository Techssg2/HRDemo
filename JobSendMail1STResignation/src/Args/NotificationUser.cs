using JobSendMail1STResignation.src.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.Args
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