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
    #region DateTime Helper
    public static class TargetPlanPeriod_Helper
    {
        public static TargetPlanPeriod GetTargetPlanPeriodByDate(this DateTime date, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            TargetPlanPeriod returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<TargetPlanPeriod>(forceViewAll).GetSingle(x => x.FromDate <= date.Date && x.ToDate >= date.Date);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static DateTime SetTime(this DateTime date, string timestring, string timeFormat = "HHmmss")
        {
            DateTime returnValue = date;
            try
            {
                string dateFormat = "yyyy-MM-dd";
                string strDate = date.ToString(dateFormat);
                strDate += " " + timestring;
                returnValue = DateTime.ParseExact(strDate, dateFormat + " " + timeFormat, null);
            }
            catch
            {
            }
            return returnValue;
        }

        public static HolidaySchedule GetPublicHoliday(this DateTime date, IUnitOfWork unitOfWork)
        {
            HolidaySchedule returnValue = null;
            try
            {
                List<HolidaySchedule> holidays = unitOfWork.GetRepository<HolidaySchedule>().GetAll().ToList();
                returnValue = holidays.FirstOrDefault(x => x != null && x.FromDate.ToLocalTime().Date <= date.Date && date.Date <= x.ToDate.ToLocalTime().Date);
            }
            catch(Exception ex)
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static bool IsPublicHoliday(this DateTime date, IUnitOfWork unitOfWork)
        {
            bool returnValue = false;
            try
            {
                var holiday = date.GetPublicHoliday(unitOfWork);
                returnValue = holiday != null;
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static string GetAsDateString(this DateTimeOffset dateTimeOffSet)
        {
            string returnValue = string.Empty;
            try
            {
                if(dateTimeOffSet != null)
                {
                    returnValue = dateTimeOffSet.DateTime.GetAsDateString();
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static string GetAsDateString(this DateTimeOffset? dateTimeOffSet)
        {
            string returnValue = string.Empty;
            try
            {
                if (dateTimeOffSet != null && dateTimeOffSet.HasValue)
                {
                    returnValue = dateTimeOffSet.Value.DateTime.GetAsDateString();
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static string GetAsDateString(this DateTime dateTime)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue = dateTime.GetAsDateString(StaticValue.dateFormat);
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static string GetAsDateString(this DateTime? dateTime)
        {
            string returnValue = string.Empty;
            try
            {
                returnValue = dateTime.GetAsDateString(StaticValue.dateFormat);
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }

        public static string GetAsDateString(this DateTime dateTime, string dateFormat)
        {
            string returnValue = string.Empty;
            try
            {
                if (dateTime != DateTime.MinValue)
                {
                    returnValue = dateTime.ToString(dateFormat);
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static string GetAsDateString(this DateTime? dateTime, string dateFormat)
        {
            string returnValue = string.Empty;
            try
            {
                if (dateTime.HasValue && dateTime.Value != DateTime.MinValue)
                {
                    returnValue = dateTime.Value.ToString(dateFormat);
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }
        public static IEnumerable<OvertimeApplicationDetail> GetOvertimeApplicationDetailByDate(this DateTime date, User user, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            IEnumerable<OvertimeApplicationDetail> returnValue = null;
            try
            {
                List<string> notInStatus = new List<string> { Const.Status.cancelled.ToLower(), Const.Status.rejected.ToLower(), Const.Status.draft.ToLower() };
                returnValue = unitOfWork.GetRepository<OvertimeApplicationDetail>(forceViewAll).FindBy(x =>
                x.OvertimeApplication != null && ((x.OvertimeApplication.Type == OverTimeType.ManagerApplyForEmployee && x.SAPCode.Equals(user.SAPCode)) || (x.OvertimeApplication.Type == OverTimeType.EmployeeSeftService && x.OvertimeApplication.UserSAPCode.Equals(user.SAPCode))) &&
                x.Date == date && !notInStatus.Contains(x.OvertimeApplication.Status.ToLower()));
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
