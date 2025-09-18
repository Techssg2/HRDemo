using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IDashboardBO
    {
        Task<ResultDTO> GetMyItems();
        ResultDTO GetEmployeeNodesByDepartment(Guid departmentId, int maxLvl = 3);
        ResultDTO ClearNode();
    }
}
