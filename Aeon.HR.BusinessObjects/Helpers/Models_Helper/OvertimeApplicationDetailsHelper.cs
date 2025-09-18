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

namespace Aeon.HR.BusinessObjects
{
    #region OvertimeApplicationDetailArgs_Helper
    public static class OvertimeApplicationDetailArgs_Helper
    {
        //public static CheckingResultCollection Check_OT_Valid(this OvertimeApplicationArgs overtimeApplication, IUnitOfWork unitOfWork, ShiftCodeDetailCollection ShiftCodeData, bool forceViewAll = false)
        //{
        //    CheckingResultCollection returnValue = new CheckingResultCollection();
        //    try
        //    {
        //        foreach (OvertimeApplicationDetailArgs current_OT_Details in overtimeApplication.OvertimeList)
        //        {
        //            #region Get user of OT details Item
        //            User user = null;
        //            if (overtimeApplication.Type == OverTimeType.ManagerApplyForEmployee)
        //            {
        //                user = current_OT_Details.SAPCode.GetUserByUserSAP(unitOfWork);
        //            }
        //            else
        //            {
        //                user = overtimeApplication.UserSAPCode.GetUserByUserSAP(unitOfWork);
        //            }
        //            #endregion

        //            returnValue.Add(current_OT_Details.Check_OT_Valid(user, unitOfWork, ShiftCodeData, forceViewAll));

        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return returnValue;
        //}

