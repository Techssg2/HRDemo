using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
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