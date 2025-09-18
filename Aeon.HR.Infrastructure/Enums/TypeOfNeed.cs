using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum TypeOfNeed
    {
        [Description("New Position")]
        NewPosition = 1,
        [Description("Replacement For")]
        ReplacementFor = 2
    }
}
