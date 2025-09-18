using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class TrackingHistoryArgs
    {
        public string ItemRefereceNumberOrCode { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Type { get; set; }
        public string DataStr { get; set; }
        public string Comment { get; set; }
        public string Documents { get; set; }
        public string RoundNum { get; set; }
        public Guid? InstanceId { get; set; }
        public Guid? ItemId { get; set; }
        public string WorkflowDataStr { get; set; }
    }
}
