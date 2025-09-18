using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum DepartmentType
    {
        [Description("Division")]
        Division = 1,
        [Description("Department")]
        Department = 2,

    }
}