        public static OTCheckingResult Check_OT_Valid(this OvertimeApplicationDetailArgs overtimeDetail, User user, IUnitOfWork unitOfWork, ShiftCodeDetailCollection ShiftCodeData, bool forceViewAll = false)
        {
            OTCheckingResult returnValue = new OTCheckingResult();
            try
            {
                //Check date is well-form
                DateTime otDate = overtimeDetail.Date.GetAsDateTime();
                if (otDate == DateTime.MinValue)
                {
                    returnValue.success = false;
                    returnValue.errorCode = "COMMON_DATE_TIME_WRONG_FORMAT";
                    returnValue.message = overtimeDetail.Date;
                    return returnValue;
                }

                //Check Target Plan exist
                TargetPlan targetPlan = user.GetTargetPlanByDate(unitOfWork, otDate);
                if (targetPlan is null || !targetPlan.Status.Equals(Const.Status.completed))
                {
                    returnValue.success = false;
                    returnValue.errorCode = "OVERTIME_APPLICATION_NEED_TO_CREATE_TARGET_PLAN_FIRST";
                    returnValue.message = $"{user.FullName} - {user.SAPCode}";
                    return returnValue;
                }

                //Check exist any in-process shift exchange
                List<ShiftExchangeApplication> shiftExchanges = user.GetProcessingShiftExchangesByDate(unitOfWork, otDate);
             
                if (!(shiftExchanges is null) && shiftExchanges.Any())
                {
                    returnValue.success = false;
                    returnValue.errorCode = "OVERTIME_APPLICATION_NEED_TO_COMPLETED_SHIFT_EXCHANGE_FIRST";
                    returnValue.message = $"{shiftExchanges.First().ReferenceNumber}";
                    return returnValue;
                }

                TargetPlanDetail targetPlanDetails = user.GetTargetPlan1(unitOfWork, otDate, forceViewAll);
                if (targetPlanDetails is null)
                {
                    returnValue.success = false;
                    returnValue.errorCode = "OVERTIME_APPLICATION_NEED_TO_CREATE_TARGET_PLAN_FIRST";
                    returnValue.message = $"{user.FullName} - {user.SAPCode}";
                    return returnValue;
                }
                else
                {
                    Func<DateTime, DateTime, OTCheckingResult> CheckOTTime = delegate (DateTime startTime, DateTime endTime)
                    {

                        OTCheckingResult returnCheckingResult = new OTCheckingResult();
                        OT_CheckingResult checkingResult = new OT_CheckingResult();
                        try
                        {
                            TargetDateDetail targetDetail = null;
                            targetDetail = user.GetActualTarget1_ByDate(unitOfWork, startTime, forceViewAll);
                            checkingResult = targetDetail.Check_ActualOT_Time(startTime, endTime, ShiftCodeData, forceViewAll);

                            switch (checkingResult.status)
                            {
                                case OT_CheckingStatus.OK:
                                    returnCheckingResult.success = true;
                                    returnCheckingResult.errorCode = string.Empty;
                                    returnCheckingResult.message = string.Empty;
                                    returnCheckingResult.calculatedActualHoursFrom = checkingResult.calculatedActualHoursFrom;
                                    returnCheckingResult.calculatedActualHoursTo = checkingResult.calculatedActualHoursTo;

                                    break;
                                case OT_CheckingStatus.INSIDE_WORKING_TIME:
                                    returnCheckingResult.success = false;
                                    returnCheckingResult.errorCode = "OVERTIME_APPLICATION_INSIDE_WORKING_TIME";
                                    returnCheckingResult.message = $"{user.FullName} - {targetDetail.GetWorkingTimeInfo(ShiftCodeData)} {overtimeDetail.GetOvertimeDetailInfo(true)}";
                                    break;
                                case OT_CheckingStatus.SHIFTCODE_NOT_FOUND:
                                    returnCheckingResult.success = false;
                                    returnCheckingResult.errorCode = "OVERTIME_APPLICATION_SHIFTCODE_NOT_FOUND";
                                    returnCheckingResult.message = $"{user.FullName} - Shift code {targetDetail.value}";
                                    break;
                                case OT_CheckingStatus.OVERLAP_WORKING_TIME:
                                    returnCheckingResult.success = false;
                                    returnCheckingResult.errorCode = "OVERTIME_APPLICATION_COVER_WORKING_TIME";
                                    returnCheckingResult.message = $"{user.FullName} - {targetDetail.GetWorkingTimeInfo(ShiftCodeData)} {overtimeDetail.GetOvertimeDetailInfo(true)}";
                                    returnCheckingResult.calculatedActualHoursFrom = checkingResult.calculatedActualHoursFrom;
                                    returnCheckingResult.calculatedActualHoursTo = checkingResult.calculatedActualHoursTo;
                                    break;
                                //case OT_CheckingStatus.DAY_OFF_CAN_NOT_OT:
                                //    returnCheckingResult.success = false;
                                //    returnCheckingResult.errorCode = "OVERTIME_APPLICATION_DAY_OFF_CAN_NOT_OT";
                                //    returnCheckingResult.message = startTime.ToString("dd/MM/yyyy");
                                //    break;
                                case OT_CheckingStatus.COVER_WORKING_TIME:
                                    returnCheckingResult.success = false;
                                    returnCheckingResult.errorCode = "OVERTIME_APPLICATION_COVER_WORKING_TIME";
                                    returnCheckingResult.message = $"{user.FullName} - {targetDetail.GetWorkingTimeInfo(ShiftCodeData)} {overtimeDetail.GetOvertimeDetailInfo(true)}";
                                    break;
                                case OT_CheckingStatus.NONE:
                                    returnCheckingResult.success = false;
                                    returnCheckingResult.errorCode = string.Empty;
                                    returnCheckingResult.message = string.Empty;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch
                        {
                            checkingResult = new OT_CheckingResult();
                        }
                        return returnCheckingResult;
                    };

                    #region Check Actual Time
                    returnValue = CheckOTTime(overtimeDetail.GetActual_StartTime(), overtimeDetail.GetActual_EndTime());
                    #endregion
                }
            }
            catch
            {
            }
            return returnValue;
        }

        public static OTCheckingResult Validate_OT_Valid(this OvertimeApplicationDetailArgs overtimeDetail, User user, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            OTCheckingResult returnValue = new OTCheckingResult();
            try
            {
                DateTime otDate = overtimeDetail.Date.GetAsDateTime();
                if (otDate == DateTime.MinValue)
                {
                    returnValue.success = false;
                    returnValue.errorCode = "COMMON_DATE_TIME_WRONG_FORMAT";
                    returnValue.message = overtimeDetail.Date;
                    return returnValue;
                }
                // HR-970 OT: Không cho đăng ký OT nếu chưa có target plan completed
                if (!user.HasTargetPlanCompleted(unitOfWork, otDate, false))
                {
                    returnValue.success = false;
                    returnValue.errorCode = "NEED_TO_COMPLETED_TARGET_PLAN_FIRST";
                    returnValue.message = $"{user.FullName} - {user.SAPCode}";
                    return returnValue;
                }

                // HR-1086: [OT APPLICATION] Chặn nếu đăng ký OT trùng vào khung giờ của lệnh OT đã đăng ký thì không cho
                var existOvertimeApplication = user.HasExistOvertimeApplication(unitOfWork, overtimeDetail);
                if (existOvertimeApplication.Any())
                {
                    returnValue.success = false;
                    returnValue.errorCode = "OVERTIME_APPLICATION_EXISTS_ITEM";
                    returnValue.description = string.Join(", ", existOvertimeApplication);
                    returnValue.message = $"{user.FullName} - {user.SAPCode}";
                    return returnValue;
                }
                else returnValue.success = true;
            }
            catch
            {
                returnValue.success = true;
            }
            return returnValue;
        }

        public static string GetOvertimeDetailInfo(this OvertimeApplicationDetailArgs overtimeDetail, bool actualTime = false)
        {
            string returnValue = string.Empty;
            try
            {
                string startDate = actualTime ? overtimeDetail.ActualHoursFrom : overtimeDetail.ProposalHoursFrom;
                string endDate = actualTime ? overtimeDetail.ActualHoursTo : overtimeDetail.ProposalHoursTo;
                string prefix = actualTime ? "Actual time" : "Proposal time";
                returnValue = $"( {prefix} {startDate} - {endDate})";
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }

        public static DateTime GetProposal_StartTime(this OvertimeApplicationDetailArgs overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                if (!string.IsNullOrEmpty(overtimeDetail.ProposalHoursFrom))
                {
                    DateTime otDate = overtimeDetail.Date.GetAsDateTime();
                    if (otDate != DateTime.MinValue)
                    {
                        returnValue = otDate.SetTime(overtimeDetail.ProposalHoursFrom, "HH:mm");
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static DateTime GetProposal_EndTime(this OvertimeApplicationDetailArgs overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                if (!string.IsNullOrEmpty(overtimeDetail.ProposalHoursTo))
                {
                    DateTime otDate = overtimeDetail.Date.GetAsDateTime();

                    if (otDate != DateTime.MinValue)
                    {
                        //Checking If start time greater than end time
                        //It means Overtime passed over the next day
                        int startTime = int.Parse(overtimeDetail.ProposalHoursFrom.Replace(":", string.Empty));
                        int endTime = int.Parse(overtimeDetail.ProposalHoursTo.Replace(":", string.Empty));
                        if (startTime < endTime)
                        {
                            returnValue = otDate.SetTime(overtimeDetail.ProposalHoursTo, "HH:mm");
                        }
                        else
                        {
                            otDate = otDate.AddDays(1);
                            returnValue = otDate.SetTime(overtimeDetail.ProposalHoursTo, "HH:mm");
                        }
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static DateTime GetActual_StartTime(this OvertimeApplicationDetailViewModel overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                string time = overtimeDetail.ActualHoursFrom;
                if (!string.IsNullOrEmpty(time))
                {
                    DateTime otDate = overtimeDetail.Date.Date;
                    if (otDate != DateTime.MinValue)
                    {
                        returnValue = otDate.SetTime(time, "HH:mm");
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static DateTime GetActual_StartTime(this OvertimeApplicationDetailArgs overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                string time = overtimeDetail.ActualHoursFrom;
                if (!string.IsNullOrEmpty(time))
                {
                    DateTime otDate = overtimeDetail.Date.GetAsDateTime();
                    if (otDate != DateTime.MinValue)
                    {
                        returnValue = otDate.SetTime(time, "HH:mm");
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static DateTime GetActual_EndTime(this OvertimeApplicationDetailViewModel overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                if (!string.IsNullOrEmpty(overtimeDetail.ActualHoursFrom))
                {
                    DateTime otDate = overtimeDetail.Date.Date;
                    if (otDate != DateTime.MinValue)
                    {
                        //Checking If start time greater than end time
                        //It means Overtime passed over the next day
                        int startTime = int.Parse(overtimeDetail.ActualHoursFrom.Replace(":", string.Empty));
                        int endTime = int.Parse(overtimeDetail.ActualHoursTo.Replace(":", string.Empty));
                        if (startTime < endTime)
                        {
                            returnValue = otDate.SetTime(overtimeDetail.ActualHoursTo, "HH:mm");
                        }
                        else
                        {
                            otDate = otDate.AddDays(1);
                            returnValue = otDate.SetTime(overtimeDetail.ActualHoursTo, "HH:mm");
                        }
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static DateTime GetActual_EndTime(this OvertimeApplicationDetailArgs overtimeDetail)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                if (!string.IsNullOrEmpty(overtimeDetail.ActualHoursFrom))
                {
                    DateTime otDate = overtimeDetail.Date.GetAsDateTime();
                    if (otDate != DateTime.MinValue)
                    {
                        //Checking If start time greater than end time
                        //It means Overtime passed over the next day
                        int startTime = int.Parse(overtimeDetail.ActualHoursFrom.Replace(":", string.Empty));
                        int endTime = int.Parse(overtimeDetail.ActualHoursTo.Replace(":", string.Empty));
                        if (startTime < endTime)
                        {
                            returnValue = otDate.SetTime(overtimeDetail.ActualHoursTo, "HH:mm");
                        }
                        else
                        {
                            otDate = otDate.AddDays(1);
                            returnValue = otDate.SetTime(overtimeDetail.ActualHoursTo, "HH:mm");
                        }
                    }
                }
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }
    }
    #endregion
}