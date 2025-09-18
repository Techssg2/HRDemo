using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Model
{
    public class Edoc1ResultModel<T>
    {
        public Edoc1ResultModel()
        {
            Items = new List<T>();
        }
        public bool IsSuccess { get; set; }
        public string ErrorCodes { get; set; }
        public string Messages { get; set; }
        public int TotalItems { get; set; }
        public string Link { get; set; }
        public List<T> Items { get; set; }
    }
    public class SimpleEdoc1Result
    {
        public bool IsSuccess { get; set; }
        public string Messages { get; set; }
        public string ItemLink { get; set; }
    }
}
