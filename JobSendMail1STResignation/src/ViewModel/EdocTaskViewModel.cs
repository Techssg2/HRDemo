using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.ViewModel
{
    public class EdocTaskViewModel
    {
        public EdocTaskViewModel()
        {

            Edoc1Tasks = new List<WorkflowTaskViewModel>();
            Edoc2Tasks = new List<WorkflowTaskViewModel>();
        }
        public NotificationUserViewModel User { get; set; }
        public NotificationUserViewModel CCUser { get; set; }
        public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; } // Edoc2
        public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
    }
}
