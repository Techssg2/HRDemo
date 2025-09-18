using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using static Aeon.HR.ViewModels.Args.CommonArgs.User;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IAPIEdoc2BO
    {
        Task<ResultDTO> UpdateStatusBTA(CommonDTO agrs);
        Task<ResultDTO> GetItemByReferenceNumber(CommonDTO agrs);
        Task<ResultDTO> GetAllBTA_API(string ReferenceNumber, string Limit);
        Task<ResultDTO> GetDepartmentTree_API();
        Task<ArrayResultDTO> GetAllUser_API();
        Task<ArrayResultDTO> GetAllUser_APIV2(IntergationAPI args);
        Task<ResultDTO> GetSpecificShiftPlan_API(ShiftPlanAPIArgs args);
        Task<ResultDTO> GetActualShiftPlan_API(IntergationAPI args);
        Task<ResultDTO> GetActualShiftPlanForDWS_API(IntergationAPI args);
        Task<ResultDTO> AccountVerification_API(string username, string password);
        Task<ResultDTO> GetUsersForTargetPlanByDeptIdDWS(UserForDWSArg arg);
    }
}
