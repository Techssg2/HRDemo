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
    public interface ISAPBO
    {
        Task<ResultDTO> SearchEmployee(string SAPCode);
        Task<ResultDTO> GetMasterData(RemoteMasterDataDetailInformation arg);
        Task<string> CheckValidEmployeeFromSAP(string[] keys);
        Task<ResultDTO> GetUsers(UserSAPArg arg);
        Task<ResultDTO> GetMasterDataEmployeeList(string arg);
        Task<ResultDTO> GetNewWorkLocationList(string arg);
        Task<ResultDTO> GetNewWorkLocationListV2(string newWorkLocationCode);
    }
}
