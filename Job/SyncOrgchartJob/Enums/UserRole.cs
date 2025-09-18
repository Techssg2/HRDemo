using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum UserRole
    {
        None = 0,
        SAdmin = 1, // Super Admin  
        Admin = 2, // Admin who allow support Setting
        HR = 4, // View on HR Features
        CB = 8, // View on C&B features
        Member = 16, // View on Items which has permission,
        HRAdmin = 32,
        Accounting = 64,
        ITHelpDesk = 128
    }
}