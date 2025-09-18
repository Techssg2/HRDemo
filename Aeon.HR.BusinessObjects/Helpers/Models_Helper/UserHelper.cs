using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Data;
using Newtonsoft.Json;
using System.Reflection;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels.BTA;
using System.Web.Security;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.ViewModels.ExternalItem;
using static Aeon.HR.ViewModels.CommonViewModel;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;

namespace Aeon.HR.BusinessObjects
{
    #region User Helper
    public static class UserBO_Helper
    {
        public static TargetPlan GetTargetPlanByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetPlan returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                TargetPlanDetail targetPlanDetail = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode && currentTargetPlanDetail.TargetPlan != null && currentTargetPlanDetail.TargetPlan.PeriodId == targetPlanPeriod.Id && currentTargetPlanDetail.TargetPlan.Status.Equals(Const.Status.completed));
                if (targetPlanDetail != null)
                {
                    returnValue = targetPlanDetail.TargetPlan;
                }
            }
            catch (Exception ex)
            {
                returnValue = null;
            }
            return returnValue;
        } 

        //AEON_658 
        public static TargetPlan GetTargetPlanByDateForTargetPlan(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetPlan returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                //TargetPlanDetail targetPlanDetail = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode && currentTargetPlanDetail.TargetPlan != null && currentTargetPlanDetail.TargetPlan.PeriodId == targetPlanPeriod.Id && currentTargetPlanDetail.TargetPlan.Status.Equals(Const.Status.completed));
                //AEON_658 
                TargetPlanDetail targetPlanDetail = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode && currentTargetPlanDetail.TargetPlan != null && currentTargetPlanDetail.TargetPlan.PeriodId == targetPlanPeriod.Id && !currentTargetPlanDetail.TargetPlan.Status.Equals(Const.Status.cancelled) && !currentTargetPlanDetail.TargetPlan.Status.Equals(Const.Status.rejected));


                returnValue = targetPlanDetail.TargetPlan;
            }
            catch (Exception ex)
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static List<TargetPlanDetail> GetAllTargetPlanDetailsByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            List<TargetPlanDetail> returnValue = new List<TargetPlanDetail>();
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                //returnValue = unitOfWork.GetRepository<TargetPlan>(forceViewAll).FindBy(currentTargetPlan => currentTargetPlan.PeriodId == targetPlanPeriod.Id)
                //    .ToList()
                //    .FirstOrDefault(currentTargetPlan => currentTargetPlan.TargetPlanDetails != null && currentTargetPlan.TargetPlanDetails.Any(currentTargetPlanDetails => currentTargetPlanDetails.SAPCode == user.SAPCode));


                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).FindBy(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode && currentTargetPlanDetail.TargetPlan.PeriodId == targetPlanPeriod.Id).ToList();
            }
            catch
            {
                returnValue = new List<TargetPlanDetail>();
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan1(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {

            Aeon.HR.Data.Models.TargetPlanDetail returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                if (targetPlanPeriod != null)
                {
                    returnValue = user.GetTargetPlan1(unitOfWork, targetPlanPeriod.Id, forceViewAll);
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan1CompletedStatus(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {

            Aeon.HR.Data.Models.TargetPlanDetail returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                returnValue = user.GetTargetPlan1CompletedStatus(unitOfWork, targetPlanPeriod.Id, forceViewAll);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan1CompletedStatus(this User user, IUnitOfWork unitOfWork, Guid periodId, bool forceViewAll = false)
        {

            Aeon.HR.Data.Models.TargetPlanDetail returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll)
                    .GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode
                    && currentTargetPlanDetail.TargetPlan.PeriodId == periodId
                    && currentTargetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target1 && currentTargetPlanDetail.TargetPlan.Status.ToLower().Equals(Const.Status.completed.ToLower()), "created desc");
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan1(this User user, IUnitOfWork unitOfWork, Guid periodId, bool forceViewAll = false)
        {

            Aeon.HR.Data.Models.TargetPlanDetail returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll)
                    .GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode
                    && currentTargetPlanDetail.TargetPlan.PeriodId == periodId
                    && currentTargetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target1, "created desc");
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan2(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetPlanDetail returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                returnValue = user.GetTargetPlan2(unitOfWork, targetPlanPeriod.Id, forceViewAll);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetPlanDetail GetTargetPlan2(this User user, IUnitOfWork unitOfWork, Guid periodId, bool forceViewAll = false)
        {
            TargetPlanDetail returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll)
                    .GetSingle(currentTargetPlanDetail => currentTargetPlanDetail.SAPCode == user.SAPCode
                    && currentTargetPlanDetail.TargetPlan.PeriodId == periodId
                    && currentTargetPlanDetail.Type == Infrastructure.Enums.TypeTargetPlan.Target2);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static TargetDateDetail GetActualTarget1_ByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetDateDetail returnValue = null;
            try
            {
                if (!date.Equals(DateTime.MinValue))
                {
                    TargetPlanDetail targetPlan = user.GetTargetPlan1(unitOfWork, date, forceViewAll);
                    if (targetPlan != null)
                    {
                        targetPlan.Type = TypeTargetPlan.Target1;
                        returnValue = targetPlan.GetActualTargetByDate(unitOfWork, date);
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static TargetDateDetail GetActualTarget2_ByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetDateDetail returnValue = null;
            try
            {
                if (!date.Equals(DateTime.MinValue))
                {
                    TargetPlanDetail targetPlan = user.GetTargetPlan2(unitOfWork, date, forceViewAll);
                    targetPlan.Type = TypeTargetPlan.Target2;
                    returnValue = targetPlan.GetActualTargetByDate(unitOfWork, date);
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static async Task<ShiftSetResponceSAPViewModel> GetCurrentShiftSetFromSAP(this User user, IUnitOfWork unitOfWork, DateTime date, ILogger log, ITrackingBO trackingBO)
        {
            ShiftSetResponceSAPViewModel returnValue = null;
            try
            {
                var excution = new ShiftSetBO(log, unitOfWork, null, trackingBO);
                var predicate = "$filter=Pernr eq ('{0}') and Date eq ('{1}')";
                var predicateParameters = new string[] { user.SAPCode, date.ToSAPFormat() };
                excution.APIName = "GetCurrentShiftSet";
                var res = await excution.GetData(predicate, predicateParameters);
                if (res != null && res.GetType() == typeof(List<ShiftSetResponceSAPViewModel>))
                {
                    returnValue = ((List<ShiftSetResponceSAPViewModel>)res).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.LogError("GetCurrentShiftSetFromSAP " + ex.Message + ex.StackTrace);
                returnValue = null;
            }
            return returnValue;
        }
        public static bool IsStore(this User user)
        {
            bool returnValue = false;
            try
            {
                if (!(user is null))
                {
                    returnValue = user.UserDepartmentMappings.Any(x => x != null && x.IsHeadCount && x.Department.IsStore);
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        //public static ShiftExchangeApplication GetShiftExchangeByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        //{
        //    ShiftExchangeApplication returnValue = null;
        //    try
        //    {
        //        TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
        //        DateTimeOffset dateOffset = new DateTimeOffset(date);
        //        ShiftExchangeApplicationDetail shiftExchangeDetail = unitOfWork.GetRepository<ShiftExchangeApplicationDetail>(forceViewAll)
        //            .GetSingle(currentShiftExchangeApplicationDetail => currentShiftExchangeApplicationDetail.UserId == user.Id && currentShiftExchangeApplicationDetail.ShiftExchangeDate.Equals(dateOffset));
        //        returnValue = shiftExchangeDetail?.ShiftExchangeApplication;
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = null;
        //    }
        //    return returnValue;
        //}

        public static List<ShiftExchangeApplication> GetProcessingShiftExchangesByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = true)
        {
            List<ShiftExchangeApplication> returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                DateTimeOffset dateOffset = new DateTimeOffset(date);
                List<string> ignoreStatus = new List<string>() { "Rejected", "Pending", "Draft", "Completed", "Cancelled" };
                List<Guid> shiftExchangeIDs = unitOfWork.GetRepository<ShiftExchangeApplicationDetail>(forceViewAll)
                    .FindBy(currentShiftExchangeApplicationDetail => currentShiftExchangeApplicationDetail.UserId == user.Id
                    && currentShiftExchangeApplicationDetail.ShiftExchangeDate.Equals(dateOffset))
                    .Where(x => x.ShiftExchangeApplicationId.HasValue)
                    .Select(x => x.ShiftExchangeApplicationId.Value).ToList();

                List<ShiftExchangeApplication> shiftExchanges = unitOfWork.GetRepository<ShiftExchangeApplication>(forceViewAll)
                    .FindBy(sea => shiftExchangeIDs.Contains(sea.Id)
                    && !ignoreStatus.Contains(sea.Status)).ToList();


                returnValue = shiftExchanges;
            }
            catch (Exception ex)
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static List<LeaveApplication> GetLeaveApplicationByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            List<LeaveApplication> returnValue = null;
            try
            {
                TargetPlanPeriod targetPlanPeriod = date.GetTargetPlanPeriodByDate(unitOfWork, forceViewAll);
                DateTimeOffset dateOffset = new DateTimeOffset(date);
                List<string> ignoreStatus = new List<string>() { "Rejected", "Pending", "Draft", "Cancelled" };

                List<LeaveApplication> shiftExchanges = unitOfWork.GetRepository<LeaveApplication>(forceViewAll)
                    .FindBy(leave => !ignoreStatus.Contains(leave.Status) && leave.UserSAPCode == user.SAPCode && leave.LeaveApplicationDetails.Any(leaveDetail => leaveDetail.FromDate <= dateOffset && leaveDetail.ToDate >= dateOffset)).ToList();

                returnValue = shiftExchanges;
            }
            catch (Exception ex)
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static decimal GetMaxBudgetLimit(this UserListViewModel userview, Guid? PartitionId, IUnitOfWork uow)
        {
            decimal returnValue = 0;
            try
            {
                if (userview != null)
                {
                    User user = userview.SAPCode.GetUserByUserSAP(uow, true);
                    if (user != null)
                    {
                        returnValue = user.GetMaxBudgetLimit(PartitionId, uow);
                    }
                }
            }
            catch
            {
            }
            return returnValue;
        }
        public static decimal GetMaxBudgetLimit(this User user, Guid? PartitionId, IUnitOfWork uow)
        {
            decimal returnValue = 0;
            try
            {
                if (user != null)
                {
                    Department dept = user.UserDepartmentMappings.Where(x => x.IsHeadCount).Select(x => x.Department).FirstOrDefault();
                    BTAPolicySpecial btaPolicySpecial = uow.GetRepository<BTAPolicySpecial>(true).GetSingle(x => x.SapCode == user.SAPCode && x.DepartmentId == dept.Id && x.PartitionId == PartitionId);
                    if (btaPolicySpecial != null)
                    {
                        returnValue = btaPolicySpecial.BudgetTo;
                    }
                    else
                    {
                        BTAPolicy btaPolicy = uow.GetRepository<BTAPolicy>(true).GetSingle(x => x.JobGradeId == dept.JobGradeId && x.IsStore == dept.IsStore && x.PartitionId == PartitionId);
                        returnValue = btaPolicy != null ? btaPolicy.BudgetTo : 0;
                    }
                }
            }
            catch
            {
            }
            return returnValue;
        }
        public static Department GetHeadCountDepartment(this User user)
        {
            Department returnValue = null;
            try
            {
                if (user != null)
                {
                    returnValue = user.UserDepartmentMappings.Where(x => x.IsHeadCount).Select(x => x.Department).FirstOrDefault();
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static Guid? GetHeadCountDepartmentID(this User user)
        {
            Guid? returnValue = null;
            try
            {
                if (user != null)
                {
                    Department dept = user.GetHeadCountDepartment();
                    if (dept != null)
                    {
                        returnValue = dept.Id;
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static BTAUserFlightTicketViewModel GetUserFlightTickets(this User user, Guid BTADetailId, IUnitOfWork uow)
        {
            BTAUserFlightTicketViewModel result = null;
            try
            {
                var flightDetails = uow.GetRepository<FlightDetail>(true).FindBy<FlightDetailViewModel>(x => x.BusinessTripApplicationDetailId == BTADetailId);
                if (flightDetails != null)
                {
                    var changeCancelData = uow.GetRepository<ChangeCancelBusinessTripDetail>(true).GetSingle<ChangeCancelBusinessTripDetailViewModel>(x => x.BusinessTripApplicationDetailId == BTADetailId);
                    BTAUserFlightTicketViewModel itemObj = new BTAUserFlightTicketViewModel
                    {
                        SAPCode = user.SAPCode,
                        BusinessTripApplicationDetailId = BTADetailId,
                        FlightTicketInfo = flightDetails.ToList(),
                        ChangeCancelInfo = changeCancelData
                    };
                    result = itemObj;
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static UserLockedStatusViewModel GetUserLockedStatus(this User user)
        {
            UserLockedStatusViewModel returnValue = new UserLockedStatusViewModel();
            try
            {
                returnValue = user.ConvertTo<UserLockedStatusViewModel>();
                var mUser = Membership.GetUser(user.LoginName);
                returnValue.IsLoginLocked = (!mUser.IsApproved || mUser.IsLockedOut);
            }
            catch
            {
                returnValue = new UserLockedStatusViewModel();
            }
            return returnValue;
        }

        public static bool UnlockedUser(this User user)
        {
            bool returnValue = false;
            try
            {
                var mUser = Membership.GetUser(user.LoginName);
                mUser.IsApproved = true;
                returnValue = mUser.UnlockUser();
                Membership.UpdateUser(mUser);
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static async Task<bool> SyncUserDataFromSAP(this User user, ILogger logger, IEmployeeBO employee, ISSGExBO bo, IUnitOfWork uow, int year = 0)
        {
            bool retrunValue = false;
            try
            {
                if (user != null && uow != null)
                {
                    uow.RefreshContext(user);
                    var resultJoiningDate = await employee.GetJoiningDateOfEmployee(user.SAPCode);
                    if (resultJoiningDate != null)
                    {
                        user.StartDate = resultJoiningDate;
                        if (year <= 0)
                        {
                            year = DateTime.Now.Year;
                        }
                        var result = await bo.GetMultipleLeaveBalanceSet(new List<string>() { user.SAPCode }, year);
                        var userInfoUpdate = new UpdateUserInfoFromSapJob(logger, uow, employee, bo);
                        await userInfoUpdate.UpdateErdRemain(new List<User>() { user }, result, year);
                    }
                    uow.GetRepository<User>().Update(user);
                    retrunValue = true;
                }
            }
            catch
            {
                retrunValue = false;
            }
            return retrunValue;
        }

        public static bool HasTargetPlanCompleted(this User user, IUnitOfWork unitOfWork, DateTime date, bool hasCheckStatusPayload)
        {
            bool returnValue = false;
            try
            {
                /*TargetPlanDetail detail = user.GetTargetPlan1(unitOfWork, date, true);*/
                TargetPlanDetail detail = user.GetTargetPlan1CompletedStatus(unitOfWork, date, true);
                if (detail is null)
                    returnValue = false;
                else
                {
                    if (detail.TargetPlan != null && !string.IsNullOrEmpty(detail.TargetPlan.Status) && detail.TargetPlan.Status.Equals(Const.Status.completed, StringComparison.OrdinalIgnoreCase))
                    {
                        returnValue = true;
                        if (hasCheckStatusPayload)
                        {
                            returnValue = false;
                            var trackingLogInitData = unitOfWork.GetRepository<TrackingLogInitData>(true).FindBy(x => x.ReferenceNumber == detail.TargetPlan.ReferenceNumber).Select(y => y.TrackingLogId).ToList();
                            if (trackingLogInitData.Any())
                            {
                                var trackingRequests = unitOfWork.GetRepository<TrackingRequest>(true).FindBy(x => x.ReferenceNumber == detail.TargetPlan.ReferenceNumber && trackingLogInitData.Contains(x.Id)
                                /*&& !string.IsNullOrEmpty(x.Status) && x.Status.ToLower().Equals("success")*/);
                                if (trackingRequests.Any())
                                {
                                    foreach (var x in trackingRequests)
                                    {
                                        if (string.IsNullOrEmpty(x.Payload)) continue;
                                        dynamic payload = JObject.Parse(x.Payload);
                                        if (payload.Pernr != null)
                                        {
                                            var userSapCode = ((string) payload.Pernr).Replace("{", "").Replace("}", "");
                                            if (userSapCode.Equals(user.SAPCode) && (!string.IsNullOrEmpty(x.Status)
                                                && ((x.Status.ToUpper().Equals(Const.SAPStatus.SUCCESS)) || (!x.Status.ToLower().Equals("success") && TargetPlanDataExistSAP(x.Response)))))
                                            {
                                                returnValue = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                returnValue = true;
            }
            return returnValue;
        }

        private static bool TargetPlanDataExistSAP(string response)
        {
            var returnValue = false;
            TargetPlanSAPAPIResponse modelResponse = Mapper.Map<TargetPlanSAPAPIResponse>(JsonConvert.DeserializeObject<TargetPlanSAPAPIResponse>(response));
            if (modelResponse != null && modelResponse.d != null && !string.IsNullOrEmpty(modelResponse.d.Status) && modelResponse.d.Status.Equals(Const.SAPStatus.FAIL) && !string.IsNullOrEmpty(modelResponse.d.Err_log))
            {
                returnValue = modelResponse.d.Err_log.Trim().ToUpper().Equals(Const.SAPErrorLog.DATA_IS_ALREADY_EXISTED.Trim().ToUpper());
            }
            return returnValue;
        }

        private const string USER_KEY = "uxr";
        private const string SECRET = "secret";
        public static string GetUrxUser()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.Headers.GetValues(USER_KEY).FirstOrDefault() + string.Empty;
            }
            return string.Empty;
        }

        public static List<string> HasExistOvertimeApplication(this User user, IUnitOfWork unitOfWork, OvertimeApplicationDetailArgs overtimeDetails)
        {
            List<string> valueReturnReferenceNumberExists = new List<string>();
            try
            {
                DateTime overtimeDate = DateTime.ParseExact(overtimeDetails.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan fromHour = ConvertTime(overtimeDetails.ProposalHoursFrom);
                TimeSpan toHour = ConvertTime(overtimeDetails.ProposalHoursTo);

                if (toHour < fromHour)
                {
                    toHour = toHour + TimeSpan.FromHours(24);
                }
                List<OvertimeApplicationDetail> details = user.GetOvertimeApplicationDetailByDate(unitOfWork, overtimeDate, true);

                valueReturnReferenceNumberExists = details.Where(x => x.OvertimeApplicationId != overtimeDetails.OvertimeApplicationId && x.Date.Date == overtimeDate
                    && ConvertTime(x.ProposalHoursFrom) <= toHour && ConvertTime(x.ProposalHoursTo) >= fromHour
                    && TimeSpan.Compare(fromHour, ConvertTime(x.ProposalHoursTo)) != 0 && TimeSpan.Compare(toHour, ConvertTime(x.ProposalHoursFrom)) != 0)
                        .Select(y => y.OvertimeApplication.ReferenceNumber).ToList();
                if (valueReturnReferenceNumberExists.Any())
                {
                    return valueReturnReferenceNumberExists;
                }
            }
            catch (Exception e)
            {
                valueReturnReferenceNumberExists = new List<string>();
            }
            return valueReturnReferenceNumberExists;
        }
        public static TimeSpan ConvertTime(string dateStr)
        {
            TimeSpan returnValues = TimeSpan.MinValue;
            try
            {
                returnValues = DateTime.ParseExact(dateStr, "HH:mm", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;
            }
            catch (Exception e)
            {

            }
            return returnValues;
        }
        public static List<OvertimeApplicationDetail> GetOvertimeApplicationDetailByDate(this User user, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {

            List<OvertimeApplicationDetail> returnValue = null;
            try
            {
                returnValue = date.GetOvertimeApplicationDetailByDate(user, unitOfWork, forceViewAll).ToList();
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
    }
    #endregion
}
