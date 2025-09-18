using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum TargetPlanType
    {
        CheckHeadCountDivision = 1, //User đc checkheadcount của phòng Division A
        CheckHeadCountDeptLine = 2, //User đc checkheadcount của phòng DeptLine A:
        SubmitDivision = 3, //User là người đc Submit của Division A
        SubmitDeptLine = 4, //User là người đc Submit của Deptline A
    }
}
