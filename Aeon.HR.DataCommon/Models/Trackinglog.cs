using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aeon.HR.Infrastructure.Abstracts;
namespace Aeon.HR.Data.Models
{
    public class TrackingLog : AuditableEntity
    {
        public string OldAssignee { get; set; }
        public string NewAssignee { get; set; }
        public Guid? ItemId { get; set; }
    }
}
