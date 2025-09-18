using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ITargetPlanBO
    {
        Task<ResultDTO> SavePendingTargetPlan(bool isSend, TargetPlanArg arg);
        Task<ResultDTO> SavePendingBeforeSubmit(TargetPlanArg arg);
        Task<ResultDTO> SendRequest_TargetPlan(bool isSend, TargetPlanArg arg);
        Task<ResultDTO> GetPendingTargetPlanDetails(Guid id);
        Task<ResultDTO> GetValidUserSAPForTargetPlan(Guid id, Guid userId, Guid periodId);
        Task<ResultDTO> GetPendingTargetPlanFromDepartmentAndPeriod(TargetPlanQueryArg args);
        Task<ResultDTO> GetPendingTargetPlanDetailFromSAPCodesAndPeriod(TargetPlanQuerySAPCodeArg args);
        Task<ResultDTO> CancelPendingTargetPLan(CancelPendingTargetArg args);
        Task<ResultDTO> RequestToChange(TargetPlanRequestToChangeArg arg);
        //code dưới đang dư -> comment lại tính sau
        //Task<ResultDTO> GetTargetPlanFromDepartment(Guid departmentId);
        Task<ResultDTO> GetTargetPlanPeriods();
        Task<ResultDTO> GetActualShiftPlan(ActualTargetPlanArg args);
        Task<ResultDTO> UpdatePermissionUserInTargetPlanItem(Guid id);
        // not used
        //Task<ResultDTO> SubmitTargetPlanToSAP(ActualTargetPlanArg args);
        Task<ResultDTO> GetTargetPlanDetailIsCompleted(TargetPlanDetailQueryArg args);
        Task<ResultDTO> Submit(SubmitDataArg args);
        Task<ResultDTO> SetSubmittedStateForDetailPendingTargetPlan(SubmitDetailPendingTartgetPlanSAPArg arg);
        Task<ResultDTO> GetList(QueryArgs arg);
        Task<ResultDTO> GetItem(Guid id);
        Task<ResultDTO> GetSAPCode_PendingTargetPlanDetails(Guid id);
        Task<ResultDTO> ValidateTargetPlan(TargetPlanArg arg);
        Task<ResultDTO> ValidateTargetPlanV2(TargetPlanArg arg);
        Task<ResultDTO> UploadData(TargetPlanQuerySAPCodeArg arg, Stream stream);
        Task<ResultDTO> GetPermissionInfoOnPendingTargetPlan(TargetPlanQueryPermissionInfo arg);
        Task<ResultDTO> GetPendingTargetPlans(QueryArgs arg);
        Task<byte[]> PrintForm(TargetPlanDetailQueryArg args);
        Task<byte[]> DownloadTemplate(DataToPrintTemplateArgs args);
        Task<ResultDTO> TargetPlanReport(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false);
        Task<ResultDTO> ValidateSubmitPendingTargetPlan(ValidateExistTargetPlanArgs args);
        #region DWS Create Target
        Task<ResultDTO> CreateTargetPlan_API(CreateTargetPlan_APIArgs agrs);
        Task<ResultDTO> ValidateForEditDWS_API(CreateTargetPlan_APIArgs agrs);
        #endregion
    }
}
