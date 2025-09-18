using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum ModuleType
    {
        RequestToHire = 1,
        Position = 2,
        Applicant = 3,
        Handover = 4,
        PromoteAndTransfer = 5,
        Acting = 6,
        LeaveApplication = 7,
        MissingTimeclockApplication = 8,
        OvertimeApplication = 9,
        ShiftExchangeApplication = 10,
        ResignationApplication = 11
    }
}