using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IRecruitmentBO
    {
        #region RequestToHire
        Task<ArrayResultDTO> GetListRequestToHires(QueryArgs args);
        Task<ResultDTO> GetListDetailRequestToHire(RequestToHireDataForCreatingArgs data);
        Task<ResultDTO> SearchListRequestToHire(RequestToHireDataForCreatingArgs data);
        Task<ResultDTO> CreateRequestToHire(RequestToHireDataForCreatingArgs data);
        Task<ResultDTO> DeleteRequestToHire(RequestToHireDataForCreatingArgs data);
        Task<ResultDTO> UpdateRequestToHire(RequestToHireDataForCreatingArgs data);
        Task<ResultDTO> GetResignationApplicantionCompletedBySapCode(string sapcode);
        Task<byte[]> DownloadTemplateImportRequestToHire(DataToImportRequestToHireTemplateArgs args);
        Task<ResultDTO> UploadData(ImportRequestToHireArg arg, Stream stream);
        Task<ResultDTO> AutoImportRequestToHire(DataToImportRequestToHireTemplateArgs arg);
        Task<ResultDTO> InserData(RequestToHireFromImportFileDTO dataFromFile, ImportRequestToHireArg arg);
        Task<ArrayResultDTO> GetImportTracking(TrackingImportArgs args);
        #endregion
        #region Handover
        Task<ArrayResultDTO> GetListHandovers(QueryArgs args);
        Task<ResultDTO> GetListDetailHandover(HandoverDataForCreatingArgs data);
        Task<ResultDTO> SearchListHandover(HandoverDataForCreatingArgs data);
        Task<ResultDTO> CreateHandover(HandoverDataForCreatingArgs data);
        Task<ResultDTO> DeleteHandover(HandoverDataForCreatingArgs data);
        Task<ResultDTO> UpdateHandover(HandoverDataForCreatingArgs data);
        Task<ArrayResultDTO> GetHandovers(QueryArgs args);
        #endregion
        #region Position
        Task<ArrayResultDTO> GetAllListPositionForFilter();
        Task<ArrayResultDTO> GetOpenPositions();
        Task<ArrayResultDTO> GetListPosition(QueryArgs arg);
        Task<ArrayResultDTO> GetListPositionDetail(QueryArgs arg);
        Task<ResultDTO> CreateNewPosition(PositionForCreatingArgs model);
        Task<ResultDTO> UpdatePosition(PositionForCreatingArgs model);
        Task<ResultDTO> DeletePosition(Guid id);
        Task<ResultDTO> ChangeStatus(PositionStatusArgs arg);
        Task<ArrayResultDTO> GetPositionMappingApplicant(QueryArgs arg);
        Task<ArrayResultDTO> GetPositionForActing(QueryArgs arg);
        Task<ResultDTO> ReAssigneeAsync(ReAssigneeInPositionArgs args);
        Task<ResultDTO> SendEmailToAssignee(ILogger logger, Guid? assigneeId, Guid positionId);
        Task<ResultDTO> GetPositionById(Guid id);
        Task<ResultDTO> GetPositionByDepartmentId(Guid deptId);
        #endregion
        #region Applicant
        Task<ArrayResultDTO> GetApplicantList(QueryArgs arg);
        Task<ArrayResultDTO> GetSimpleApplicantList(QueryArgs arg);
        Task<ResultDTO> SearchApplicantList(ApplicantSearchArgs query);
        Task<ResultDTO> CreateApplicant(ApplicantArgs data);
        Task<ResultDTO> DeleteApplicant(Guid id);
        Task<ResultDTO> UpdateApplicant(ApplicantArgs data);
        Task<ResultDTO> SubmitApplicant(Guid Id);
        Task<ResultDTO> SearchApplicant(QueryArgs args);
        Task<ResultDTO> UpdatePositionDetail(PositionDetailItemArgs data);
        Task<ResultDTO> GetDetailApplicant(Guid id);
        #endregion
        #region acting
        Task<ResultDTO> CreateActing(MasterActingArgs arg);
        Task<ResultDTO> GetActings(QueryArgs arg);
        Task<ResultDTO> GetActingByReferenceNumber(ActingRequestArgs arg);
        Task<byte[]> PrintFormActing(Guid Id);
        #endregion
        #region New Staff On Board
        Task<ResultDTO> GetAllNewStaffOnboard(QueryArgs arg);
        Task<ResultDTO> UpdateStatusNewStaffOnBoard(UpdateNewStaffOnBoardArgs args);
        #endregion
        #region Promote And Transfer
        Task<ArrayResultDTO> GetListPromoteAndTransfers(QueryArgs args);
        Task<ResultDTO> GetPromoteAndTransferById(PromoteAndTransferDataForCreatingArgs data);
        Task<ResultDTO> SearchListPromoteAndTransfers(PromoteAndTransferDataForCreatingArgs data);
        Task<ResultDTO> CreatePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data);
        Task<ResultDTO> DeletePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data);
        Task<ResultDTO> UpdatePromoteAndTransfer(PromoteAndTransferDataForCreatingArgs data);
        Task<byte[]> PrintForm(Guid Id);
        Task<byte[]> PrintRequestToHire(Guid Id);

        #endregion
        #region Tracking Log
        Task<ArrayResultDTO> GetListTrackingLog(QueryArgs arg);
        Task<ResultDTO> SaveTrackingLog(TrackingLogArgs data);
        #endregion
        Task<List<Dictionary<string, string>>> GetWorkFlowHistories(Guid Id, ObjectToPrintFromType type, object dataToPrint = null);
    }
}
