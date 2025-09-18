using JobSendMail1STResignation.src.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.Args
{
    public class EdocTask
    {
        public EdocTask()
        {
            Edoc2Tasks = new List<WorkflowTaskViewModel>();
        }
        public NotificationUser User { get; set; }
        public NotificationUser CCUser { get; set; }
        public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; }
        public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
    }
}