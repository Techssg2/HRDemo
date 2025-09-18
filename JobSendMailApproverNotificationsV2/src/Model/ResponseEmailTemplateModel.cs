using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Model
{
    public class ResponseEmailTemplateModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
    }

    public class ResponseCreateEmailWaitlistModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
