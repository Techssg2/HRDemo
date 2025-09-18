using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Model
{
    public class EdocTaskModel
    {
        public EdocTaskModel()
        {

            Edoc1Tasks = new List<WorkflowTaskModel>();
            Edoc2Tasks = new List<WorkflowTaskModel>();
        }
        public NotificationUserModel User { get; set; }
        public NotificationUserModel CCUser { get; set; }
        public List<WorkflowTaskModel> Edoc1Tasks { get; set; } // Edoc2
        public List<WorkflowTaskModel> Edoc2Tasks { get; set; }
    }
}
