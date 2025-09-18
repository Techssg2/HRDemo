using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ViewModel
{
    public class EmailTemplate
    {
        public string Name { get; set; }
        public string TemplatCode { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }

    public class EmailTemplateViewModel
    {
        public string Code { get; set; }
    }
}
