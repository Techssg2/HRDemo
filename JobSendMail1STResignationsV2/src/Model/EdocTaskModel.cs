using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Model
{
    public class EdocTaskModel
    {
        public EdocTaskModel()
        {
            Edoc2Tasks = new List<WorkflowTaskModel>();
        }
        public NotificationUserModel User { get; set; }
        public NotificationUserModel CCUser { get; set; }
        public List<WorkflowTaskModel> Edoc1Tasks { get; set; }
        public List<WorkflowTaskModel> Edoc2Tasks { get; set; }
    }
}