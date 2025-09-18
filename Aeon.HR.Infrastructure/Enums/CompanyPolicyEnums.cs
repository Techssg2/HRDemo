using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum CompanyPolicyEnums
    {
        [Description("Password Expired After")]
        PASSWORD_EXPIRED_AFTER = 1,
        [Description("Minimum Length")]
        MINUMUM_LENGTH = 2,
        [Description("Password cannot match")]
        PASSWORD_CANNOT_MATCH = 3
    }
}
