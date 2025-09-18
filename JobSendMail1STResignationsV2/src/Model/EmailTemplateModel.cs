using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src.Model
{
    public class EmailTemplateModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string TemplatCode { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}