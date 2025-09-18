using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.Infrastructure.Enums;
using System.IO;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ICBBO
    {
        #region missing timelock
        Task<ResultDTO> GetAllMissingTimelockByUser(QueryArgs args);
        Task<ResultDTO> GetMissingTimelockByReferenceNumber(MasterdataArgs args);
        Task<ResultDTO> SaveMissingTimelock(MissingTimelockArgs data);
        Task<ResultDTO> SubmitMissingTimelock(MissingTimelockArgs data);
        Task<ResultDTO> GetMissingTimelocks(QueryArgs args);
        #endregion
        #region Resignation Application 
        Task<ResultDTO> GetAllResignationApplicantion(QueryArgs args);
        Task<ResultDTO> CheckInProgressResignationApplicantion(string userSapCode);
        Task<ResultDTO> CheckInProgressResignationWithIsActive(string userSapCode);
        Task<ResultDTO> GetResignationApplicantionByReferenceNumber(MasterdataArgs args);
        Task<ResultDTO> SaveResignationApplicantion(ResignationApplicationArgs data);
        Task<ResultDTO> SubmitResignationApplicantion(ResignationApplicationArgs data);
        Task<ResignationApplicationViewModel> GetResignationApplicantionById(Guid id);
        Task<byte[]> PrintFormResignation(Guid Id);
        Task<List<Dictionary<string, string>>> GetWorkFlowHistories(Guid Id, ObjectToPrintFromType type, object dataToPrint = null);
        Task<ResultDTO> GetSubmittedFirstDate(Guid ItemId);
        Task<ResultDTO> CountExitInterview(Guid ItemId);
        #endregion

        #region Leave Management
        Task<ArrayResultDTO> GetListLeaveApplication(QueryArgs arg);
        Task<ResultDTO> CreateNewLeaveApplication(LeaveApplicationForCreatingArgs args);
        Task<ResultDTO> UpdateLeaveApplication(LeaveApplicationForCreatingArgs data);
        Task<ResultDTO> DeleteLeaveApplication(Guid id);
        Task<ResultDTO> CheckValidLeaveKind(ObjectToCheckValidLeaveManagemetDTO objectToCheck);
        Task<ArrayResultDTO> GetLeaveApplicantDetail(Guid Id);
        Task<ArrayResultDTO> GetLeaveApplicantDetailFromUserId(Guid userId);
        Task<ResultDTO> GetLeaveApplicationById(Guid Id);
        Task<ArrayResultDTO> GetAllLeaveManagementByUserId(Guid Id);

        Task<ResultDTO> FinalCheckValidLeaveKind(ObjectToCheckValidLeaveManagemetDTO objectToCheck);
        #endregion

        #region Overtime Application
        Task<ResultDTO> GetOvertimeApplicationList(QueryArgs query);
        Task<ResultDTO> SaveOvertimeApplication(OvertimeApplicationArgs model);
        Task<ResultDTO> UpdateOvertimeApplication(OvertimeApplicationArgs model);
        //Task<ResultDTO> GetOvertimeApplication(MasterdataArgs args);
        Task<ResultDTO> GetOvertimeApplicationById(Guid Id);
        Task<ResultDTO> ApproveOvertimeApplication(Guid Id);
        Task<ResultDTO> RequestToChangeOvertimeApplication(Guid Id);
        Task<ResultDTO> RejectOvertimeApplication(Guid Id);
        Task<ResultDTO> SubmitOvertimeApplication(Guid Id);
        Task<ResultDTO> GetOvertimeApplications(QueryArgs args);
        Task<byte[]> PrintFormOvertime(Guid Id);
        Task<ResultDTO> UploadDataForOvertime(OvertimeQuerySAPCodeArg arg, Stream stream);
        Task<ResultDTO> UploadActualDataForOvertime(OvertimeQuerySAPCodeArg arg, Stream stream);
        #endregion

        #region shift Exchange
        Task<ResultDTO> GetAllShiftExchange(QueryArgs args);
        Task<ResultDTO> GetShiftExchange(QueryArgs args, Guid currentUserId);
        Task<ResultDTO> GetShiftExchangeDetailById(Guid id);
        Task<ResultDTO> SaveShiftExchange(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId);

        // AEON_658
        Task<ResultDTO> CheckTargetPlanComplete(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId);
        Task<ResultDTO> SubmitShiftExchange(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId);

        Task<ResultDTO> GetShiftExchanges(QueryArgs args, Guid currentUserId);
        Task<ResultDTO> ValidateERDShiftExchangeDetail(List<ValidateERDShiftExchangeViewModel> shiftExhangeArgs);
        Task<ArrayResultDTO> GetCurrentShiftCodeFromShiftPlan(List<CurrentShiftArg> shiftExhangeArgs);
        #endregion
    }
}
