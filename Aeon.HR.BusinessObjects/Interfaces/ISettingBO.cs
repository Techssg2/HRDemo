using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aeon.HR.Data.Models;
using System;
using Aeon.HR.Infrastructure.Enums;
using static Aeon.HR.BusinessObjects.Handlers.SettingBO;
using System.IO;
using TargetPlanTesting.ImportData;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    public interface ISettingBO
    {
        Task<ResultDTO> GetRegionList(QueryArgs args);
        #region Users
        Task<ArrayResultDTO> GetListUsers(QueryArgs args);
        Task<ArrayResultDTO> GetChildUsers(Guid departmentId, int limit, int page, string searchText, bool isAll = false);
        Task<ArrayResultDTO> GetUsersForReportTargetPlan(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false);
        Task<ArrayResultDTO> GetUsersByOnlyDeptLine(Guid depLineId, int limit, int page, string searchText, bool isAll = false);
        Task<ArrayResultDTO> GetUsersByDeptLines(CommonArgs.Department.GetUsersByDeptLines args, bool isAll = false);
        Task<ArrayResultDTO> GetUsersByDivision(Guid? departmentId, GetUserByDivisionEnum type, string currentSapCode);
        Task<ResultDTO> SAPCreateUser(SAPUserDataForCreatingArgs data);
        Task<ResultDTO> CreateUser(UserDataForCreatingArgs data);
        Task<ResultDTO> Update(UserDataForCreatingArgs data);
        Task<ResultDTO> ChangeStatus(Guid userId, bool isActivated);
        Task<ResultDTO> LockUserMembership(Guid userId, bool isActivated, string lockType);
        Task<ResultDTO> CheckLockUser(Guid userId);
        Task<ResultDTO> ResetPassword(Guid userId);
        Task<ResultDTO> GetUserById(UserInfoCABArg args);
        Task<ResultDTO> GetUsers();
        Task<ResultDTO> GetCurrentUser();
        Task<ResultDTO> GetCurrentUserV2();
        Task<ResultDTO> GetLinkImageUserByLoginName(string loginName);
        Task<ResultDTO> UpdateImageUser(UserDataForCreatingArgs data);
        Task<ResultDTO> GetImageUserById(UserInfoCABArg data);
        Task<ResultDTO> ChangePassword(ChangePasswordArgs args);
        Task<ResultDTO> ForgotPassword(string username, string email);
        Task ActiveAllUser(string password);
        Task<ResultDTO> FindUserForDataInvalid(UserForCheckDataArg args);
        Task<ResultDTO> GetUserProfileDataById(UserInfoCABArg args);
        Task<ResultDTO> GetUserProfileCustomById(UserInfoCABArg args);
        Task<ArrayResultDTO> GetUserCheckedHeadCount(Guid departmentId, string textSearch);
        Task<ResultDTO> CheckUserIsStore(string departmentCode);
        Task<ArrayResultDTO> GetUsersForTargetPlanByDeptId(UserForTargetArg args);
        Task<ArrayResultDTO> GetUsersByListSAPs(string sapCodes);
        Task<List<string>> GetValidUsersForSubmitTargetPlan(UserForTargetArg arg);
        Task<ResultDTO> CheckUserBySAPCode(string sapCode);
        Task<ArrayResultDTO> GetAllUsers();
        Task<ArrayResultDTO> GetAllUsersByKeyword(CommonArgs.Member.User.GetAllUserByKeyword args);
        #endregion
        #region recruitmentSetting - Categories
        Task<ResultDTO> GetRecruitmentCategories(QueryArgs arg);
        Task<ResultDTO> SearchRecruitmentCategories(MasterdataArgs arg);
        Task<ResultDTO> CreateRecruitmentCategory(RecruitmentCategoryArgs data);
        Task<ResultDTO> DeleteRecruitmentCategory(RecruitmentCategoryArgs data);
        Task<ResultDTO> UpdateRecruitmentCategory(RecruitmentCategoryArgs data);
        #endregion

        #region recruitmentSetting - WorkingTime
        Task<ResultDTO> GetWorkingTimeRecruiments(QueryArgs arg);
        Task<ResultDTO> SearchWorkingTimeRecruiment(MasterdataArgs arg);
        Task<ResultDTO> CreateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data);
        Task<ResultDTO> DeleteWorkingTimeRecruitment(WorkingTimeRecruimentArgs data);
        Task<ResultDTO> UpdateWorkingTimeRecruitment(WorkingTimeRecruimentArgs data);
        Task<ResultDTO> GetWorkingTimeRecruimentByCode(QueryArgs arg);
        #endregion

        #region recruitmentSetting - ItemList
        Task<ResultDTO> GetItemListRecruiments(QueryArgs arg);
        Task<ResultDTO> SearchItemListRecruimentByName(MasterdataArgs arg);
        Task<ResultDTO> CreateItemListRecruitment(ItemListViewRecruitmentArgs data);
        Task<ResultDTO> DeleteItemListRecruitment(ItemListViewRecruitmentArgs data);
        Task<ResultDTO> UpdateItemListRecruitment(ItemListViewRecruitmentArgs data);
        Task<ResultDTO> GetItemListByCode(QueryArgs arg);
        Task<ResultDTO> GetDepartmentByCode(string deptCode);
        Task<ArrayResultDTO> GetAllListDepartments();
        #endregion

        #region recruitmentSetting - ApplicantStatus
        Task<ResultDTO> GetApplicantStatusRecruiments(QueryArgs arg);
        Task<ResultDTO> SearchApplicantStatusRecruimentByName(MasterdataArgs arg);
        Task<ResultDTO> CreateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data);
        Task<ResultDTO> DeleteApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data);
        Task<ResultDTO> UpdateApplicantStatusRecruitment(ApplicantStatusRecruitmentArgs data);
        Task<ResultDTO> GetAllApplicantStatusRecruiments();
        Task<ResultDTO> GetApplicantStatusRecruitmentByCode(QueryArgs arg);
        #endregion

        #region recruitmentSetting - AppreciationList
        Task<ResultDTO> GetAppreciationListRecruiments(QueryArgs arg);
        Task<ResultDTO> SearchAppreciationListRecruimentByName(MasterdataArgs arg);
        Task<ResultDTO> CreateAppreciationListRecruitment(AppreciationListRecruitmentArgs data);
        Task<ResultDTO> DeleteAppreciationListRecruitment(AppreciationListRecruitmentArgs data);
        Task<ResultDTO> UpdateAppreciationListRecruitment(AppreciationListRecruitmentArgs data);
        Task<ResultDTO> GetAppreciationListRecruimentByCode(QueryArgs arg);
        #endregion

        #region Department management
        Task<ResultDTO> CreateDepartment(DepartmentArgs department);
        Task<ResultDTO> UpdateDepartment(DepartmentArgs department);
        Task<ResultDTO> DeleteDepartment(Guid Id);
        Task<ResultDTO> GetDepartmentById(Guid Id);
        Task<ResultDTO> GetDepartments(QueryArgs args);
        Task<ResultDTO> GetDetailDepartment(DepartmentArgs data);
        Task<ResultDTO> GetDepartmentTree();
        Task<ResultDTO> GetDepartmentTreeByGrade(Guid jobGradeId);
        Task<ResultDTO> GetDepartmentTreeByType(Guid Id, DepartmentFilterEnum option);
        Task<bool> HasDepartmentCircle(Guid Id, Guid parentId);

        Task<ResultDTO> GetUserInDepartment(Guid departmentId);
        Task<ResultDTO> RemoveUserFromDepartment(Guid Id);
        Task<ResultDTO> RemoveUserFromDepartmentByUser(Guid userId);
        Task<ResultDTO> UpdateUserInDepartment(UpdateUserDepartmentMappingArgs model);
        Task<ResultDTO> AddUserToDepartment(UserInDepartmentArgs model);
        Task<ResultDTO> GetEmployeeCodes();
        Task<ResultDTO> MoveHeadCountUpdate(UpdateUserDepartmentMappingArgs model);
        Task<ResultDTO> MoveHeadCountAdd(UserInDepartmentArgs model);
        Task<ResultDTO> GetAllDeptLineByGrade(Guid jobGradeId);
        Task<ResultDTO> GetDepartmentByFilter(QueryArgs arg);
        Task<ResultDTO> GetDepartmentByReferenceNumber(string prefix, Guid itemId);
        Task<ResultDTO> GetDepartmentsByUserId(Guid userId);
        Task<ResultDTO> GetDepartmentUpToG4ByUserId(Guid userId);
        Task<ResultDTO> GetDepartmentByArg(QueryArgs args);
        //Task<ResultDTO> GetDivisionByFilter(DivisionSearchByDeptLine arg);
        Task<ResultDTO> GetAllDepartmentsByPositonName(string posiontionName);
        Task<ArrayResultDTO> GetDepartmentsByUserKeyword(CommonArgs.Member.User.GetAllUserByKeyword args);
        Task<ResultDTO> GetDepartmentByFilterV2(QueryArgs arg);
        bool ResetAllDepartmentCache();

        #endregion
        #region AdminHistory
        //Admin History
        //Task<ResultDTO> CreateAdminHistory(UserDepartmentMapping userdepartment, string action);
        //Task<ResultDTO> CreateAdminHistory(UserDataForCreatingArgs user, string action);
        //Task<ResultDTO> CreateAdminHistory(Department department, string action);
        #endregion

        #region C&B setting
        Task<ResultDTO> SearchReason(string searchString, string type);
        Task<ResultDTO> GetReasons(MasterdataArgs args);
        Task<ResultDTO> AddReason(object args);
        Task<ResultDTO> UpdateReason(object args);
        Task<ResultDTO> DeleteReason(object args);
        Task<ResultDTO> GetAllReason(MetadataTypeArgs arg);
        //ngan
        Task<ResultDTO> GetHolidaySchedules(QueryArgs arg);
        Task<ResultDTO> CreateHolidaySchedule(HolidayScheduleArgs data);
        Task<ResultDTO> UpdateHolidaySchedule(HolidayScheduleArgs data);
        Task<ResultDTO> DeleteHolidaySchedule(HolidayScheduleArgs data);
        Task<ResultDTO> GetYearHolidays();
        //end
        Task<ResultDTO> GetDataShiftCode(QueryArgs arg);
        Task<ResultDTO> CreateShiftCode(ShiftCodeAgrs data);
        Task<ResultDTO> UpdateShiftCode(ShiftCodeAgrs data);
        Task<ResultDTO> DeleteShiftCode(ShiftCodeAgrs data);
        Task<byte[]> DownloadTemplate();
        Task<ResultDTO> UploadData(ShiftCodeAgrs arg, Stream stream);
        #endregion

        #region HeadCount Setting
        Task<ResultDTO> CreateHeadCount(HeadCountArgs model);
        Task<ResultDTO> UpdateHeadCount(HeadCountArgs model);
        Task<ResultDTO> DeleteHeadCount(Guid Id);
        Task<ResultDTO> GetHeadCountList(QueryArgs args);
        Task<ResultDTO> GetDepartmentListForHeadCount();
        Task<ResultDTO> GetHeadCountByDepartmentId(Guid id, int jobGrade);

        #endregion

        #region ReferencyNumber
        Task<ResultDTO> GetReferencyNumberRecruiments();
        //Task<ResultDTO> SearchReferencyNumberRecruimentsByName(MasterdataArgs arg);
        Task<ResultDTO> UpdateReferencyNumberRecruitment(ReferencyNumberArgs data);
        #endregion

        #region JobGrade
        Task<ResultDTO> GetJobGradeList(QueryArgs args);
        Task<ResultDTO> UpdateJobGrade(JobGradeArgs model);
        Task<ResultDTO> DeleteJobGrade(Guid Id);
        Task<ResultDTO> CreateJobGrade(JobGradeArgs args);

        Task<ResultDTO> AddOrUpdateItemsOfJobGrade(JobGradeForAddOrUpdateItemViewModel jobGradeItemViewModel);
        Task<ResultDTO> GetItemRecruitmentsOfJobGrade(Guid jobGradeId);
        Task<ResultDTO> GetAllItemRecruitments();
        Task<ResultDTO> GetJobGradeById(Guid id);
        Task<ResultDTO> GetItemRecruitmentsByJobGradeId(Guid jobGradeId);
        Task<ResultDTO> GetJobGradeByJobGradeValue(int jobGradeValue); //===== CR11.2 =====
        #endregion

        #region MasterDataApplicant
        Task<ResultDTO> GetMasterDataApplicantList(QueryArgs args);
        #endregion

        #region Tracking request // Tracking log
        Task<ResultDTO> GetTrackingRequest(QueryArgs args);
        Task<ResultDTO> UpdatePayloadById(EditPayloadArgs args);
        #endregion

        #region Log history
        Task<ResultDTO> SaveLogHistory(LogHistoryViewModel args);
        Task<ResultDTO> GetLogHistory(QueryArgs args);
        #endregion
        #region recruitmentSetting - Position
        Task<ResultDTO> GetPositionRecruiments(QueryArgs arg);
        Task<ResultDTO> CreatePositionRecruitment(PositionRecruitmentArgs data);
        Task<ResultDTO> DeletePositionRecruitment(PositionRecruitmentArgs data);
        Task<ResultDTO> UpdatePositionRecruitment(PositionRecruitmentArgs data);
        Task<ResultDTO> GetPositionRecruimentByCode(QueryArgs arg);
        #endregion

        #region recruitmentSetting - CostCenter
        Task<ResultDTO> GetCostCenterRecruiments(QueryArgs arg);
        Task<ResultDTO> GetCostCenterRecruitmentByCode(QueryArgs arg);
        Task<ResultDTO> CreateCostCenterRecruitment(CostCenterRecruitmentArgs data);
        Task<ResultDTO> DeleteCostCenterRecruitment(CostCenterRecruitmentArgs data);
        Task<ResultDTO> UpdateCostCenterRecruitment(CostCenterRecruitmentArgs data);
        Task<ResultDTO> GetCostCenterByDepartmentId(Guid id);
        #endregion

        #region Shift plan submit person
        Task<ResultDTO> GetShiftPlanSubmitPersons(QueryArgs args);
        Task<ResultDTO> GetShiftPlanSubmitPersonById(Guid departmentId);
        Task<ResultDTO> CreateShiftPlanSubmitPerson(ShiftPlanSubmitPersonArg arg);
        Task<ResultDTO> DeleteShiftPlanSubmitPerson(Guid departmentId);
        Task<ResultDTO> CheckDepartmentExist(Guid departmentId);
        Task<ResultDTO> SaveShiftPlanSubmitPerson(ShiftPlanSubmitPersonArg arg);
        Task<ResultDTO> GetDepartmentTargetPlansByUserId(Guid userId);
        Task<ResultDTO> GetDepartmentTargetPlans(Guid userId);
        Task<ResultDTO> FilterDivisionByUserNotSubmit(DepartmentTargetPlanViewModel arg);
        Task<ResultDTO> CheckIsSubmitPersonOfDepartment(Guid divisionId, string currentUserSapCode);
        Task<ResultDTO> CheckSubmitPersonFromDepartmentId(Guid departmentId);

        #endregion
        #region Airline
        Task<ResultDTO> GetAirlines(QueryArgs arg);
        Task<ResultDTO> SaveAirline(AirlineArg data);
        Task<ResultDTO> DeleteAirline(Guid id);
        Task<ResultDTO> GetAirlineById(Guid id);
        Task<ResultDTO> CheckValidateCode(AirlineArg arg);
        Task<ResultDTO> ValidateWhenFlightNumberUsed(Guid id);
        #endregion

        #region Flight Number
        Task<ResultDTO> GetFlightNumbers(QueryArgs arg);
        Task<ResultDTO> SaveFlightNumber(FlightNumberArg data);
        Task<ResultDTO> DeleteFlightNumber(Guid id);
        Task<ResultDTO> GetFlightNumberById(Guid id);
        Task<ResultDTO> CheckValidateFlightNumberCode(FlightNumberArg arg);
        #endregion

        #region BusinessTripLocationArgs
        Task<ResultDTO> GetListBusinessTripLocation(QueryArgs arg);
        Task<ResultDTO> CreateBusinessTripLocation(BusinessTripLocationArgs data);
        Task<ResultDTO> DeleteBusinessTripLocation(BusinessTripLocationArgs Id);
        Task<ResultDTO> UpdateBusinessTripLocation(BusinessTripLocationArgs data);
        Task<ResultDTO> GetBusinessTripLocationByCode(QueryArgs arg);
        #endregion

        #region Hotel
        Task<ResultDTO> GetListHotels(QueryArgs arg);
        Task<ResultDTO> CreateHotel(HotelArgs data);
        Task<ResultDTO> DeleteHotel(HotelArgs data);
        Task<ResultDTO> UpdateHotel(HotelArgs data);
        Task<ResultDTO> GetHotelByCode(QueryArgs arg);
        #endregion

        #region RoomType
        Task<ResultDTO> GetListRoomTypes(QueryArgs arg);
        Task<ResultDTO> CreateRoomType(RoomTypeArgs data);
        Task<ResultDTO> DeleteRoomType(RoomTypeArgs Id);
        Task<ResultDTO> UpdateRoomType(RoomTypeArgs data);
        Task<ResultDTO> GetRoomTypeByCode(QueryArgs arg);
        #endregion

        #region days configuration
        Task<ResultDTO> GetSalaryDayConfigurations(QueryArgs arg);
        Task<ResultDTO> SaveSalaryDayConfiguration(SalaryDayConfigurationArg data);
        #endregion

        #region Time setting
        Task<ResultDTO> GetTimeSettings(MasterdataArgs args);
        Task<ResultDTO> UpdateConfiguration(ConfigurationViewModel args);

        #endregion

        #region recruitmentSetting - Working Address
        Task<ResultDTO> GetWorkingAddressRecruiments(QueryArgs arg);
        Task<ResultDTO> GetWorkingAddressRecruimentByCode(QueryArgs arg);
        Task<ResultDTO> CreateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data);
        Task<ResultDTO> DeleteWorkingAddressRecruiment(WorkingAddressRecruimentArgs data);
        Task<ResultDTO> UpdateWorkingAddressRecruiment(WorkingAddressRecruimentArgs data);
        #endregion
        #region recruitmentSetting - Promote And Tranfer Print - Removing
        Task<ResultDTO> GetPromoteAndTranferPrintValue(QueryArgs arg);
        Task<ResultDTO> GetPromoteAndTranferPrintByName(QueryArgs arg);
        Task<ResultDTO> CreatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data);
        Task<ResultDTO> DeletePromoteAndTranferPrint(PromoteAndTranferPrintArgs data);
        Task<ResultDTO> UpdatePromoteAndTranferPrint(PromoteAndTranferPrintArgs data);
        #endregion
        #region Global Location
        Task<ResultDTO> GetListGlobalLocations(QueryArgs arg);
        Task<ResultDTO> CreateGlobalLocation(GlobalLocationArgs data);
        Task<ResultDTO> DeleteGlobalLocation(GlobalLocationArgs data);
        Task<ResultDTO> UpdateGlobalLocation(GlobalLocationArgs data);
        Task<ResultDTO> GetGlobalLocationByCode(QueryArgs arg);
        Task<ResultDTO> GetGlobalLocationByArrivalPartition(QueryArgs arg);
        #endregion
        #region Partition
        Task<ResultDTO> GetListPartitions(QueryArgs arg);
        Task<ResultDTO> CreatePartition(PartitionArgs data);
        Task<ResultDTO> DeletePartition(PartitionArgs data);
        Task<ResultDTO> UpdatePartition(PartitionArgs data);
        Task<ResultDTO> GetPartitionByCode(QueryArgs arg);
        #endregion
        #region Reason-BTA
        Task<ResultDTO> GetBTAReasons(QueryArgs arg);
        Task<ResultDTO> CheckValidateReasons(ReasonArg arg);
        Task<ResultDTO> SaveBTAReason(ReasonArg arg);
        Task<ResultDTO> DeleteReason(Guid id);
        #endregion

        #region BTA Error Message
        Task<ResultDTO> GetBTAErrorMessageList(QueryArgs arg);
        Task<ResultDTO> CreateBTAErrorMessageList(BTAErrorMessageArgs data);
        Task<ResultDTO> DeleteBTAErrorMessage(BTAErrorMessageArgs data);
        Task<ResultDTO> UpdateBTAErrorMessage(BTAErrorMessageArgs data);
        Task<BTAErrorMessageViewModel> GetBTAErrorMessageByCode(BTAErrorEnums APIType, string errorCode);
        #endregion

        Task<ResultDTO> GetBookingContract(QueryArgs arg);
        Task<ResultDTO> UpdateBookingContract(BookingContractArgs arg);

        #region Business Model
        Task<ResultDTO> GetBusinessModelList(QueryArgs args);
        Task<ResultDTO> CreateBusinessModel(BusinessModelArgs args);
        Task<ResultDTO> UpdateBusinessModel(BusinessModelArgs model);
        Task<ResultDTO> DeleteBusinessModel(Guid Id);
        #endregion

        #region Business Model Mapping
        Task<ResultDTO> GetBusinessModelUnitMappingList(QueryArgs args);
        Task<ResultDTO> CreateBusinessModelUnitMapping(BusinessModelUnitMappingArgs args);
        Task<ResultDTO> UpdateBusinessModelUnitMapping(BusinessModelUnitMappingArgs model);
        Task<ResultDTO> DeleteBusinessModelUnitMapping(Guid Id);
        #endregion

        #region Shift plan submit person
        Task<ResultDTO> GetTargetPlanSpecial(QueryArgs args);
        Task<ResultDTO> CreateTargetPlanSpecial(TargetPlanSpecialArgs arg);
        Task<ResultDTO> DeleteTargetPlanSpecial(Guid departmentId);
        Task<ResultDTO> CheckDepartmentExistInSpecial(Guid departmentId);
        Task<ResultDTO> SaveTargetPlanSpecial(TargetPlanSpecialArgs arg);


        #endregion

    }
}
