using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum ActionExposeAPI
    {
        [Description("Send")]
        Send = 0,
        [Description("Receive")]
        Receive = 1
    }
}
