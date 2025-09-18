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
using System.Net.Http.Headers;

namespace Aeon.HR.BusinessObjects
{
    #region Target date Details Helper
    public static class TargetDateDetails_Helper
    {
        public static DateTime GetDate(this TargetDateDetail targetDateDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                returnValue = targetDateDetail.date.GetAsDateTime("yyyyMMdd");
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static OT_CheckingResult Check_ActualOT_Time(this TargetDateDetail targetDateDetail, DateTime startTime, DateTime endTime, ShiftCodeDetailCollection ShiftCodeData, bool forceViewAll = false)
        {
            OT_CheckingResult returnValue = new OT_CheckingResult() { };
            if (targetDateDetail != null && !string.IsNullOrEmpty(targetDateDetail.value) && !targetDateDetail.value.Equals("AL"))
            {
                returnValue = new OT_CheckingResult() { status = OT_CheckingStatus.NONE };
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
                            }
                            else if (startWorkingTime < startTime && (startTime < endWorkingTime && endWorkingTime < endTime))
                            {
                                //Overlap last part of working time
                                returnValue.status = OT_CheckingStatus.OVERLAP_WORKING_TIME;
                                returnValue.calculatedActualHoursFrom = endWorkingTime.ToString("HH:mm");
                                returnValue.calculatedActualHoursTo = endTime.ToString("HH:mm");
                            }
                            else if (startTime <= startWorkingTime && startTime < endWorkingTime && endWorkingTime <= endTime)
                            {
                                //OT time cover working time
                                returnValue.status = OT_CheckingStatus.COVER_WORKING_TIME;
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
            }
            return returnValue;
        }

        public static string GetWorkingTimeInfo(this TargetDateDetail targetDateDetail, ShiftCodeDetailCollection ShiftCodeData)
        {
            string returnValue = string.Empty;
            try
            {
                DateTime date = targetDateDetail.GetDate();
                ShiftCodeDetail shiftCodeDetail = ShiftCodeData.GetDetailsByCode(targetDateDetail.value);
                DateTime startTime = shiftCodeDetail.GetStartDateTime(date);
                DateTime endTime = shiftCodeDetail.GetEndDateTime(date);
                returnValue = $"{date.ToString("dd/MM/yyyy")}: ( Working time {startTime.ToString("HH:mm")} - {endTime.ToString("HH:mm")})";
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }

    }
    #endregion

    #region Leave Application helper
    public static class LeaveApplication_Helper
    {
        public static IEnumerable<Guid> FilterLeaveApplication_WasRevoked(this IEnumerable<LeaveApplication> leaveApplications, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            IEnumerable<Guid> returnValue = new List<Guid>().AsEnumerable();
            if (leaveApplications != null && leaveApplications.Any())
            {
                var leaveApplicationIDs = leaveApplications.Select(x => x.Id);
                returnValue = unitOfWork.GetRepository<WorkflowInstance>(forceViewAll).FindBy(cWorkflow => cWorkflow != null
                && leaveApplicationIDs.Contains(cWorkflow.ItemId)
                && cWorkflow.IsCompleted
                && !cWorkflow.IsTerminated
                && cWorkflow.Histories.Any(cHistory => cHistory.Outcome == "Revoked")).Select(x => x.ItemId).ToList();
            }
            return returnValue;
        }
    }
    #endregion

    #region Shift Exchange Application helper
    public static class ShiftExchangeApplication_Helper
    {
        public static IEnumerable<Guid> FilterShiftExchangeApplication_WasRevoked(this IEnumerable<ShiftExchangeApplication> shiftExchangeApplications, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            IEnumerable<Guid> returnValue = new List<Guid>().AsEnumerable();
            if (shiftExchangeApplications != null && shiftExchangeApplications.Any())
            {
                var shiftExchange_ApplicationIDs = shiftExchangeApplications.Select(x => x.Id);
                returnValue = unitOfWork.GetRepository<WorkflowInstance>(forceViewAll).FindBy(cWorkflow => cWorkflow != null
                && shiftExchange_ApplicationIDs.Contains(cWorkflow.ItemId)
                && cWorkflow.IsCompleted
                && !cWorkflow.IsTerminated
                && cWorkflow.Histories.Any(cHistory => cHistory.Outcome == "Revoked")).Select(x => x.ItemId).ToList();
            }
            return returnValue;
        }
    }
    #endregion

    #region WorkflowInstance
    public static class WorkflowInstance_Helper
    {
        public static bool UpdateAsOutOfPeriod(this WorkflowInstance workflowInstance, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowInstance != null && unitOfWork != null)
                {
                    if (workflowInstance.Id == new Guid("deb162c7-be24-45ec-a663-c205e2b20e20"))
                    {
                        var abc = true;
                    }
                    List<WorkflowTask> tasks = workflowInstance.GetProcessingTasks(unitOfWork);
                    if (tasks != null)
                    {
                        tasks.UpdateAsOutOfPeriod(unitOfWork);
                    }
                    List<WorkflowHistory> history = workflowInstance.GetProcessingWFHistories(unitOfWork);
                    if (history != null)
                    {
                        history.UpdateAsOutOfPeriod(unitOfWork);
                    }

                    workflowInstance.IsCompleted = true;
                    unitOfWork.GetRepository<WorkflowInstance>().Update(workflowInstance);
                    unitOfWork.Commit();
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static bool UpdateAsOutOfPeriod(this IEnumerable<WorkflowInstance> workflowInstances, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowInstances != null && workflowInstances.Any() && unitOfWork != null)
                {
                    var trackingList = workflowInstances.Where(x => x != null).Select(x => new { ID = x.Id, ReferenceNumber = x.ItemReferenceNumber, status = x.UpdateAsOutOfPeriod(unitOfWork) }).ToList();
                    List<string> failedList = trackingList.Where(x => x != null && !x.status).Select(x => x.ReferenceNumber).ToList();
                    List<string> successList = trackingList.Where(x => x != null && x.status).Select(x => x.ReferenceNumber).ToList();

                    string strFailedList = failedList != null && failedList.Any() ? failedList.Aggregate((x, y) => x + "; " + y) : "";
                    string strSuccessList = successList != null && successList.Any() ? successList.Aggregate((x, y) => x + "; " + y) : "";
                    successList.UpdateItemStatus_As_OutOfPeriod(unitOfWork);
                    unitOfWork.Commit();
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static List<WorkflowTask> GetProcessingTasks(this WorkflowInstance workflowInstance, IUnitOfWork unitOfWork)
        {
            List<WorkflowTask> returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<WorkflowTask>(true).FindBy(x => x != null
                && x.ItemId == workflowInstance.ItemId && !x.IsCompleted).ToList();
            }
            catch
            {
            }
            return returnValue;
        }

        public static List<WorkflowHistory> GetProcessingWFHistories(this WorkflowInstance workflowInstance, IUnitOfWork unitOfWork)
        {
            List<WorkflowHistory> returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<WorkflowHistory>(true).FindBy(x => x != null
                && x.InstanceId == workflowInstance.Id && !x.IsStepCompleted).ToList();
            }
            catch
            {
            }
            return returnValue;
        }

        private static bool UpdateItemStatus_As_OutOfPeriod(this string itemRef, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (unitOfWork != null && !string.IsNullOrEmpty(itemRef))
                {
                    string strPrefix = itemRef.Substring(0, 3);
                    switch (strPrefix)
                    {
                        case "LEA":
                            {
                                LeaveApplication item = unitOfWork.GetRepository<LeaveApplication>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<LeaveApplication>().Update(item);
                                returnValue = true;
                                break;
                            }
                        case "OVE":
                            {
                                OvertimeApplication item = unitOfWork.GetRepository<OvertimeApplication>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<OvertimeApplication>().Update(item);
                                returnValue = true;
                                break;
                            }
                        case "MIS":
                            {
                                MissingTimeClock item = unitOfWork.GetRepository<MissingTimeClock>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<MissingTimeClock>().Update(item);
                                returnValue = true;
                                break;
                            }
                        case "SHI":
                            {
                                ShiftExchangeApplication item = unitOfWork.GetRepository<ShiftExchangeApplication>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<ShiftExchangeApplication>().Update(item);
                                returnValue = true;
                                break;
                            }
                        case "BTA":
                            {
                                BusinessTripApplication item = unitOfWork.GetRepository<BusinessTripApplication>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<BusinessTripApplication>().Update(item);
                                returnValue = true;
                                break;
                            }
                        case "RES":
                            {
                                ResignationApplication item = unitOfWork.GetRepository<ResignationApplication>(true).GetSingle(x => x != null && x.ReferenceNumber.Equals(itemRef));
                                if (item != null)
                                {
                                    item.Status = Const.Status.outOfPeriod;
                                }
                                unitOfWork.GetRepository<ResignationApplication>().Update(item);
                                returnValue = true;
                                break;
                            }
                        default:
                            returnValue = false;
                            break;
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }
        private static bool UpdateItemStatus_As_OutOfPeriod(this List<string> itemRef, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (unitOfWork != null && itemRef != null && itemRef.Any())
                {
                    itemRef.Select(x => x.UpdateItemStatus_As_OutOfPeriod(unitOfWork)).ToList();
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }
    }
    #endregion

    #region Acting helper

    public static class ActingHelper
    {
        public static DateTimeOffset? GetEndPeriodDate(this Acting actingItem, IUnitOfWork unitOfWork)
        {
            DateTimeOffset? returnValue = DateTimeOffset.MinValue;
            try
            {
                if (actingItem != null && unitOfWork != null)
                {
                    var periodToDates = new List<DateTimeOffset?>();
                    periodToDates.Add(actingItem.Period1To);
                    periodToDates.Add(actingItem.Period2To);
                    periodToDates.Add(actingItem.Period3To);
                    periodToDates.Add(actingItem.Period4To);
                    if (periodToDates.Any())
                    {
                        returnValue = periodToDates.Max(x => x);
                    }
                }
            }
            catch
            {
                returnValue = DateTimeOffset.MinValue;
            }
            return returnValue;
        }
    }
    #endregion

    #region Workflow Tasks
    public static class WorkflowTasks_Helper
    {
        public static bool UpdateAsOutOfPeriod(this WorkflowTask workflowTask, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowTask != null && !workflowTask.IsCompleted)
                {
                    workflowTask.IsCompleted = true;
                    workflowTask.Vote = VoteType.OutOfPeriod;
                    unitOfWork.GetRepository<WorkflowTask>(true).Update(workflowTask);
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static bool UpdateAsOutOfPeriod(this List<WorkflowTask> workflowTasks, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowTasks != null && workflowTasks.Any() && unitOfWork != null)
                {
                    foreach (WorkflowTask currentTask in workflowTasks)
                    {
                        currentTask.UpdateAsOutOfPeriod(unitOfWork);
                    }
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }
    }
    #endregion

    #region Workflow History
    public static class WorkflowHistory_Helper
    {
        public static bool UpdateAsOutOfPeriod(this WorkflowHistory workflowHistory, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowHistory != null && !workflowHistory.IsStepCompleted)
                {
                    workflowHistory.IsStepCompleted = true;
                    workflowHistory.VoteType = VoteType.OutOfPeriod;
                    workflowHistory.Comment = "Out Of Period";
                    unitOfWork.GetRepository<WorkflowHistory>(true).Update(workflowHistory);
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static bool UpdateAsOutOfPeriod(this List<WorkflowHistory> workflowTasks, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                if (workflowTasks != null && workflowTasks.Any() && unitOfWork != null)
                {
                    foreach (WorkflowHistory currentTask in workflowTasks)
                    {
                        currentTask.UpdateAsOutOfPeriod(unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }
    }
    #endregion

    #region UnitOfWork
    public static class UnitOfWork
    {
        public static int GetLatestDepartmentCode(this IUnitOfWork uow, int jobGrade)
        {
            int returnValue = 0;
            try
            {
                if (!(uow is null))
                {
                    Department dept = uow.GetRepository<Department>(true).FindBy(x => x != null && !string.IsNullOrEmpty(x.Code) && x.Code.Length == 10 && x.Code.ToLower().StartsWith("dep" + jobGrade)).OrderByDescending(x => x.Code).FirstOrDefault();
                    returnValue = dept.Code.Substring(4).GetAsInt();
                }
            }
            catch
            {
                returnValue = 0;
            }
            return returnValue;
        }

        public static IEnumerable<LeaveApplication> GetLeaveApplications_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<LeaveApplication> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };

                IQueryable<LeaveApplication> aLeaveApplication = uow.GetRepository<LeaveApplication>(true)
                    .FindBy(x => x != null
                     && !ignoreStatus.Contains(x.Status)
                     && !x.LeaveApplicationDetails.Any(y => y.FromDate > endDate || y.ToDate > endDate)).AsQueryable();


                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = aLeaveApplication
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static IEnumerable<MissingTimeClock> GetMissingTimeClock_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<MissingTimeClock> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };
                IQueryable<MissingTimeClock> qMissingTimeClock = uow.GetRepository<MissingTimeClock>(true).FindBy(x => x != null
                 && !ignoreStatus.Contains(x.Status)
                 && !x.MissingTimeClockDetails.Any(y => y.Date > endDate)).AsQueryable();

                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = qMissingTimeClock
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static IEnumerable<OvertimeApplication> GetOvertimeApplication_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<OvertimeApplication> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };
                IQueryable<OvertimeApplication> qOvertimeApplication = uow.GetRepository<OvertimeApplication>(true).FindBy(x => x != null
                 && !ignoreStatus.Contains(x.Status)
                 && !x.OvertimeItems.Any(y => y.Date > endDate)).AsQueryable();

                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = qOvertimeApplication
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static IEnumerable<ShiftExchangeApplication> GetShiftExchangeApplication_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<ShiftExchangeApplication> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };
                IQueryable<ShiftExchangeApplication> qShiftExchangeApplication = uow.GetRepository<ShiftExchangeApplication>(true).FindBy(x => x != null
                 && !ignoreStatus.Contains(x.Status)
                 && !x.ExchangingShiftItems.Any(y => y.ShiftExchangeDate > endDate)).AsQueryable();

                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = qShiftExchangeApplication
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static IEnumerable<BusinessTripApplication> GetBusinessTripApplication_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<BusinessTripApplication> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, "Completed Changing", Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };
                IQueryable<BusinessTripApplication> qBusinessTripApplication = uow.GetRepository<BusinessTripApplication>(true).FindBy(x => x != null
                 && !ignoreStatus.Contains(x.Status)
                 && !x.BusinessTripApplicationDetails.Any(y => y.FromDate > endDate || y.ToDate > endDate)).AsQueryable();

                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = qBusinessTripApplication
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static IEnumerable<ResignationApplication> GetResignationApplication_OutOfPeriod(this IUnitOfWork uow, DateTime startDate, DateTime endDate, bool isStore)
        {
            IEnumerable<ResignationApplication> returnValue = null;
            try
            {
                List<string> ignoreStatus = new List<string>() { Const.Status.cancelled, Const.Status.completed, Const.Status.draft, Const.Status.rejected, Const.Status.outOfPeriod };
                IQueryable<ResignationApplication> qResignationApplication = uow.GetRepository<ResignationApplication>(true).FindBy(x => x != null
                 && !ignoreStatus.Contains(x.Status)
                 && x.OfficialResignationDate <= endDate).AsQueryable();

                DateTime _45DayBefore = DateTime.Now.AddDays(-45);
                returnValue = qResignationApplication
                    .GroupJoin(uow.GetRepository<UserDepartmentMapping>().GetAll(), t => t.CreatedById, p => p.UserId, (t, p) => new { t, p })
                    .Where(x => x.p.Any(d => d.Department.IsStore == isStore) || x.t.Created < _45DayBefore).Select(x => x.t).AsEnumerable();
            }
            catch
            {
            }
            return returnValue;
        }

        public static async Task<WorkflowInstance> GetWorkflowInstance_ByItemID(this IUnitOfWork uow, Guid itemID)
        {
            WorkflowInstance returnValue = null;
            try
            {
                returnValue = await uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x != null && x.Id == itemID);
            }
            catch
            {
            }
            return returnValue;
        }

        public static async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstance_ByItemIDs(this IUnitOfWork uow, List<Guid> itemID)
        {
            IEnumerable<WorkflowInstance> returnValue = null;
            try
            {
                returnValue = await uow.GetRepository<WorkflowInstance>(true).FindByAsync(x => x != null && itemID.Contains(x.ItemId));
            }
            catch
            {
            }
            return returnValue;
        }
    }
    #endregion

    #region object    
    public static class ObjectHelper
    {
        public static T ConvertTo<T>(this object sourceObj)
        {
            var returnValue = Activator.CreateInstance(typeof(T));
            returnValue = sourceObj.ConvertTo(returnValue);
            return (T)returnValue;
        }

        public static object ConvertTo(this object sourceObj, object targetObj)
        {
            Type T1 = sourceObj.GetType();
            Type T2 = targetObj.GetType();

            PropertyInfo[] sourceProprties = T1.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] targetProprties = T2.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var sourceProp in sourceProprties)
            {
                var targetProp = targetProprties.FirstOrDefault(targetProperty => targetProperty.Name.Equals(sourceProp.Name));
                if (!(targetProp is null))
                {
                    object osourceVal = sourceProp.GetValue(sourceObj, null);
                    targetProp.SetValue(targetObj, osourceVal);
                }
            }

            return targetObj;
        }
        public static T TransformValues<T>(this object source, T destination, List<string> ignoreProperies = null)
        {
            try
            {
                // If any this null throw an exception
                if (source != null && destination != null)
                {
                    // Getting the Types of the objects
                    Type typeDest = destination.GetType();
                    Type typeSrc = source.GetType();

                    // Iterate the Properties of the source instance and  
                    // populate them from their desination counterparts  
                    PropertyInfo[] srcProps = typeSrc.GetProperties();
                    foreach (PropertyInfo srcProp in srcProps)
                    {
                        if (!(ignoreProperies is null) && ignoreProperies.Contains(srcProp.Name))
                        {
                            continue;
                        }
                        if (!srcProp.CanRead)
                        {
                            continue;
                        }
                        PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                        if (targetProperty == null)
                        {
                            continue;
                        }
                        if (!targetProperty.CanWrite)
                        {
                            continue;
                        }
                        if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                        {
                            continue;
                        }
                        if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                        {
                            continue;
                        }
                        if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                        {
                            if (srcProp.PropertyType == typeof(string))
                            {
                                string value = (string)srcProp.GetValue(source, null);
                                if (value != null)
                                {
                                    //need to add some script to parse string to object
                                    //targetProperty.SetValue(destination, JsonConvert.SerializeObject(value), null);
                                }
                            }
                            continue;
                        }
                        // Get Property Value
                        var propValue = srcProp.GetValue(source, null);

                        //Check value is type of string and was encrypted as '******'
                        if (propValue is string && string.IsNullOrEmpty(propValue.GetAsString().Trim('*')))
                        {
                            continue;
                        }

                        // Set Property Value
                        targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return destination;
        }

        public static T CloneFrom<T>(this T destinationObj, object sourceObj)
        {
            try
            {
                Type typeDest = destinationObj.GetType();
                Type typeSrc = sourceObj.GetType();
                PropertyInfo[] srcProps = typeSrc.GetProperties();
                foreach (PropertyInfo srcProp in srcProps)
                {
                    if (!srcProp.CanRead)
                    {
                        continue;
                    }
                    PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                    if (targetProperty == null)
                    {
                        continue;
                    }
                    if (!targetProperty.CanWrite)
                    {
                        continue;
                    }
                    if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                    {
                        continue;
                    }
                    if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                    {
                        continue;
                    }
                    if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                    {
                        if (targetProperty.PropertyType == typeof(string))
                        {
                            object value = srcProp.GetValue(sourceObj, null);
                            if (value != null)
                            {
                                targetProperty.SetValue(destinationObj, JsonConvert.SerializeObject(value), null);
                            }
                        }
                        continue;
                    }
                    // Passed all tests, lets set the value
                    targetProperty.SetValue(destinationObj, srcProp.GetValue(sourceObj, null), null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return destinationObj;
        }

        public static object GetPropertyValue(this Object obj, string propertyname)
        {
            object returnValue = null;
            try
            {
                if (obj != null)
                {
                    PropertyInfo[] srcProps = obj.GetType().GetProperties();
                    PropertyInfo propInfo = srcProps.FirstOrDefault(x => x.Name.Equals(propertyname, StringComparison.OrdinalIgnoreCase));
                    if (propInfo != null)
                    {
                        returnValue = propInfo.GetValue(obj);
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static string GetAsString(this object obj)
        {
            string returnValue = string.Empty;
            try
            {
                if (obj != null)
                {
                    returnValue = obj.ToString();
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static string GetAsString(this byte[] byteArray)
        {
            string returnValue = string.Empty;
            try
            {
                if (byteArray != null)
                {
                    returnValue = Convert.ToBase64String(byteArray);
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static byte[] GetAsByteArray(this string stringValue)
        {
            byte[] returnValue = null;
            try
            {
                if (!string.IsNullOrEmpty(stringValue))
                {
                    returnValue = Convert.FromBase64String(stringValue);
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
    }
    #endregion

    #region Helpers_Enum
    public class OT_CheckingResult
    {
        public OT_CheckingStatus status { get; set; }
        public string calculatedActualHoursFrom { get; set; }
        public string calculatedActualHoursTo { get; set; }
    }

    public enum OT_CheckingStatus
    {
        OK,
        OVERLAP_WORKING_TIME,
        INSIDE_WORKING_TIME,
        COVER_WORKING_TIME,
        SHIFTCODE_NOT_FOUND,
        DAY_OFF_CAN_NOT_OT,
        NONE
    }
    #endregion

    #region eDoc hander helper
    public static class eDocHandlerHelper
    {
        public static async Task<HttpResponseMessage> CalleDocAPI(string paramURI)
        {
            var client = eDocAPI();
            HttpResponseMessage response = null;
            var url = $"/_layouts/15/AeonHR/Handler/Common.ashx?{paramURI}";
            try
            {
                response = await client.GetAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private static HttpClient eDocAPI()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(AppSettingsHelper.SiteUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Requested-With", "X");
            return client;
        }
    }
    #endregion
}