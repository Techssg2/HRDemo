using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    class ShiftExchangeBO : ExternalExcution
    {
        private readonly ILogger _log = null;
        private readonly ITrackingBO _trackingBO = null;
        private readonly IUnitOfWork _uow = null;

        //: base(log, uow, "shiftExchangeDataInfo.json") { }
        public ShiftExchangeBO(ILogger log, IUnitOfWork uow, ShiftExchangeApplication shiftExchange, ITrackingBO trackingBO) : base(log, uow, "shiftExchangeDataInfo.json", shiftExchange, trackingBO)
        {
            _log = log;
            _trackingBO = trackingBO;
            _uow = uow;
        }
        public override async void ConvertToPayload()
        {

        }

        public override Task<object> GetData(string predicate, string[] param)
        {
            throw new NotImplementedException();
        }

        public override async Task SubmitData(bool allowSendToSAP)
        {
            var model = (ShiftExchangeApplication)_integrationEntity;
            if (model != null)
            {
                ItemId = model.Id;
                var items = new List<ISAPEntity>();
                Dictionary<string, TargetDateDetailCollection> actualTargetDic = new Dictionary<string, TargetDateDetailCollection>();
                Func<User, Guid, TypeTargetPlan, TargetDateDetailCollection> GetActualTarget = (User user, Guid periodId, TypeTargetPlan targetType) =>
                {
                    TargetDateDetailCollection returnTargetDateDetail = null;
                    try
                    {
                        string key = user.Id.ToString() + periodId.ToString() + (targetType == TypeTargetPlan.Target1 ? "1" : "2");
                        if (!actualTargetDic.ContainsKey(key))
                        {
                            if (targetType == TypeTargetPlan.Target1)
                            {
                                TargetPlanDetail targetPlan1 = user.GetTargetPlan1(_uow, periodId, true);
                                targetPlan1.Type = TypeTargetPlan.Target1;
                                TargetDateDetailCollection actual1Details = targetPlan1.GetActualTargetInfos(_uow);
                                actualTargetDic.Add(key, actual1Details);
                            }
                            else if (targetType == TypeTargetPlan.Target2)
                            {
                                TargetPlanDetail targetPlan2 = user.GetTargetPlan2(_uow, periodId, true);
                                targetPlan2.Type = TypeTargetPlan.Target2;
                                TargetDateDetailCollection actual2Details = targetPlan2.GetActualTargetInfos(_uow);
                                actualTargetDic.Add(key, actual2Details);
                            }
                        }

                        returnTargetDateDetail = actualTargetDic[key];
                    }
                    catch
                    {
                        returnTargetDateDetail = null;
                    }
                    return returnTargetDateDetail;
                };


                if (model.Status == Const.Status.completed)
                {
                    foreach (var item in model.ExchangingShiftItems)
                    {
                        #region Collect shift code Info
                        User user = item.User;
                        DateTime currentDate = item.ShiftExchangeDate.DateTime.ToLocalTime();
                        TargetPlanPeriod period = currentDate.GetTargetPlanPeriodByDate(_uow, true);
                        Guid periodId = period.Id;
                        TypeTargetPlan typeTarget = TypeTargetPlan.Target1;
                        string currentShiftCode = item.CurrentShiftCode;
                        string newShiftCode = item.NewShiftCode;
                        bool allowDel = false;
                        if (currentShiftCode.IsShiftOfTarget2())
                        {
                            typeTarget = TypeTargetPlan.Target2;
                        }
                        ShiftSetResponceSAPViewModel sapTargetInfo = await user.GetCurrentShiftSetFromSAP(_uow, currentDate, _log, _trackingBO);
                        TargetDateDetailCollection currentActualTarget1 = GetActualTarget(user, periodId, TypeTargetPlan.Target1);
                        TargetDateDetail target1Detail = currentActualTarget1.GetByDate(currentDate);

                        TargetDateDetailCollection currentActualTarget = GetActualTarget(user, periodId, typeTarget);
                        if (currentActualTarget != null)
                        {
                            TargetDateDetail targetDetail = currentActualTarget.GetByDate(currentDate);
                            newShiftCode = targetDetail.value;

                            if (
                                //Change from target 2 -> target 1
                                //The new target 1 is the same old target 1 => delete target 2
                                //The new target 1 is NOT the same old target 1 => delete target 2 And Use the second payload to change Target 1
                                !item.NewShiftCode.IsShiftOfTarget2() && item.CurrentShiftCode.IsShiftOfTarget2())
                            {
                                allowDel = true;
                            }
                        }
                        #endregion

                        #region Check the case need to payload
                        //Use in case:
                        //Change from ShiftCode of target 2 to new Shift code of target 1
                        //And new ShiftCode has value difference with the current shift code 1
                        bool needToUse2Payloads = false;
                        string strNewShift = item.NewShiftCode;
                        if (item.CurrentShiftCode.IsShiftOfTarget2() &&  !item.NewShiftCode.IsShiftOfTarget2() && !item.NewShiftCode.Equals(sapTargetInfo.Shift1, StringComparison.OrdinalIgnoreCase))
                        {
                            needToUse2Payloads = true;
                            strNewShift = sapTargetInfo.Shift1;
                        }
                        #endregion

                        var data = new ShiftExchangeDataInfo
                        {
                            EmployeeCode = item.User.SAPCode,
                            Date = currentDate.ToSAPFormat(),
                            CurrentShift = item.CurrentShiftCode,
                            NewShift = strNewShift,
                            RequestFrom = model.UserCreatedBy.SAPCode,
                            IsCheckedERD = item.IsERD ? "1" : "0",
                            Del_flag = allowDel ? "X" : ""
                        };
                        items.Add(data);

                        #region Prepare new payload to change the shift code of target 1
                        if(needToUse2Payloads)
                        {
                            var data2 = new ShiftExchangeDataInfo
                            {
                                EmployeeCode = item.User.SAPCode,
                                Date = currentDate.ToSAPFormat(),
                                CurrentShift = sapTargetInfo.Shift1,
                                NewShift = item.NewShiftCode,
                                RequestFrom = model.UserCreatedBy.SAPCode,
                                IsCheckedERD = item.IsERD ? "1" : "0",
                                Del_flag = allowDel ? "" : ""
                            };
                            items.Add(data2);
                        }
                        #endregion
                    }
                    var trackingRequests = await AddTrackingRequests(items, "Employee");
                    foreach (var item in trackingRequests)
                    {
                        await base.SubmitAPIWithTracking(item, allowSendToSAP);
                    }
                }
                else if (model.Status == Const.Status.cancelled)
                {
                   
                    foreach (var item in model.ExchangingShiftItems)
                    {
                        User user = item.User;
                        DateTime currentDate = item.ShiftExchangeDate.DateTime.ToLocalTime();
                        TargetPlanPeriod period = currentDate.GetTargetPlanPeriodByDate(_uow, true);
                        Guid periodId = period.Id;
                        string currentShiftCode = item.NewShiftCode;
                        string newShiftCode = item.CurrentShiftCode;
                        TypeTargetPlan typeTarget = TypeTargetPlan.Target1;
                        bool allowDel = false;
                        if(currentShiftCode.IsShiftOfTarget2())
                        {
                            typeTarget = TypeTargetPlan.Target2;
                        }

                        #region Check the case need to payload
                        //Use in case:
                        //Change from ShiftCode of target 2 to new Shift code of target 1
                        //And new ShiftCode has value difference with the current shift code 1
                        TargetDateDetailCollection revokeCurrentActualTarget1 = GetActualTarget(user, periodId, TypeTargetPlan.Target1);
                        TargetDateDetail oldTarget1 = revokeCurrentActualTarget1.GetByDate(currentDate);


                        bool needToUse2Payloads = false;
                        string strCurrentShift = item.NewShiftCode;
                        if (newShiftCode.IsShiftOfTarget2() && !currentShiftCode.IsShiftOfTarget2() && !strCurrentShift.Equals(oldTarget1.value, StringComparison.OrdinalIgnoreCase))
                        {
                            needToUse2Payloads = true;
                            newShiftCode = oldTarget1.value;
                        }
                        #endregion

                        TargetDateDetailCollection currentActualTarget = GetActualTarget(user, periodId, typeTarget);
                        if(currentActualTarget != null)
                        {
                            TargetDateDetail targetDetail = currentActualTarget.GetByDate(currentDate);
                            if((string.IsNullOrEmpty(targetDetail.value) || newShiftCode.Equals(item.CurrentShiftCode)) && item.CurrentShiftCode.IsShiftOfTarget2() && !newShiftCode.IsShiftOfTarget2())
                            {
                                allowDel = true;
                            }
                        }


                        var data = new ShiftExchangeDataInfo
                        {
                            EmployeeCode = item.User.SAPCode,
                            Date = currentDate.ToSAPFormat(),
                            CurrentShift = currentShiftCode,
                            NewShift = newShiftCode,
                            RequestFrom = model.UserCreatedBy.SAPCode,
                            IsCheckedERD = item.IsERD ? "1" : "0",
                            Del_flag = allowDel ? "X" : ""
                        };
                        items.Add(data);

                        #region Prepare new payload to change the shift code of target 1
                        if (needToUse2Payloads)
                        {
                            var data2 = new ShiftExchangeDataInfo
                            {
                                EmployeeCode = item.User.SAPCode,
                                Date = currentDate.ToSAPFormat(),
                                CurrentShift = newShiftCode,
                                NewShift = item.CurrentShiftCode,
                                RequestFrom = model.UserCreatedBy.SAPCode,
                                IsCheckedERD = item.IsERD ? "1" : "0",
                                Del_flag = allowDel ? "" : ""
                            };
                            items.Add(data2);
                        }
                        #endregion
                    }
                    var trackingRequests = await AddTrackingRequests(items, "Employee");
                    foreach (var item in trackingRequests)
                    {
                        await base.SubmitAPIWithTracking(item, allowSendToSAP);
                    }
                }
            }
        }
    }
}
