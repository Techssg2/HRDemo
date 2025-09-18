using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum HalfDayOption
    {
        [Description("HaftDay1")]
        HaftDay1 = 1, // là nghỉ buổi sáng
        [Description("HaftDay2")]
        HaftDay2 = 2 //  là nghỉ buổi chiều
    }
}