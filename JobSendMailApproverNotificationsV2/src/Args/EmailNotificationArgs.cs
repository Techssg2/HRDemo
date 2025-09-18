using JobSendMailApproverNotificationsV2.src.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Args
{
    public class EmailNotificationArgs
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public IList<string> Recipients { get; set; }
        public IList<string> CcRecipients { get; set; }
        public IDictionary<string, byte[]> Attachments { get; set; }
        public string Smtp { get; set; }
        public int Port { get; set; }
        public bool RequiredAuthentication { get; set; }
        public bool EnableSSL { get; set; }
        public string Sender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
