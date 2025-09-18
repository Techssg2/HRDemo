using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class PermissionBO : IPermissionBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        public PermissionBO(ILogger logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public async Task<Right> GetItemPerm(Guid itemId)
        {
            var perms = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == itemId
            && (x.UserId == _uow.UserContext.CurrentUserId
                || x.Department.UserDepartmentMappings.Any(t => t.UserId == _uow.UserContext.CurrentUserId && t.Role == x.DepartmentType && !t.IsDeleted)));
            var right = Right.None;
            foreach (var perm in perms)
            {
                right |= perm.Perm;
            }
            return right;
        }
    }
}
