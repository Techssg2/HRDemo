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
using Aeon.HR.BusinessObjects.Helpers;
using System.Configuration;

namespace Aeon.HR.BusinessObjects
{

    #region Target Plan Helper
    public static class TargetPlan_Helper
    {
        public static List<TargetPlanDetail> GetTargetPlanDetailsByPeriod(this TargetPlan targetPlan, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            List<TargetPlanDetail> returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).FindBy(x => x.TargetPlanId == targetPlan.Id).ToList();
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static List<TargetPlanDetail> GetTargetPlanDetailsByPeriod(this TargetPlan targetPlan, IUnitOfWork unitOfWork, User user, bool forceViewAll = false)
        {
            List<TargetPlanDetail> returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanDetail>(forceViewAll).FindBy(currentTargetPlanDetail => currentTargetPlanDetail.TargetPlanId == targetPlan.Id && currentTargetPlanDetail.SAPCode == user.SAPCode).ToList();
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static bool IsDraft(this TargetPlanViewModel targetPlanViewModel)
        {
            return !(targetPlanViewModel is null) && !string.IsNullOrEmpty(targetPlanViewModel.Status) && targetPlanViewModel.Status.Equals("draft", StringComparison.OrdinalIgnoreCase);
        }
    }
    #endregion

    #region Target Plan Details Helper
    public static class TargetPlanDetails_Helper
    {
        public static TargetDateDetailCollection GetActualTargetInfos(this TargetPlanDetail targetPlan, IUnitOfWork unitOfWork)
        {
            TargetDateDetailCollection returnValue = new TargetDateDetailCollection();
            try
            {
                DateTimeOffset fromDate = targetPlan.TargetPlan.PeriodFromDate;
                DateTimeOffset toDate = targetPlan.TargetPlan.PeriodToDate;
                var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };

                var listLeaveDetails = unitOfWork.GetRepository<LeaveApplicationDetail>().FindBy(x => x.LeaveApplication.UserSAPCode == targetPlan.SAPCode && !statusToCheckes.Contains(x.LeaveApplication.Status) &&
                               ((fromDate <= x.FromDate && x.FromDate <= toDate)
                               ||
                               (fromDate <= x.ToDate && x.ToDate <= toDate)), "", y => y.LeaveApplication).ToList();
                var listShiftDetails = unitOfWork.GetRepository<ShiftExchangeApplicationDetail>().FindBy(x => targetPlan.SAPCode == x.User.SAPCode &&
                            (fromDate <= x.ShiftExchangeDate && x.ShiftExchangeDate <= toDate) && !statusToCheckes.Contains(x.ShiftExchangeApplication.Status)).ToList();

                //loc shiftCode theo targetType
                List<string> halfCodes = ConfigurationManager.AppSettings["validateTargetPlanTarget1"].Split(',').Select(x => x.Trim()).ToList();
                if (targetPlan.Type == TypeTargetPlan.Target1)
                {
                    listLeaveDetails = listLeaveDetails.Where(x => !halfCodes.Contains(x.LeaveCode)).ToList();
                    listShiftDetails = listShiftDetails.Where(x => !halfCodes.Contains(x.NewShiftCode)).ToList();
                }
                else if(targetPlan.Type == TypeTargetPlan.Target2)
                {
                    listLeaveDetails = listLeaveDetails.Where(x => halfCodes.Contains(x.LeaveCode)).ToList();
                    listShiftDetails = listShiftDetails.Where(x => halfCodes.Contains(x.NewShiftCode)).ToList();
                }

                var tempIds = listLeaveDetails.Select(x => x.LeaveApplicationId).ToList();
                var listLeaves = unitOfWork.GetRepository<LeaveApplication>().FindBy(x => tempIds.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                var listLeavesId = listLeaves.Select(x => x.Id);
                var tempIdShifts = listShiftDetails.Select(x => x.ShiftExchangeApplicationId).ToList();
                var listShifts = unitOfWork.GetRepository<ShiftExchangeApplication>(true).FindBy(x => tempIdShifts.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                var listShiftsId = listShifts.Select(x => x.Id);

                returnValue = JsonConvert.DeserializeObject<TargetDateDetailCollection>(targetPlan.JsonData);
                List<DateValueArgs> jsonActual1 = new List<DateValueArgs>();

                foreach (var dict in returnValue)
                {
                    var lUser = targetPlan.SAPCode;
                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == lUser && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.date));
                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == lUser && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.date));
                    int actualValue = 0;
                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, out actualValue);
                    if (targetPlan.Type == TypeTargetPlan.Target1)
                    {
                        //target1
                        if (actualValue == 0 && !string.IsNullOrEmpty(res.Value))
                        {
                            dict.value = res.Value;
                        }
                    }
                    else if (targetPlan.Type == TypeTargetPlan.Target2)
                    {
                        //target2
                        if (actualValue == 1 && !string.IsNullOrEmpty(res.Value))
                        {
                            dict.value = res.Value;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
            }
            return returnValue;
        }

