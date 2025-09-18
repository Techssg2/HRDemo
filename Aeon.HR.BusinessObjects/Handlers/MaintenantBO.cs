using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class MaintenantBO : IMaintenantBO
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ILogger _logger;
        protected readonly IMasterDataB0 _masterData;
        protected readonly SAPSettingsSection _sapSetting;
        protected string sap_client;
        protected string format;
		private readonly IIntegrationExternalServiceBO _exBO;
		private readonly IEmployeeBO _employee;
		private readonly ISSGExBO _bo;
		public MaintenantBO(IUnitOfWork uow, ILogger logger, IMasterDataB0 masterData, IIntegrationExternalServiceBO exBO, IEmployeeBO employee, ISSGExBO bo)
        {
            _uow = uow;
            _sapSetting = (SAPSettingsSection)ConfigurationManager.GetSection("sapSettings");
            sap_client = $"sap-client={_sapSetting.Header.SapClient}";
            format = $"$format={_sapSetting.Header.Format}";
            _logger = logger;
			_masterData = masterData;
			_exBO = exBO;
			_employee = employee;
			_bo = bo;
		}
		public ResultDTO GetItemsHasWrongStatus()
		{
			try
			{
				List<MaintenantItemInfo> returnValue = new List<MaintenantItemInfo>();
				string sqlQuery = @"
				SELECT ItemReferenceNumber, ItemId FROM 
				(
					SELECT
						ItemReferenceNumber
						, A.ItemId
						, Lastest_Created
						, Latest_IsCompleted
						, Latest_IsTerminated
					FROM 
						WorkflowInstances AS A
					CROSS APPLY 
						(
							Select Top(1) 
								Created as Lastest_Created
								, IsCompleted as Latest_IsCompleted
								, IsTerminated as Latest_IsTerminated
							from WorkflowInstances AS B
							Where A.ItemReferenceNumber = B.ItemReferenceNumber
							Order BY Created DESC
						) AAAAA
				) as BBBBB
				Left join MissingTimeClocks ON MissingTimeClocks.Id = ItemId
				Left join LeaveApplications ON LeaveApplications.Id = ItemId
				Left join ShiftExchangeApplications ON ShiftExchangeApplications.Id = ItemId
				Left join OvertimeApplications ON OvertimeApplications.Id = ItemId
				Left join TargetPlans ON TargetPlans.Id = ItemId
				Left join RequestToHires ON RequestToHires.Id = ItemId
				Left join ResignationApplications ON ResignationApplications.Id = ItemId
				Left join PromoteAndTransfers ON PromoteAndTransfers.Id = ItemId
				--Left join Actings ON Actings.Id = WorkflowInstances.ItemId
				Left join BusinessTripApplications ON BusinessTripApplications.Id = ItemId
				WHERE 
					(Latest_IsCompleted = 1 AND Latest_IsTerminated = 0)
					AND Lastest_Created > DATEADD(MONTH, -1, GETDATE())
					AND 
					(
						([ItemReferenceNumber]  like 'MIS%' AND MissingTimeClocks.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'LEA%' AND LeaveApplications.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'SHI%' AND ShiftExchangeApplications.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'OVE%' AND OvertimeApplications.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'TAR%' AND TargetPlans.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'REQ%' AND RequestToHires.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'RES%' AND ResignationApplications.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'PRO%' AND PromoteAndTransfers.Status NOT IN ('Completed', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
						OR ([ItemReferenceNumber]  like 'BTA%' AND BusinessTripApplications.Status NOT IN ('Completed', 'Completed Changing', 'Cancelled', 'Rejected', 'Requested To Change', 'Out Of Period'))
					)
				Group BY ItemReferenceNumber, ItemId
				Order BY ItemReferenceNumber";
				string connectString = ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString;
				using (DataHelper dthelper = new DataHelper(connectString, _logger))
				{
					DataTable wrongStatusTable = dthelper.GetDataAsTable(sqlQuery);
					if (wrongStatusTable != null && wrongStatusTable.Rows != null && wrongStatusTable.Rows.Count > 0)
					{
						returnValue = wrongStatusTable.Rows.Cast<DataRow>().Select(x => x.ToObject<MaintenantItemInfo>()).ToList();
						foreach (MaintenantItemInfo item in returnValue)
						{
							StatusItemInfo statusInfo = item.ItemId.GetStatusItemInfo(_uow);
							item.Status = statusInfo?.Status;

						}
					}
				}
				return new ResultDTO() { Object = returnValue };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public ResultDTO GetItemsNotHavePayload()
		{
			try
			{
				List<MaintenantItemInfo> returnValue = new List<MaintenantItemInfo>();
				string sqlQuery = @"DECLARE @currentDate DATETIME = GETDATE()
DECLARE @prevMonthDate DATETIME = DATEADD(MONTH, -1, @currentDate)
DECLARE @startDate DATETIME = DATEFROMPARTS(YEAR(@prevMonthDate), MONTH(@prevMonthDate), 25) 

BEGIN
WITH referenceNumberHasTrackingRequest AS 
(
	SELECT ReferenceNumber from TrackingRequests where Created > @startDate
)
SELECT ItemReferenceNumber
,ItemId
FROM
(
	SELECT	WorkflowInstances.*
	, LEFT(ItemReferenceNumber, 3) as RefPrefix
	, IIF(ItemReferenceNumber in (SELECT ReferenceNumber from referenceNumberHasTrackingRequest ),1,0) as HasTrackingRequest
	FROM WorkflowInstances 
	WHERE 
	Created > @startDate
	AND IsCompleted = 1 AND IsTerminated = 0
) as AAA
CROSS APPLY (
	SELECT top(1) Outcome AS Outcome
	, VoteType as VoteType
		FROM WorkflowHistories 
		WHERE WorkflowHistories.InstanceId = AAA.Id
		order by Created desc
) AS C
CROSS APPLY (
	SELECT Vote 
		FROM WorkflowTasks 
		WHERE WorkflowTasks.WorkflowInstanceId = AAA.Id
) AS E
Left join MissingTimeClocks ON MissingTimeClocks.Id = ItemId
Left join LeaveApplications ON LeaveApplications.Id = ItemId
Left join ShiftExchangeApplications ON ShiftExchangeApplications.Id = ItemId
Left join OvertimeApplications ON OvertimeApplications.Id = ItemId
Left join TargetPlans ON TargetPlans.Id = ItemId
Left join RequestToHires ON RequestToHires.Id = ItemId
Left join ResignationApplications ON ResignationApplications.Id = ItemId
Left join PromoteAndTransfers ON PromoteAndTransfers.Id = ItemId
WHERE
	HasTrackingRequest = 0
	AND Outcome not IN ('Cancelled', 'Revoked', 'Submitted', 'Rejected')
	AND VoteType = 1
	AND RefPrefix NOT IN ( 'BTA', 'REQ')
	AND 
	(
		([ItemReferenceNumber]  like 'MIS%' AND MissingTimeClocks.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'LEA%' AND LeaveApplications.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'SHI%' AND ShiftExchangeApplications.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'OVE%' AND OvertimeApplications.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'TAR%' AND TargetPlans.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'REQ%' AND RequestToHires.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'RES%' AND ResignationApplications.Status = 'Completed')
		OR ([ItemReferenceNumber]  like 'PRO%' AND PromoteAndTransfers.Status = 'Completed')
		)
group by ItemReferenceNumber, ItemId
order by ItemReferenceNumber
END";
				string connectString = ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString;
				using (DataHelper dthelper = new DataHelper(connectString, _logger))
				{
					DataTable wfsInMonthTable = dthelper.GetDataAsTable(sqlQuery);
					if (wfsInMonthTable != null && wfsInMonthTable.Rows != null && wfsInMonthTable.Rows.Count > 0)
					{
						List<WorkflowInstance> WFInstances = wfsInMonthTable.Rows.Cast<DataRow>().Select(x => x.ToObject<WorkflowInstance>()).ToList();
						returnValue = WFInstances.Select(x=> new MaintenantItemInfo()
						{
							ItemId=x.ItemId,
							ItemReferenceNumber = x.ItemReferenceNumber
						}).ToList();

					}
				}
				return new ResultDTO() { Object = returnValue };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public ResultDTO GetItemsHadPending()
		{
			try
			{
				List<MaintenantItemInfo> returnValue = new List<MaintenantItemInfo>();
				string sqlQuery = @"
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM TargetPlans
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM ShiftExchangeApplications
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM ResignationApplications
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM Actings
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM LeaveApplications
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM PromoteAndTransfers
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM MissingTimeClocks
				where [Status] = 'Pending'
				UNION ALL
				SELECT
					ReferenceNumber as ItemReferenceNumber
					, Id as ItemId
					, [Status]
				FROM OvertimeApplications
				where [Status] = 'Pending'";
				string connectString = ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString;
				using (DataHelper dthelper = new DataHelper(connectString, _logger))
				{
					DataTable itemsHadPending = dthelper.GetDataAsTable(sqlQuery);
					if (itemsHadPending != null && itemsHadPending.Rows != null && itemsHadPending.Rows.Count > 0)
					{
						returnValue = itemsHadPending.Rows.Cast<DataRow>().Select(x => x.ToObject<MaintenantItemInfo>()).ToList();
					}
				}
				return new ResultDTO() { Object = returnValue };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public ResultDTO GetUserLockedStatus(string sapCode)
		{
			try
			{
				List<User> users = _uow.GetRepository<User>().FindBy(x => x.SAPCode == sapCode && x.Type == LoginType.Membership).ToList();
				List<UserLockedStatusViewModel> userLockedStatusList = users.Where(x => x != null).Select(x => x.GetUserLockedStatus()).ToList();
				return new ResultDTO() { Object = userLockedStatusList };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public async Task<ResultDTO> GenerateOTPayload(string otReferenceNumber)
		{
			try
			{
				List<TrackingRequest> returnValue = new List<TrackingRequest>();
				try
				{
					OvertimeApplicationViewModel otItem =_uow.GetRepository<OvertimeApplication>().GetSingle<OvertimeApplicationViewModel>(x=>x.ReferenceNumber == otReferenceNumber);
					if (otItem != null && otItem.Id  != Guid.Empty)
					{
						var item = await GetWorkflowItem(otItem.Id);
						var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == otItem.Id, "Created desc");
						if (wfInstance.IsCompleted)
						{
							await PushData(item.Entity);
							returnValue = _uow.GetRepository<TrackingRequest>()
								.FindBy(x=>x.ReferenceNumber == otReferenceNumber)
								.OrderBy(x=>x.Payload).ToList();
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message, ex); 
					returnValue = new List<TrackingRequest>();
				}
				return new ResultDTO() { Object = returnValue };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public ResultDTO UnlockedUser(string sapCode)
		{
			try
			{
				string membershipConnection = ConfigurationManager.ConnectionStrings["MembershipConnection"].ConnectionString;
				List<User> users = _uow.GetRepository<User>().FindBy(x => x.SAPCode == sapCode && x.Type == LoginType.Membership).ToList();
				using (DataHelper dthelper = new DataHelper(membershipConnection, _logger))
				{
					foreach (User user in users)
					{
						string unlockUserCommand = @"Declare @UserName NVarChar(30)    = '" + user.LoginName + @"'
											Declare @Application NVarChar(255)
											set @Application = '/'    
											
											--Unlock user
											Update aspnet_Membership set IsApproved = 1, IsLockedOut = 0  WHERE UserID IN  (SELECT UserID FROM aspnet_Users u, aspnet_Applications a WHERE u.UserName=@UserName and a.ApplicationName = @Application AND u.ApplicationId = a.ApplicationId)";
						dthelper.ExecuteSQLNonQuery(unlockUserCommand);
					}
				}

				List<bool> userLockedStatusList = users.Where(x => x != null).Select(x => x.UnlockedUser()).ToList();
				return new ResultDTO() { Object = userLockedStatusList };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
		public ResultDTO GetLockedUser()
		{
			List<UserLockedStatusViewModel> returnValue = null;
			try
			{				
				string sqlQuery = @"SELECT aspnet_Users.LoweredUserName as userLoginName FROM aspnet_Membership
									inner join aspnet_Users on aspnet_Users.UserId = aspnet_Membership.UserId
									where IsApproved = 0 OR IsLockedOut = 1";
				string connectString = ConfigurationManager.ConnectionStrings["MembershipConnection"].ConnectionString;
				using (DataHelper dthelper = new DataHelper(connectString, _logger))
				{
					DataTable lockedUserTable = dthelper.GetDataAsTable(sqlQuery);
					if (lockedUserTable != null && lockedUserTable.Rows != null && lockedUserTable.Rows.Count > 0)
					{
						List<string> lockedUserLoginList = lockedUserTable.Rows.Cast<DataRow>().Select(x => x.ToObject<LockedUser>()).Select(x=>x.UserLoginName).ToList();
						List<User> users = _uow.GetRepository<User>().FindBy(x => lockedUserLoginList.Contains(x.LoginName.ToLower()) && x.IsActivated && !x.IsDeleted && x.Type == LoginType.Membership
						).ToList();
						returnValue = users.Where(x => x != null).Select(x => x.GetUserLockedStatus()).ToList();
						
					}
					return new ResultDTO() { Object = returnValue };
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				throw;
			}
		}
						
		public ResultDTO UpdateStatus(string ReferenceNumber) {
			try
			{
				string type = ReferenceNumber.Substring(0, 3);
				switch (type) {
					case "OVE":
						var overtime =   _uow.GetRepository<OvertimeApplication>().FindBy(x => x.ReferenceNumber == ReferenceNumber).ToList();

						if (overtime.Count == 1) {
							OvertimeApplication item = overtime[0];
							item.Status = "Completed";
							_uow.GetRepository<OvertimeApplication>().Update(item);
							_uow.Commit(); 
						}
						break;
					case "RES":
						var res = _uow.GetRepository<ResignationApplication>().FindBy(x => x.ReferenceNumber == ReferenceNumber).ToList();

						if (res.Count == 1)
						{
							ResignationApplication item = res[0];
							item.Status = "Completed";
							_uow.GetRepository<ResignationApplication>().Update(item);
							_uow.Commit();
						}
						break;
					case "MIS":
						var mis = _uow.GetRepository<MissingTimeClock>().FindBy(x => x.ReferenceNumber == ReferenceNumber).ToList();

						if (mis.Count == 1)
						{
							MissingTimeClock item = mis[0];
							item.Status = "Completed";
							_uow.GetRepository<MissingTimeClock>().Update(item);
							_uow.Commit();
						}
						break;
					default:
						return new ResultDTO() { Object = new {isSuccess = false } };
				};
					 
			}
			catch (Exception ex) {
				_logger.LogError(ex.Message, ex);
				throw;  
			}
			return new ResultDTO() { Object = new { isSuccess = true } };
		}
		public async Task<ResultDTO> SubmitPayload(Guid itemID)
		{
			bool returnValue = false;
			try
			{
				if (itemID != Guid.Empty)
				{
					var item = await GetWorkflowItem(itemID);
					var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == itemID, "Created desc");
					if (wfInstance.IsCompleted)
					{
						 await PushData(item.Entity);
						returnValue = true;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex); returnValue = false;
			}
			return new ResultDTO() { Object = returnValue };
		}

		public async Task PushData(WorkflowEntity item)
		{
			try
			{
				if (item.Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
				{
					await _exBO.SubmitData(item, false);
				}

			}
			catch (Exception ex)
			{
				_logger.LogError("PushData: " + ex.Message + ex.StackTrace, ex);
			}
		}

		private async Task<WorkflowItem> GetWorkflowItem(Guid ItemId)
		{
			var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
			var workflowModelTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
			foreach (var workflowModelType in workflowModelTypes)
			{
				var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", workflowModelType);
				var item = (await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", ItemId));
				if (item != null)
				{
					return new WorkflowItem() { Entity = item as WorkflowEntity, Type = workflowModelType.Name };
				}
			}
			return null;
		}

		public async Task<ResultDTO> GetUserSend_OT_Holiday_Failed()
		{
			//List<HolidayOT_CheckingResult> returnValue = new List<HolidayOT_CheckingResult>();
			//try
			//{
			//	List<TrackingRequest> payload_SendSAP_Failed = _uow.GetRepository<TrackingRequest>(true).FindBy(x => x.ReferenceNumber.StartsWith("OVE") && x.Status == "Error: Time OT is conflicted with normal shift" && x.Created > new DateTime(2021, 12, 01)).ToList();
			//	if(payload_SendSAP_Failed.Any())
			//             {
			//		ResultDTO ShiftCodeData = await _masterData.GetMasterDataValues(new MasterDataArgs() { Name = "ShiftCode", ParentCode = string.Empty });
			//		ArrayResultDTO ShiftCodeArray = ShiftCodeData.Object as ArrayResultDTO;
			//		List<MasterExternalDataViewModel> ShiftCode_MasterData = ShiftCodeArray.Data as List<MasterExternalDataViewModel>;
			//		ShiftCodeDetailCollection shiftCodeCollection = new ShiftCodeDetailCollection(ShiftCode_MasterData);

			//		List<string> oveRefNumbers = payload_SendSAP_Failed.Select(x => x.ReferenceNumber).ToList();
			//		List<OvertimeApplicationViewModel> ovetimeTickets = _uow.GetRepository<OvertimeApplication>(true)
			//			.FindBy<OvertimeApplicationViewModel>(x => oveRefNumbers.Contains(x.ReferenceNumber)).ToList();
			//		int totalItemCheck = 0;
			//                 foreach (OvertimeApplicationViewModel cOVE in ovetimeTickets)
			//                 {
			//			foreach (OvertimeApplicationDetailViewModel cDetail in cOVE.OvertimeItems)
			//                     {
			//                         try
			//				{
			//					User cUser = null;
			//					if (cOVE.Type == OverTimeType.EmployeeSeftService)
			//					{
			//						cUser = cOVE.UserSAPCode.GetUserByUserSAP(_uow, true);
			//					}
			//					else
			//					{
			//						cUser = cDetail.SAPCode.GetUserByUserSAP(_uow, true);
			//					}

			//					HolidayOT_CheckingResult checkingResult = new HolidayOT_CheckingResult();
			//					TargetDateDetail targetDetail = null;
			//					DateTime otDate = cDetail.Date.Date.ToLocalTime();
			//					targetDetail = cUser.GetActualTarget1_ByDate(_uow, otDate, true);

			//					checkingResult = targetDetail.Check_CheckOT_Holiday_Time(cDetail.GetActual_StartTime(), cDetail.GetActual_EndTime(), shiftCodeCollection, true);
			//					if (checkingResult.status == OT_CheckingStatus.OVERLAP_WORKING_TIME || checkingResult.status == OT_CheckingStatus.COVER_WORKING_TIME)
			//					{
			//						checkingResult.OTDetailsId = cDetail.Id;
			//						checkingResult.otDate = otDate;
			//						checkingResult.userSapCode = cUser.SAPCode;
			//						checkingResult.name = cUser.FullName;
			//						returnValue.Add(checkingResult);
			//					}
			//				}
			//                         catch
			//                         {
			//                         }
			//			}
			//			totalItemCheck += cOVE.OvertimeItems.Count();

			//		}
			//		_logger.LogInformation("Check OT holiday failed: totalItemCheck - " + totalItemCheck );
			//	}
			//	return new ResultDTO() { Object = returnValue };
			//}
			//catch (Exception ex)
			//{
			//	_logger.LogError(ex.Message, ex);
			//}
			return new ResultDTO() { Object = null, ErrorCodes= new List<int>() { 1002 }, Messages = new List<string>() { "System Errors" } };
		}

		public async Task<ResultDTO> SyncUserDataFromSAP(string userSAPCode)
		{
			bool returnValue = false;

			try
            {
				var user = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == userSAPCode && x.IsActivated == true && x.IsDeleted == false);
				returnValue = await user.SyncUserDataFromSAP(_logger, _employee, _bo, _uow, DateTime.Now.Year);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				returnValue = false;
			}
			return new ResultDTO() { Object = returnValue };
		}

		private class WorkflowItem
		{
			public WorkflowEntity Entity { get; set; }
			public string Type { get; set; }
		}
	}

	public static class MaintainHelper
    {

		public static HolidayOT_CheckingResult Check_CheckOT_Holiday_Time(this TargetDateDetail targetDateDetail, DateTime startTime, DateTime endTime, ShiftCodeDetailCollection ShiftCodeData, bool forceViewAll = false)
		{
			HolidayOT_CheckingResult returnValue = new HolidayOT_CheckingResult() { status = OT_CheckingStatus.NONE };
			try
			{
				List<string> dayoff_ShiftCodes = new List<string>() { "ERD", "PRD" };
				if (!(targetDateDetail is null))
				{
					ShiftCodeDetail shiftCodeDetail = ShiftCodeData.GetDetailsByCode(targetDateDetail.value);
					if (shiftCodeDetail is null)
					{
						returnValue.status = OT_CheckingStatus.SHIFTCODE_NOT_FOUND;
					}
					else if (dayoff_ShiftCodes.Contains(targetDateDetail.value.ToUpper()))
					{
						//returnValue.status = OT_CheckingStatus.DAY_OFF_CAN_NOT_OT;
						//Forced: ERD and PROD not check working time
						returnValue.status = OT_CheckingStatus.OK;
					}
					else
					{
						DateTime date = targetDateDetail.GetDate();
						DateTime startWorkingTime = shiftCodeDetail.GetStartDateTime(date);
						DateTime endWorkingTime = shiftCodeDetail.GetEndDateTime(date);
						if (endWorkingTime < startWorkingTime)
						{
							endWorkingTime = endWorkingTime.AddDays(1);
						}


						if (startWorkingTime < startTime && endTime < endWorkingTime)
						{
							//Inside working time
							returnValue.status = OT_CheckingStatus.INSIDE_WORKING_TIME;
						}
						else if (startTime < startWorkingTime && (startWorkingTime < endTime && endTime < endWorkingTime))
						{
							//Overlap first part of working time
							returnValue.status = OT_CheckingStatus.OVERLAP_WORKING_TIME;
							returnValue.calculatedActualHoursFrom = startTime.ToString("HH:mm");
							returnValue.calculatedActualHoursTo = startWorkingTime.ToString("HH:mm");

							returnValue.workingtimeFrom = startWorkingTime;
							returnValue.workingtimeTo = endWorkingTime;
							returnValue.totalWorkingTime = (endWorkingTime - startWorkingTime).TotalHours;
							returnValue.OtFrom = startTime;
							returnValue.OtTo = endTime;
							returnValue.totalOTTime = (endTime - startTime).TotalHours;
						}
						else if (startWorkingTime < startTime && (startTime < endWorkingTime && endWorkingTime < endTime))
						{
							//Overlap last part of working time
							returnValue.status = OT_CheckingStatus.OVERLAP_WORKING_TIME;
							returnValue.calculatedActualHoursFrom = endWorkingTime.ToString("HH:mm");
							returnValue.calculatedActualHoursTo = endTime.ToString("HH:mm");

							returnValue.workingtimeFrom = startWorkingTime;
							returnValue.workingtimeTo = endWorkingTime;
							returnValue.totalWorkingTime = (endWorkingTime - startWorkingTime).TotalHours;
							returnValue.OtFrom = startTime;
							returnValue.OtTo = endTime;
							returnValue.totalOTTime = (endTime - startTime).TotalHours;
						}
						else if (startTime <= startWorkingTime && startTime < endWorkingTime && endWorkingTime <= endTime)
						{
							//OT time cover working time
							returnValue.status = OT_CheckingStatus.COVER_WORKING_TIME;

							returnValue.workingtimeFrom = startWorkingTime;
							returnValue.workingtimeTo = endWorkingTime;
							returnValue.totalWorkingTime = (endWorkingTime - startWorkingTime).TotalHours;
							returnValue.OtFrom = startTime;
							returnValue.OtTo = endTime;
							returnValue.totalOTTime = (endTime - startTime).TotalHours;
						}
						else
						{
							returnValue.status = OT_CheckingStatus.OK;
							returnValue.calculatedActualHoursFrom = startTime.ToString("HH:mm");
							returnValue.calculatedActualHoursTo = endTime.ToString("HH:mm");

						}

					}
				}
			}
			catch
			{
				returnValue.status = OT_CheckingStatus.NONE;
			}
			return returnValue;
		}

	}


	public class HolidayOT_CheckingResult
	{
		public OT_CheckingStatus status { get; set; }
		public Guid OTDetailsId { get; set; }
		public string userSapCode { get; set; }
		public string name { get; set; }
		public DateTime otDate { get; set; }
		public DateTime workingtimeFrom { get; set; }
		public DateTime workingtimeTo { get; set; }
		public double totalWorkingTime { get; set; }
		public DateTime OtFrom { get; set; }
		public DateTime OtTo { get; set; }
		public double totalOTTime { get; set; }
		public string calculatedActualHoursFrom { get; set; }
		public string calculatedActualHoursTo { get; set; }
	}
}

