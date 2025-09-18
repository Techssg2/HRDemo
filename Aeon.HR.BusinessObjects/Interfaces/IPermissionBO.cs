using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IPermissionBO
    {
        Task<Right> GetItemPerm(Guid itemId);
    }
}
