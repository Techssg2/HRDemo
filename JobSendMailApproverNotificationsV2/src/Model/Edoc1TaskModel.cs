using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Model
{
    public class Edoc1TaskModel
    {
        public string ReferenceNumber { get; set; }
        public string Purpose { get; set; }
        public string Amount { get; set; }
        public string Requestor { get; set; }
        public string DueDate { get; set; }
        public string RequestedDepartmentName { get; set; }
        public string CreatedDate { get; set; }
        public string Link { get; set; }
        public string Status { get; set; }
    }
}