        public static TargetDateDetail GetActualTargetByDate(this TargetPlanDetail targetPlanDetail, IUnitOfWork unitOfWork, DateTime date, bool forceViewAll = false)
        {
            TargetDateDetail returnValue = null;
            try
            {
                if (!date.Equals(DateTime.MinValue))
                {
                    returnValue = targetPlanDetail.GetActualTargetInfos(unitOfWork).GetByDate(date);
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        private static bool CompareDateRangeAndStringDate(DateTimeOffset fromDate, DateTimeOffset toDate, string compareStringDate)
        {
            var dateToCheck = fromDate;
            bool isStopResult = false;
            while (!isStopResult && DateTime.Compare(dateToCheck.Date, toDate.Date.AddDays(1)) < 0)
            {
                isStopResult = CompareString(dateToCheck.DateTime.ToSAPFormat(), compareStringDate) == 0;
                dateToCheck = dateToCheck.AddDays(1);
            }
            return isStopResult;
        }

        private static int CompareString(string input1, string input2)
        {
            return String.Compare(input1, input2, comparisonType: StringComparison.OrdinalIgnoreCase);
        }

        private static DateValueArgs GetLeaveOrShiftCode(TargetDateDetail dict, LeaveApplicationDetail leave, ShiftExchangeApplicationDetail shift, IEnumerable<LeaveApplication> listLeaves, IEnumerable<ShiftExchangeApplication> listShifts, out int actualValue)
        {
            DateValueArgs res = new DateValueArgs
            {
                Date = dict.date,
                Value = ""
            };
            actualValue = 0; //is actual 1
            int assignFrom = 0;
            if ((leave != null) && (shift == null))
                assignFrom = 1;
            else if ((leave == null) && (shift != null))
                assignFrom = 2;
            if ((leave != null) && (shift != null))
            {
                //check leave<>shift modified/created sau cùng thì lấy
                var theLeave = listLeaves.FirstOrDefault(x => x.Id == leave.LeaveApplicationId);
                var theShift = listShifts.FirstOrDefault(x => x.Id == shift.ShiftExchangeApplicationId);
                var date1 = (theLeave != null) ? ((theLeave.Modified != null) ? theLeave.Modified : theLeave.Created) : DateTimeOffset.MinValue;
                var date2 = (theShift != null) ? ((theShift.Modified != null) ? theShift.Modified : theShift.Created) : DateTimeOffset.MinValue;
                if (date1 >= date2)
                    assignFrom = 1;
                else
                    assignFrom = 2;
            }
            List<string> halfCodes = ConfigurationManager.AppSettings["validateTargetPlanTarget1"].Split(',').Select(x => x.Trim()).ToList();
            if (assignFrom == 1)
            {   //check is actual2 or 1
                if (halfCodes.Contains(leave.LeaveCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.date,
                    Value = leave.LeaveCode,
                    Status = leave.LeaveApplication.Status,
                    Type = "Leave"
                };
            }
            else if (assignFrom == 2)
            {
                if (halfCodes.Contains(shift.NewShiftCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.date,
                    Value = shift.NewShiftCode,
                    Status = shift.ShiftExchangeApplication.Status,
                    Type = "Shift"
                };
            }
            return res;
        }

    }
    #endregion
}
