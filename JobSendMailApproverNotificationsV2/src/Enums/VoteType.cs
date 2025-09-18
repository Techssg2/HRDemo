using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src.Enums
{
    public enum VoteType
    {
        None = 0,
        Approve = 1,
        Reject = 2,
        RequestToChange = 3,
        Cancel = 4,
        OutOfPeriod = 5
    }
}
