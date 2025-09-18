using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class TrackingLogArgs
    {
        public Guid Id { get; set; }
        public string OldAssignee { get; set; }
        public string NewAssignee { get; set; }
        public Guid? ItemId { get; set; }
    }
}
