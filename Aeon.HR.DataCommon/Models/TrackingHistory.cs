using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Data.Models
{
    public class TrackingHistory : SoftDeleteEntity
    { 
        public string ItemType { get; set; }
        public string Type { get; set; }
        public Guid ItemID { get; set; }
        public string ItemName { get; set; }
        public Guid? IntanceId { get; set; }
        public string WorkflowDataStr { get; set; }
        public string ItemReferenceNumberOrCode { get; set; }
        public string DataStr { get; set; }
        public string RoundNum { get; set; }
        public string ErrorLog { get; set; }
        public string Comment { get; set; }
        public string Documents { get; set; }
        public string SubSystem { get; set; }
    }
}