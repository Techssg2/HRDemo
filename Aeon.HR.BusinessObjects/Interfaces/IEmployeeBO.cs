using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IEmployeeBO
    {
        Task<ResultDTO> GetFullEmployeeInfo(string SAPCode);
        Task<DateTime?> GetJoiningDateOfEmployee(string SAPCode);
        Task<ResultDTO> GetFullEmployeeInfo(Guid userId, bool isActive = true);
        Task<ResultDTO> GetUsers(UserSAPArg arg);
        Task<ResultDTO> GetMasterDataEmployeeList(string arg);
        Task<ResultDTO> GetNewWorkLocationList(string arg);
        Task<ResultDTO> GetNewWorkLocationListV2(string newWorkLocationCode);
        Task<EmployeeViewModel> GetUserProfileAdditionData(EmployeeViewModel employeeInfo);
    }
}
