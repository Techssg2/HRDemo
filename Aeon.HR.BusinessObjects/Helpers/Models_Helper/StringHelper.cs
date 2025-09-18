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
using System.Configuration;
using System.Dynamic;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Utilities;

namespace Aeon.HR.BusinessObjects
{
    #region String Helper
    public static class StringHelper
    {
        public static User GetUserByUserSAP(this string sapCode, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            User returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<User>(forceViewAll).GetSingle(x => x.SAPCode.Equals(sapCode));
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static User GetUserById(this Guid userID, IUnitOfWork unitOfWork, bool forceViewAll = false)
        {
            User returnValue = null;
            try
            {
                returnValue = unitOfWork.GetRepository<User>(forceViewAll).GetSingle(x => x.Id == userID);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static List<LeaveApplicationDetail> GetRevokeLeaveApplicationDetails(this string sapCode, IUnitOfWork unitOfWork, DateTimeOffset fromDate, DateTimeOffset toDate, bool forceViewAll = false)
        {
            List<LeaveApplicationDetail> returnValue = new List<LeaveApplicationDetail>();
            try
            {
                var leaveApplications = unitOfWork.GetRepository<LeaveApplication>(forceViewAll).FindBy(cLeave => cLeave.UserSAPCode == sapCode && cLeave.Status == "Cancelled");

                if (leaveApplications.Any())
                {
                    var leaveApplicationIDs = leaveApplications.Select(x => x.Id);
                    var revokedLeaveApplicationIDs = leaveApplications.FilterLeaveApplication_WasRevoked(unitOfWork, forceViewAll);
                    if (revokedLeaveApplicationIDs.Any())
                    {
                        var la_DetailsList = leaveApplications.Where(x => revokedLeaveApplicationIDs.Contains(x.Id)).Select(x => x.LeaveApplicationDetails).ToList();
                        foreach (var currenntLA in la_DetailsList)
                        {
                            returnValue.AddRange(currenntLA);
                        }
                    }
                }
            }
            catch
            {
                returnValue = new List<LeaveApplicationDetail>();
            }
            return returnValue;
        }

        public static List<LeaveApplicationDetail> GetRevokeLeaveApplicationDetails(this List<string> sapCode, IUnitOfWork unitOfWork, DateTimeOffset fromDate, DateTimeOffset toDate, bool forceViewAll = false)
        {
            List<LeaveApplicationDetail> returnValue = new List<LeaveApplicationDetail>();
            try
            {
                var leaveApplications = unitOfWork.GetRepository<LeaveApplication>(forceViewAll).FindBy(cLeave => sapCode.Contains(cLeave.UserSAPCode) && cLeave.Status == "Cancelled").ToList();

                if (leaveApplications.Any())
                {
                    var leaveApplicationIDs = leaveApplications.Select(x => x.Id).ToList();
                    var revokedLeaveApplicationIDs = leaveApplications.FilterLeaveApplication_WasRevoked(unitOfWork, forceViewAll);
                    if (revokedLeaveApplicationIDs.Any())
                    {
                        var la_DetailsList = leaveApplications.Where(x => revokedLeaveApplicationIDs.Contains(x.Id)).Select(x => x.LeaveApplicationDetails).ToList();
                        foreach (var currenntLA in la_DetailsList)
                        {
                            returnValue.AddRange(currenntLA);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = new List<LeaveApplicationDetail>();
            }
            return returnValue;
        }

        public static List<ShiftExchangeApplicationDetail> GetRevokeShiftExchangeApplicationDetails(this List<string> sapCode, IUnitOfWork unitOfWork, DateTimeOffset fromDate, DateTimeOffset toDate, bool forceViewAll = false)
        {
            List<ShiftExchangeApplicationDetail> returnValue = new List<ShiftExchangeApplicationDetail>();
            try
            {
                var shiftExchangeApplications = unitOfWork.GetRepository<ShiftExchangeApplication>(forceViewAll).FindBy(shExchange => shExchange.Status == "Cancelled" &&
                shExchange.ExchangingShiftItems.Any(y => fromDate <= y.ShiftExchangeDate && y.ShiftExchangeDate <= toDate)).ToList();

                if (shiftExchangeApplications.Any())
                {
                    var shiftExchangeApplicationIDs = shiftExchangeApplications.Select(x => x.Id).ToList();
                    var revokedLeaveApplicationIDs = shiftExchangeApplications.FilterShiftExchangeApplication_WasRevoked(unitOfWork, forceViewAll);
                    if (revokedLeaveApplicationIDs.Any())
                    {
                        var shiftExchange_DetailsList = shiftExchangeApplications.Where(x => revokedLeaveApplicationIDs.Contains(x.Id)).Select(x => x.ExchangingShiftItems).ToList();
                        foreach (var currenntShiftDetails in shiftExchange_DetailsList)
                        {
                            returnValue.AddRange(currenntShiftDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = new List<ShiftExchangeApplicationDetail>();
            }
            return returnValue;
        }

        //Check half day working shift code
        public static bool IsShiftOfTarget2(this string shiftCode)
        {
            bool returnValue = false;
            try
            {
                if (!string.IsNullOrEmpty(shiftCode))
                {
                    string strShiftOfTarget2 = ConfigurationManager.AppSettings["validateTargetPlanTarget1"];
                    if (!string.IsNullOrEmpty(strShiftOfTarget2))
                    {
                        List<string> ShiftOfTarget2 = strShiftOfTarget2.Split(',').Select(x => x.Trim()).ToList();
                        returnValue = ShiftOfTarget2.Any(x => !string.IsNullOrEmpty(x) && x.Equals(shiftCode, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }

        public static TypeTargetPlan GetTypeTargetPlan(this string shiftCode)
        {
            TypeTargetPlan returnValue = TypeTargetPlan.Target1;
            try
            {
                if (!string.IsNullOrEmpty(shiftCode))
                {
                    string strShiftOfTarget2 = ConfigurationManager.AppSettings["validateTargetPlanTarget1"];
                    if (!string.IsNullOrEmpty(strShiftOfTarget2))
                    {
                        List<string> ShiftOfTarget2 = strShiftOfTarget2.Split(',').Select(x => x.Trim()).ToList();
                        if (ShiftOfTarget2.Any(x => !string.IsNullOrEmpty(x) && x.Equals(shiftCode, StringComparison.OrdinalIgnoreCase)))
                        {
                            returnValue = TypeTargetPlan.Target2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = TypeTargetPlan.Target1;
            }
            return returnValue;
        }

        public static string GetAsURLParams (this string url,  object dataObj)
        {
            string returnValue = url;
            try
            {
                if(dataObj != null)
                {
                    List<string> paramsArray = new List<string>();
                    if (dataObj.GetType().Equals(typeof(ExpandoObject)))
                    {
                        ExpandoObject expObj = (ExpandoObject)dataObj;
                        paramsArray.AddRange(expObj.Select(p => $"{p.Key}={p.Value}").ToArray());
                    }
                    else
                    {
                        Func<PropertyInfo, object, string> GetAsURLParam = (PropertyInfo p, object o) =>
                        {
                            string strReturn = string.Empty;
                            try
                            {
                                strReturn = p.GetValue(o) + string.Empty;
                            }
                            catch
                            {
                                strReturn = string.Empty;
                            }
                            return strReturn;
                        };

                        PropertyInfo[] properties = dataObj.GetType().GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (property != null && !string.IsNullOrEmpty(property.Name))
                            {
                                paramsArray.Add($"{property.Name}={GetAsURLParam(property, dataObj)}");
                            }
                        }
                    }


                    if(paramsArray != null && paramsArray.Any())
                    {
                        returnValue = $"{url}?{paramsArray.Aggregate((x, y) => x + "&" + y)}";
                    }
                }
            }
            catch
            {
                returnValue = url;
            }
            return returnValue;
        }

        public static T ParseTo<T>(this string url)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(url);
            }
            catch
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }

        #region Convert type
        public static DateTime GetAsDateTime(this string datetime, string formatDate = "")
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                if (string.IsNullOrEmpty(formatDate))
                {
                    formatDate = StaticValue.dateFormat;
                }
                returnValue = DateTime.ParseExact(datetime, formatDate, DateTimeFormatInfo.InvariantInfo);
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public static int GetAsInt(this string strInt)
        {
            int returnValue = 0;
            try
            {
                int.TryParse(strInt, out returnValue);
            }
            catch
            {
                returnValue = 0;
            }
            return returnValue;
        }
        #endregion

        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static Object GetWorkflowItem(this Guid itemId, IUnitOfWork uow)
        {
            var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
            var workflowModelTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
            foreach (var workflowModelType in workflowModelTypes)
            {
                var repository = DynamicInvoker.InvokeGeneric(uow, "GetRepository", workflowModelType);
                var item = (DynamicInvoker.Invoke(repository, "FindById", itemId));
                if(item != null)
                {
                    return item;
                }
            }
            return null;
        }
        public static WorkflowEntity GetWorkflowEntity(this Guid itemId, IUnitOfWork uow)
        {
            var item = itemId.GetWorkflowItem(uow);
            if (item != null)
            {
                return item as WorkflowEntity; 
            }
            return null;
        }

        public static StatusItemInfo GetStatusItemInfo(this Guid itemId, IUnitOfWork uow)
        {
            StatusItemInfo returnValue = new StatusItemInfo();
            try
            {
                if(itemId != Guid.Empty && !(uow is null))
                {
                    WorkflowEntity wfwEntity = itemId.GetWorkflowEntity(uow);
                    if(wfwEntity != null)
                    {
                        returnValue.ItemId = itemId;
                        returnValue.Status = wfwEntity.Status;
                    }
                }
            }
            catch
            {
                returnValue = new StatusItemInfo();
            }
            return returnValue;
        }
    }
    #endregion
}
