using JobApproverNotification.src.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.Args
{
    public class EdocTask
    {
        public EdocTask()
        {
            Edoc1Tasks = new List<WorkflowTaskViewModel>();
            Edoc2Tasks = new List<WorkflowTaskViewModel>();
        }
        public NotificationUser User { get; set; }
        public NotificationUser CCUser { get; set; }
        public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; } // Edoc2
        public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
    }
}
