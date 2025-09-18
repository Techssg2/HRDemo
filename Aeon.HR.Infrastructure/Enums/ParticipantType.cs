using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum ParticipantType
    {
        SpecificUser = 0,
        SpecificDepartment = 1,
        CurrentDepartment = 2,
        UpperDepartment = 3,
        //LowerDepartment = 4,
        DepartmentLevel = 5,
        ItemUserField = 6,
        ItemDepartmentField = 7,
        HRDepartment = 8,
        PerfomanceDepartment = 9,
        CBDepartment = 10,
        AdminDepartment = 11,
        HRManagerDepartment = 12,
        Appraiser1Department = 13,
        Appraiser2Department = 14,
        BTABudgetApprover = 15
    }
}