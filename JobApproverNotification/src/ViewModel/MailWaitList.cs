using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ViewModel
{
    public class MailWaitList
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string TemplateCode { get; set; }
        public List<string> MailTo { get; set; }
        public List<string> MailCC { get; set; }
        public List<string> MailBCC { get; set; }
        public DateTimeOffset SendDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Module { get; set; }
        public Nullable<int> SendCount { get; set; }
        public List<L_AttachmentFile> Attachments { get; set; }
    }

    public class L_AttachmentFile
    {
        public string FileName { get; set; }
        public string Base64 { get; set; }
    }
}
