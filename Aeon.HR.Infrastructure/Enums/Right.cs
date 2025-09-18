using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    [Flags]
    public enum Right
    {
        None = 0,
        View = 1,
        Edit = 2,
        Delete = 4,
        Full = View | Edit | Delete
    }
}