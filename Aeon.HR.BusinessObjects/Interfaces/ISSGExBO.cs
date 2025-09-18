using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ISSGExBO
    {
        Task<ResultDTO> UpdateEmployeeStatus(StatusArgs statusArgs);
        Task<ResultDTO> TeminateEmployee(string SAPCode);
        Task<ResultDTO> GetUserInFoByCode(string SAPCode);
        Task<ResultDTO> Retry(Guid Id);
        Task<ResultDTO> InserDepartment(int number);
        Task<ResultDTO> GetMultipleLeaveBalanceSet(List<string> sapCodes, int? customYear = null);
        Task<ResultDTO> UpdateUserInformationQuoteFromSAP(CommonArgs.SAP.UpdateUserInformationQuoteFromSAP args);
        Task<ResultDTO> GetLeaveBalanceSet(string sapCodes);
        Task<ResultDTO> GetLeaveBalanceSet(string sapCode, int year);
        Task<ResultDTO> GetOvertimeBalanceSet(string sapCode, Guid? OvertimeApplicationid);
        Task<ResultDTO> GetMultiOvertimeBalanceSet(List<OverTimeBalanceArgs> model);
        Task<ResultDTO> GetOTHourInMonth(List<OverTimeBalanceArgs> model);
        Task<ResultDTO> TestPushTargetPlanToSAP(string url, string payload);
        Task<ResultDTO> UpdatePayload(UpdatePayloadArgs args);
    }
}
