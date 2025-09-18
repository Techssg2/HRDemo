using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using EFSecondLevelCache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.DataHandlers
{
    public static class DbHelper
    {
        private const int MAX_STORE_LEVEL = 5;
        public static Department LoadDept(DbContext ctx, Guid userId)
        {
            try
            {
                var department = ctx.Set<Department>().Include(x => x.JobGrade).Where(x => !x.IsDeleted && x.UserDepartmentMappings.Any(t => t.UserId == userId && t.IsHeadCount && t.Role == Group.Member)).Cacheable().FirstOrDefault();
                ///while (department != null && department.ParentId.HasValue && department.JobGrade.Grade < MAX_STORE_LEVEL)
                while (department != null && department.ParentId.HasValue && department.Type == DepartmentType.Division)
                {
                    var lastDept = ctx.Set<Department>().Include(x => x.JobGrade).Where(x => x.Id == department.ParentId && !x.IsDeleted).Cacheable().FirstOrDefault();
                    if (lastDept != null)
                    {
                        department = lastDept;
                    }
                }
                return department;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Department LoadDept(IUnitOfWork _uow, Guid deptId)
        {
            try
            {
                var department = _uow.GetRepository<Department>().FindById(deptId);
                //while (department != null && department.ParentId.HasValue && department.JobGrade.Grade < MAX_STORE_LEVEL)
                while (department != null && department.ParentId.HasValue && department.Type == DepartmentType.Division)
                {
                    var lastDept = _uow.GetRepository<Department>().GetSingle(x => x.Id == department.ParentId && !x.IsDeleted);
                    if (lastDept != null)
                    {
                        department = lastDept;
                    }
                }
                return department;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
