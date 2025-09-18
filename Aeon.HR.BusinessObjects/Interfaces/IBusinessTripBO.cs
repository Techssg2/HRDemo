using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.BTA;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface IBusinessTripBO
    {
        Task<ResultDTO> GetItemById(Guid id);
        Task<BusinessTripApplication> GetById(Guid id);
        Task<ResultDTO> GetList(QueryArgs arg);
        Task<ResultDTO> Save(BusinessTripDTO data);
        Task<ResultDTO> SaveRevokeInfo(RevokeBTAInfoViewModel data);
        Task<ResultDTO> SaveCancellationFee_Revoke(RevokeBTAInfoViewModel data);
        Task<ResultDTO> SaveCancellationFee_Changing(RevokeBTAInfoViewModel data);
        Task<ResultDTO> SaveRoomOrganization(Guid id, string roomDetails, bool isChange = false);
        Task<ResultDTO> GetReports(ReportType type, QueryArgs args);
        ResultDTO GetUserTicketsInfo(Guid BTADetailId);
        Task<ResultDTO> GetDetailUsersInRoom(ReportType type, string RoomTypeCode, QueryArgs args);
        Task<ResultDTO> GetTripGroups(Guid Id);
        Task<ResultDTO> Validate(BusinessTripValidateArg arg);
        Task<ResultDTO> ExportReport(ReportType type, ViewModels.Args.ExportReportArg args);
        Task<ResultDTO> ExportTypeStatus(ReportType type, ViewModels.Args.ExportReportArg args);
        Task<byte[]> PrintForm(Guid Id);
        Task<ResultDTO> GetRevokingBTA(ViewModels.Args.RevokingArg args);
        Task<ResultDTO> SavePassengerInfo(List<BTAPassengerViewModel> btaPassengerInfoArray);
        Task<ResultDTO> SaveBookingInfo(List<BookingFlightViewModel> btaPassengerInfoArray);
        Task<ResultDTO> GetPassengerInformationBySAPCodes(List<string> btaPassengerSAPCodeArray);

        Task<ResultDTO> GetDetailItemById(Guid id);
        Task<ResultDTO> GetEmailSubmitter(Guid id);
        Task<ResultDTO> GetInfoAdmin();
        // lamnl
        Task<ResultDTO> GetBtaRoomHotel(Guid id);
        ResultDTO CheckBookingCompleted(BtaDTO agrs);
        Task<ResultDTO> CheckAdminDept(BTAAdminDeptDTO agrs);
        Task<ResultDTO> SaveBTADetail(BusinessTripDTO data);
        Task<ResultDTO> UpdateBeforeCommitBookingForBTADetail(List<BusinessTripApplicationDetail> data, string bookingNumber, string bookingCode, bool isCommit);
        Task<bool> CheckCommitBookingForBTADetail(List<BusinessTripApplicationDetail> data);
        Task<bool> CheckHasBookingNumberForBTADetail(List<BusinessTripApplicationDetail> data);
        bool SaveBTALog(Guid? btaID, string message);
        Task<ResultDTO> SendEmailDeleteRows(BtaDTO agrs);
        Task<ResultDTO> CheckRoomHotel(BtaDTO Agrs);
        Task<ResultDTO> DeleteTripGroup(CommonDTO agrs);
        Task<ResultDTO> SaveRoomBookingFlight(Guid id, string roomDetails, int tripGroup, bool isChange = false);
        Task<ResultDTO> GetFareRulesByFareSourceCode(ViewModels.Args.FareRulesRequestArgs args);
        Task<string> GetBTAApprovedDay(Guid businessTripApplicationId);
        Task<ResultDTO> ValidationBTADetails(ViewModels.Args.ValidateBTADetailsArgs btaArgs);
        Task<BTAErrorMessageViewModel> GetBTAErrorMessageByCode(BTAErrorEnums type, string errorCode);
    }
}
