using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum DepartmentFilterEnum
    {
        OnlyDepartment = 1,// >= 5
        OnlyDivison = 2,// <= 4
        AllChild = 3,
    }
    public enum GetUserByDivisionEnum
    {
        InCurrentDepartment = 0,
        AllChildDepartment = 1, // Lấy những user có trong division hiện tại và child division của nó
    }
}
