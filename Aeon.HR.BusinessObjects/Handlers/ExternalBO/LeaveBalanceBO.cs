using Aeon.HR.API.Helpers;
using Aeon.HR.BusinessObjects.ExternalHelper.SAP;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.ViewModels.ExternalItem;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.ExternalBO
{
    public class LeaveBalanceBO : ExternalExcution
    {
        private readonly ITargetPlanBO _targetPlanBO;
        private readonly ILogger _log;
        private readonly IUnitOfWork _uow;
        private readonly ITrackingBO _trackingBO;
        private List<LeaveQuotaMapping> LeaveQuotaMappings { get; set; }
        private List<ShiftCodeViewModel> TargetPlanQuoteMapping { get; set; }
        public LeaveBalanceBO(ILogger log, IUnitOfWork uow, LeaveApplication leaveApplication, ITrackingBO trackingBO, ITargetPlanBO targetPlanBO) : base(log, uow, "leaveBalanceInfo.json", leaveApplication, trackingBO)
        {
            var leaveQuotaMappings = JsonHelper.GetJsonContentFromFile("Mappings", "leave-quota-mapping.json");
            /*var targetPlanQuoteMapping = JsonHelper.GetJsonContentFromFile("Mappings", "target-plan-valid.json");*/
            if (!string.IsNullOrEmpty(leaveQuotaMappings))
            {
                var quotas = JsonConvert.DeserializeObject<List<LeaveQuotaMapping>>(leaveQuotaMappings);
                if (quotas.Any())
                {
                    LeaveQuotaMappings = quotas;
                }
            }


            _targetPlanBO = targetPlanBO;
            _log = log;
            _uow = uow;
            _trackingBO = trackingBO;
        }
        public override void ConvertToPayload()
        {
        }
        public override async Task SubmitData(bool allowSentoSAP)
        {
            var model = (LeaveApplication)_integrationEntity;
            if (model != null)
            {
                ItemId = model.Id;
            };

            var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
            if (shiftCode.Any())
            {
                TargetPlanQuoteMapping = shiftCode.ToList();
            }

            if (model.Status == Const.Status.completed)
            {
                //Completed leave application
                var leaveApplicationDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => x.LeaveApplicationId == model.Id);
                if (leaveApplicationDetails.Any())
                {
                    var items = new List<ISAPEntity>();
                    var departmentCode = string.IsNullOrEmpty(model.DivisionCode) ? model.DeptCode : model.DivisionCode;
                    var currentDepartment = await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.Code == departmentCode);
                    var isHQ = currentDepartment != null && currentDepartment.Any() && !currentDepartment.FirstOrDefault().IsStore;
                    foreach (var item in leaveApplicationDetails)
                    {
                        var dataInfo = new LeaveApplicationInfo
                        {
                            EmployeeCode = model.UserSAPCode,
                            UserEdoc = model.CreatedBy
                        };
                        dataInfo.FromDate = item.FromDate.LocalDateTime.ToSAPFormat();
                        dataInfo.ToDate = item.ToDate.LocalDateTime.ToSAPFormat();
                        if (item.FromDate.DateTime == item.ToDate.DateTime)
                        {
                            dataInfo.LeaveKind = item.LeaveCode;
                            items.Add(dataInfo);
                        }
                        else
                        {
                            dataInfo.FromDate = dataInfo.FromDate;
                            dataInfo.ToDate = dataInfo.ToDate;
                            dataInfo.LeaveKind = item.LeaveCode;
                            items.Add(dataInfo);

                        }
                    }
                    var trackingRequests = await AddTrackingRequests(items, "Employee");
                    foreach (var item in trackingRequests)
                    {
                        await base.SubmitAPIWithTracking(item, allowSentoSAP);
                    }

                }
            }
            else if (model.Status == Const.Status.cancelled)
            {
                
                //revoke leave application
                if (model.LeaveApplicationDetails != null && model.LeaveApplicationDetails.Any())
                {
                    User user = model.UserSAPCode.GetUserByUserSAP(_uow, true);
                    if (user != null)
                    {
                        Dictionary<string, TargetDateDetailCollection> actualTargetDic = new Dictionary<string, TargetDateDetailCollection>();
                        Func<Guid, TypeTargetPlan, TargetDateDetailCollection> GetActualTarget = (Guid periodId, TypeTargetPlan targetType) =>
                        {
                            TargetDateDetailCollection returnTargetDateDetail = null;
                            try
                            {
                                string key = periodId.ToString() + (targetType == TypeTargetPlan.Target1 ? "1" : "2");
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
                        var sapItems = new List<ISAPEntity>();
                        foreach (LeaveApplicationDetail leaveDetail in model.LeaveApplicationDetails)
                        {
                            DateTime fromDate = leaveDetail.FromDate.ToLocalTime().Date;
                            DateTime toDate = leaveDetail.ToDate.ToLocalTime().Date;
                            DateTime currentDate = fromDate;

                            while (currentDate <= toDate)
                            {
                                TargetPlanPeriod period = currentDate.GetTargetPlanPeriodByDate(_uow, true);
                                if (period != null)
                                {
                                    Guid periodId = period.Id;
                                    TypeTargetPlan targetType = TypeTargetPlan.Target1;
                                    // check tào lao
                                    /*if (leaveDetail.LeaveCode.EndsWith("1") || leaveDetail.LeaveCode.EndsWith("2"))
                                    {
                                        //check actual 2
                                        targetType = TypeTargetPlan.Target2;
                                    }*/

                                    // Task #10765
                                    var checkTypeTargetPlan = TargetPlanQuoteMapping.Where(x => x.Code == leaveDetail.LeaveCode).FirstOrDefault();
                                    if (!(checkTypeTargetPlan is null))
                                    {
                                        targetType = (TypeTargetPlan) checkTypeTargetPlan.Line;
                                    }
                                    string curentShiftCode = leaveDetail.LeaveCode;
                                    string newShiftCode = "";
                                    bool allowDel = false;
                                    TargetDateDetailCollection currentActualTarget = GetActualTarget(periodId, targetType);
                                    if (currentActualTarget != null)
                                    {
                                        TargetDateDetail targetDetail = currentActualTarget.GetByDate(currentDate);
                                        newShiftCode = targetDetail.value;
                                        if (string.IsNullOrEmpty(newShiftCode) && targetType == TypeTargetPlan.Target2)
                                        {
                                            allowDel = true;
                                        }
                                    }
                                    else
                                    {
                                        var currenShiftSet = await user.GetCurrentShiftSetFromSAP(_uow, currentDate, _log, _trackingBO);
                                        newShiftCode = currenShiftSet.Shift1;
                                        if (targetType == TypeTargetPlan.Target2)
                                        {
                                            allowDel = true;
                                        }
                                    }

                                    var data = new ShiftExchangeDataInfo
                                    {
                                        EmployeeCode = user.SAPCode,
                                        Date = currentDate.ToSAPFormat(),
                                        CurrentShift = curentShiftCode,
                                        NewShift = string.IsNullOrEmpty(newShiftCode) ? "V818" : newShiftCode, //SAP Trick: NewShift not empty allowed or equal CurrentShift => add V818 As default 
                                        RequestFrom = user.SAPCode,
                                        IsCheckedERD = "0",
                                        Del_flag = allowDel ? "X" : ""
                                    };
                                    sapItems.Add(data);
                                }
                                currentDate = currentDate.AddDays(1);
                            }
                        }
                        if (sapItems != null && sapItems.Any())
                        {
                            //switch to shift exchange
                            string bkJsonFile = _jsonFile;
                            _jsonFile = "shiftExchangeDataInfo.json";
                            var trackingRequests = await AddTrackingRequests(sapItems, "Employee");
                            foreach (var item in trackingRequests)
                            {
                                await base.SubmitAPIWithTracking(item, allowSentoSAP);
                            }
                            //return back to leave
                            _jsonFile = bkJsonFile;
                        }
                    }
                }
            }
        }
        public override async Task<object> GetData(string predicate, string[] predicateParameter)
        {

            HttpResponseMessage response = await base.GetDataExcution(predicate, predicateParameter);
            if (response != null && response.IsSuccessStatusCode && response.Content != null)
            {
                string httpResponseResult = await response.Content.ReadAsStringAsync();
                var responseResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<SAPAPIResultForArray>(httpResponseResult).D.Results);
                var jsonData = JsonHelper.GetJsonContentFromFile("Mappings", _jsonFile);
                var mappingFiles = GenericExtension<List<FieldMappingDTO>>.DeserializeObject(jsonData);
                var jsonResponce = ProcessingFields(mappingFiles, responseResult, TypeProcessingField.Get);
                var res = JsonConvert.DeserializeObject<List<LeaveBalanceResponceSAPViewModel>>(jsonResponce);
                if (res.Count > 0)
                {
                    foreach (var item in res)
                    {
                        if (!string.IsNullOrEmpty(item.AbsenceQuotaType))
                        {
                            item.AbsenceQuotaName = GetAbsenceQuotaTypeTextFromAbsenceQuotaType(item.AbsenceQuotaType);
                        }
                    }
                    return res;
                }
                else
                {
                    return new List<LeaveBalanceResponceSAPViewModel>()
                    {

                    };
                }
            }
            return null;
        }
        private string GetAbsenceQuotaTypeTextFromAbsenceQuotaType(string absenceQuotaType)
        {
            var quota = LeaveQuotaMappings.Where(x => x.AbsenceQuotaType == absenceQuotaType);
            if (quota.Any())
            {
                return quota.FirstOrDefault().AbsenceQuotaTypeText;
            }
            return string.Empty;
        }
    }
    public class LeaveQuotaMapping
    {
        public string AbsenceQuotaType { get; set; }
        public string AbsenceQuotaTypeText { get; set; }
    }
}