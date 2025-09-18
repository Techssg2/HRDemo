using JobSendMailApproverNotificationsV2.src.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Args
{
    public class Edoc1ArgV2
    {
        public string LoginName { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public DateTimeOffset? FromDueDate { get; set; }
        public DateTimeOffset? ToDueDate { get; set; }
    }
}
