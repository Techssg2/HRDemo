using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApproverNotification.src.ViewModel
{
    public class Edoc1TaskViewModel
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
