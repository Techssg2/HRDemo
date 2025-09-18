using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    public interface IUserService
    {
        User GetUser(string loginName);
        IList<UserDepartmentMapping> GetUserDepartmentMappingsByUserId(Guid userId);
        bool IsCheckerAcademy(Guid requestId, User user);
        bool IsHODAcademy(Guid requestId, User user);
    }
}
