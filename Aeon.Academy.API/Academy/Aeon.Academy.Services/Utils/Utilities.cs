using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services.Utils
{
    public static class Utilities
    {
        public static bool HasParentDepartmentIsAcademy(IUnitOfWork<EDocDbContext> uow, Department Department)
        {
            var IsHeadCountOfAcademy = false;
            while (true)
            {
                if (Department.ParentId.HasValue)
                {
                    var parentId = Department.ParentId.Value;
                    Department = uow.GetRepository<Department>().Query(x => x.Id == parentId).FirstOrDefault();
                    if (Department != null && Department.IsAcademy.HasValue && Department.IsAcademy.Value)
                        IsHeadCountOfAcademy = true;
                }
                else break;
            }
            return IsHeadCountOfAcademy;
        }

    }
}
