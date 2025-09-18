using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ISharePointBO
    {
        Task<bool> AssignUser(string loginName);
        Task<bool> RemoveUser(string loginName);
    }
}
