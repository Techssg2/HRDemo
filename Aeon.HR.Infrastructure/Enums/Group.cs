using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum Group
    {
        HOD = 1, // Head of Department
        Checker = 2, // Checker 
        Member = 4, // Normal Employee,
        Assistance = 8,
        All = HOD | Checker | Member
    }
}