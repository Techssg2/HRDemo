using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetPlanTesting.ImportData;
using Aeon.HR.API.Helpers;
using Aeon.HR.ViewModels.PrintFormViewModel;
using System.Security;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.CodeDom.Compiler;

namespace Aeon.HR.BusinessObjects.Handlers.CB
{
    public class TargetPlanBO : ITargetPlanBO
    {
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;
        private readonly ILogger _logger;
        private readonly IEmployeeBO _employee;
        private readonly ISettingBO _settingBO;
        public TargetPlanBO(IUnitOfWork uow, IWorkflowBO workflowBO, ILogger logger, IEmployeeBO employee, ISettingBO settingBO)
        {
            _uow = uow;
            _workflowBO = workflowBO;
            _logger = logger;
            _employee = employee;
            _settingBO = settingBO;
        }
        public async Task<ResultDTO> GetValidUserSAPForTargetPlan(Guid id, Guid userId, Guid periodId)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();
            var listRes = new List<ValidUserForTargetPlanDTO>();
            try
            {
                var validUserLogin = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync<UserDepartmentMappingViewModel>(x => x.IsHeadCount && x.DepartmentId == id && x.UserId == userId);
                if (validUserLogin != null)
                {
                    var pendingDetailsSAPCode = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.SAPCode == validUserLogin.UserSAPCode && x.PeriodId == periodId);
                    var resignation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && x.UserSAPCode == validUserLogin.UserSAPCode);
                    if (resignation != null)
                    {
                        if (validUserLogin.StartDate.HasValue && resignation.OfficialResignationDate.Date > validUserLogin.StartDate.Value.Date)
                        {
                            validUserLogin.OfficialResignationDate = resignation.OfficialResignationDate;
                        }
                        else
                        {
                            validUserLogin.OfficialResignationDate = new DateTimeOffset(new DateTime(3000, 1, 1));
                        }

                    }
                    if (!pendingDetailsSAPCode.Any() || !pendingDetailsSAPCode.All(x => x.IsSent))
                    {
                        listRes.Add(new ValidUserForTargetPlanDTO { StartDate = validUserLogin.StartDate, SAPCode = validUserLogin.UserSAPCode, OfficialResignationDate = validUserLogin.OfficialResignationDate });
                    }

                }
                var department = await _uow.GetRepository<Department>().FindByIdAsync(id);
                if (department != null)
                {
                    //get G1
                    var g1 = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Grade == 1);
                    // find department have parentId = department.id and is G1 of Store
                    var listDept = (await _uow.GetRepository<Department>().FindByAsync(x => x.ParentId == department.Id && x.JobGradeId == g1.Id && x.IsStore)).Select(x => x.Id).ToList();
                    if (listDept.Count() > 0)
                    {
                        var res = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync<UserDepartmentMappingViewModel>(x => listDept.Contains(x.DepartmentId ?? Guid.Empty) && x.IsHeadCount == true && x.User.IsActivated);
                        if (res.Any())
                        {
                            foreach (var userMapping in res)
                            {
                                var resignation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.Status.Contains("Completed") && x.UserSAPCode == userMapping.UserSAPCode, "Created desc");
                                if (resignation != null)
                                {
                                    userMapping.OfficialResignationDate = resignation.OfficialResignationDate;
                                }
                            }
                            listRes.AddRange(res.Select(x => new ValidUserForTargetPlanDTO { SAPCode = x.UserSAPCode, StartDate = x.StartDate, OfficialResignationDate = x.OfficialResignationDate > x.StartDate ? x.OfficialResignationDate : null }).ToList());

                        }

                    }
                    var listResSapCodes = listRes.Select(x => x.SAPCode);
                    var isSentDataSAPCodes = (await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => listResSapCodes.Contains(x.SAPCode) && x.IsSent && x.PeriodId == periodId)).Select(y => y.SAPCode);
                    var existPendingTargetPlans = (await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => listResSapCodes.Contains(x.SAPCode) && x.PeriodId == periodId)).Select(y => y.SAPCode);
                    var notExistPendingTargetDetails = listResSapCodes.Where(x => !existPendingTargetPlans.Contains(x) && !isSentDataSAPCodes.Contains(x));
                    if (isSentDataSAPCodes != null && isSentDataSAPCodes.Any())
                    {
                        listRes = listRes.Where(x => !isSentDataSAPCodes.Contains(x.SAPCode)).ToList();
                    }
                    if (notExistPendingTargetDetails.Any())
                    {
                        listRes.AddRange(listRes.Where(x => notExistPendingTargetDetails.Contains(x.SAPCode)).ToList());
                    }

                }
                if (listRes.Any())
                {
                    var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync(periodId);
                    data.Data = listRes.Where(x => x.OfficialResignationDate.HasValue ? x.OfficialResignationDate.Value.Date > currentPeriod.FromDate.Date : true);
                    data.Count = listRes.Count;
                }
                else
                {
                    data.Data = new List<ValidUserForTargetPlanDTO>();
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "No Data" };
                }
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanDetails: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        public async Task<ResultDTO> GetPendingTargetPlanDetails(Guid id)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();
            try
            {
                IEnumerable<PendingTargetPlanDetailViewModel> targetPlanDetails = Enumerable.Empty<PendingTargetPlanDetailViewModel>();
                targetPlanDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>
                    (x => x.PendingTargetPlanId == id);
                data.Data = targetPlanDetails;
                data.Count = targetPlanDetails.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanDetails: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        public async Task<ResultDTO> GetPendingTargetPlanDetailFromSAPCodesAndPeriod(TargetPlanQuerySAPCodeArg args)
        {
            var result = new ResultDTO { };
            var data = new TargetPlanResultDTO();
            try
            {
                var currentLoginUser = await _uow.GetRepository<User>().FindByIdAsync(_uow.UserContext.CurrentUserId);
                var notIncludeUsers = new List<string>();
                IEnumerable<TargetPlanArg> targetPlans = Enumerable.Empty<TargetPlanArg>();
                targetPlans = await _uow.GetRepository<PendingTargetPlan>().FindByAsync<TargetPlanArg>
                    (x => x.PeriodId == args.PeriodId);
                var targetPlanIds = targetPlans.Select(x => x.Id).ToList();
                List<TargetPlanArgDetail> targetPlanDetails = (await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<TargetPlanArgDetail>
                       (x => args.SAPCodes.Contains(x.SAPCode) && x.PeriodId == args.PeriodId)).ToList();
                data.AllData = targetPlanDetails;
                var updateIsSentItems = targetPlanDetails.Where(x => args.SAPCodes.Contains(x.SAPCode)).ToList();
                if (updateIsSentItems.Count > 0 && args.VisibleSubmit)
                {
                    foreach (var item in updateIsSentItems)
                    {
                        if ((await IsG5UpHQOrG4Store(item.SAPCode, currentLoginUser.SAPCode)))
                        {
                            notIncludeUsers.Add(item.SAPCode);
                        }
                        item.IsSent = args.VisibleSubmit;
                    }
                }
                data.Data = updateIsSentItems.Where(x => !notIncludeUsers.Contains(x.SAPCode));
                data.Count = targetPlanDetails.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanDetailFromSAPCodesAndPeriod: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        private async Task<bool> IsG5UpHQOrG4Store(string checkSAPCode, string currentLoginSapCode)
        {
            var result = false;

            try
            {
                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == checkSAPCode);
                var jobGrade45 = await _uow.GetRepository<JobGrade>().FindByAsync(x => x.Title.ToUpper().Equals("G4") || x.Title.ToUpper().Equals("G5"));
                int g4 = jobGrade45.FirstOrDefault(x => x.Title.ToUpper().Equals("G4"))?.Grade ?? 4;
                int g5 = jobGrade45.FirstOrDefault(x => x.Title.ToUpper().Equals("G5"))?.Grade ?? 5;
                if (currentUser != null && currentUser.SAPCode != currentLoginSapCode && ((currentUser.JobGradeValue.Value >= g5 && !currentUser.IsStore.Value) || (currentUser.JobGradeValue.Value >= g4 && currentUser.IsStore.Value)))
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public async Task<ResultDTO> GetPendingTargetPlanFromDepartmentAndPeriod(TargetPlanQueryArg args)
        {
            var result = new ResultDTO { };
            try
            {
                var res = await GetPendingTargetPlan(args);
                if (res != null)
                {
                    result.Object = res;
                }
                else
                {
                    result.ErrorCodes = new List<int> { 1004 };
                    result.Messages = new List<string> { "Not Found" };
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanFromDepartmentAndPeriod: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        private async Task<TargetPlanArg> GetPendingTargetPlan(TargetPlanQueryArg args)
        {
            TargetPlanArg result = null;
            var targetPlans = await _uow.GetRepository<PendingTargetPlan>().FindByAsync<TargetPlanArg>
                (x => (x.DeptId.Value == args.DepartmentId)
                && x.PeriodId == args.PeriodId);

            var targetPlanIds = targetPlans.Select(x => x.Id).ToList();
            List<TargetPlanArgDetail> targetPlanDetails = (await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<TargetPlanArgDetail>
                   (x => targetPlanIds.Contains(x.PendingTargetPlanId) && args.SAPCodes.Contains(x.SAPCode))).ToList();

            if (targetPlanDetails.Any())
            {
                result = targetPlans.FirstOrDefault(x => (x.DivisionId.HasValue && x.DivisionId == args.DivisionId) && x.PeriodId == args.PeriodId);
            }
            return result;
        }
        private async Task<List<PendingTargetPlanDetail>> CheckExistPendingTargetPlans(TargetPlanQueryArg args)
        {

            List<PendingTargetPlanDetail> targetPlanDetails = (await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync
                   (x => args.SAPCodes.Contains(x.SAPCode) && x.PeriodId == args.PeriodId)).ToList();

            return targetPlanDetails;
        }
        private async Task<ResultDTO> ValidateTargetPlan(List<TargetPlanArgDetail> list, Guid periodId)
        {
            var res = new ResultDTO();
            // var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
            var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => !x.IsDeleted && !x.Code.Contains("*"));
            /*string invalidCode = ConfigurationManager.AppSettings["validateTargetPlanTarget1"];*/

            /*List<string> target2CodeList = invalidCode.Split(',').Select(x => x.Trim()).ToList();
            List<string> target2Args = invalidCode.Split(',').Select(x => x.Trim()).ToList();*/
            /*for (int i = 0; i < target2Args.Count; i++)
            {
                target2Args[i] = $"\"value\":\"{target2Args[i]}\"";
            }*/

            if (shiftCode.Any())
            {
                foreach (var item in list)
                {
                    if (!item.IsSubmitted)
                    {
                        if (item.Type == TypeTargetPlan.Target1)
                        {
                            await ValidateHolidays(true, res, item);
                            var obj = JsonConvert.DeserializeObject(item.JsonData);
                            //minimize json string to check contains string
                            item.JsonData = JsonConvert.SerializeObject(obj, Formatting.None);
                            var dict = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                            // validate shiftCode not exist
                            var shiftCodeFullDayInvalid = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY && !x.IsActive).Select(p => p.Code).ToList();
                            var inValid = dict.Where(x => !string.IsNullOrEmpty(x.Value) && shiftCodeFullDayInvalid.Contains(x.Value)).Select(p => p.Date.ToDateTime().ToString("dd/MM/yyyy")).ToList();
                            if (inValid.Any())
                            {
                                res.ErrorCodes.Add(17);
                                res.Messages.Add(item.SAPCode + " (" + string.Join(", ", inValid) + ")");
                            } else
                            {
                                shiftCodeFullDayInvalid = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY && x.IsActive).Select(p => p.Code).ToList();
                                inValid = dict.Where(x => !string.IsNullOrEmpty(x.Value) && !shiftCodeFullDayInvalid.Contains(x.Value)).Select(p => p.Date.ToDateTime().ToString("dd/MM/yyyy")).ToList();
                                if (inValid.Any())
                                {
                                    res.ErrorCodes.Add(1);
                                    res.Messages.Add(item.SAPCode + " (" + string.Join(", ", inValid) + ")");
                                }
                            }
                            
                        }
                        else if (item.Type == TypeTargetPlan.Target2)
                        {
                            await ValidateHolidays(false, res, item);
                            var dict = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                            var shiftCodeHalfDayInvalid = shiftCode.Where(x => x.Line == ShiftLine.HAFTDAY && !x.IsActive).Select(p => p.Code).ToList();
                            var inValid = dict.Where(x => !string.IsNullOrEmpty(x.Value) && shiftCodeHalfDayInvalid.Contains(x.Value)).Select(p => p.Date.ToDateTime().ToString("dd/MM/yyyy")).ToList();
                            if (inValid.Any())
                            {
                                res.ErrorCodes.Add(18);
                                res.Messages.Add(item.SAPCode + " (" + string.Join(", ", inValid) + ")");
                            } else
                            {
                                // tức là nếu target2 không có các code trong list thì coi như invalid
                                shiftCodeHalfDayInvalid = shiftCode.Where(x => x.Line == ShiftLine.HAFTDAY && x.IsActive).Select(p => p.Code).ToList();
                                inValid = dict.Where(x => !string.IsNullOrEmpty(x.Value) && !shiftCodeHalfDayInvalid.Contains(x.Value)).Select(p => p.Date.ToDateTime().ToString("dd/MM/yyyy")).ToList();
                                if (inValid.Any())
                                {
                                    res.ErrorCodes.Add(2);
                                    res.Messages.Add(item.SAPCode + " (" + string.Join(", ", inValid) + ")");
                                }
                            }
                        }
                    }
                }
                if (res.ErrorCodes.Count == 0)
                {
                    await ValidateTargetPlanByJsonFile(list, res, periodId);
                }
            }
            return res;
        }
        private async Task<ResultDTO> ValidateTargetPlanCustom(List<TargetPlanArgDetail> list)
        {
            var res = new ResultDTO();
            string invalidCode = ConfigurationManager.AppSettings["validateTargetPlanTarget1"];

            List<string> target2CodeList = invalidCode.Split(',').Select(x => x.Trim()).ToList();
            List<string> target2Args = invalidCode.Split(',').Select(x => x.Trim()).ToList();

            for (int i = 0; i < target2Args.Count; i++)
            {
                target2Args[i] = $"\"value\":\"{target2Args[i]}\"";
            }

            foreach (var item in list)
            {
                if (!item.IsSubmitted)
                {
                    if (item.Type == TypeTargetPlan.Target1)
                    {
                        await ValidateHolidays(true, res, item);
                        if (res.ErrorCodes.Count == 0)
                        {
                            var obj = JsonConvert.DeserializeObject(item.JsonData);
                            //minimize json string to check contains string
                            item.JsonData = JsonConvert.SerializeObject(obj, Formatting.None);
                            bool inValid = target2Args.Any(x => item.JsonData.Contains(x));

                            if (inValid)
                            {
                                res.ErrorCodes.Add(1);
                                res.Messages.Add(item.SAPCode);
                            }
                        }
                    }
                    else if (item.Type == TypeTargetPlan.Target2)
                    {
                        var dict = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                        // tức là nếu target2 không có các code trong list thì coi như invalid
                        var inValid = dict.Any(x => !target2CodeList.Contains(x.Value) && !string.IsNullOrEmpty(x.Value));
                        if (inValid)
                        {
                            res.ErrorCodes.Add(2);
                            res.Messages.Add(item.SAPCode);
                        }
                    }
                }

            }
            return res;
        }
        private async Task<List<DateValueArgs>> GetHolidaysInYear(List<DateValueArgs> target, Guid periodId)
        {
            var period = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync(periodId);
            var holidays = await _uow.GetRepository<HolidaySchedule>().FindByAsync<HolidayScheduleViewModel>(x => x.FromDate.Year >= period.FromDate.Year);
            if (holidays.Any())
            {
                target = target.Where(i => !String.IsNullOrEmpty(i.Value) && holidays.Any(x => x.FromDate <= i.Date.ToDateTime() && x.ToDate >= i.Date.ToDateTime())).ToList();
                return target;
            }
            return new List<DateValueArgs>();

        }
        private async Task<ResultDTO> ValidateHolidays(bool isFullDay, ResultDTO result, TargetPlanArgDetail target)
        {
            try
            {
                var validateHolidays = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => (isFullDay ? x.Line == ShiftLine.FULLDAY : x.Line == ShiftLine.HAFTDAY) && !x.IsDeleted && x.IsHoliday && x.IsActive);
                var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
                var lstDateValues = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                lstDateValues = await GetHolidaysInYear(lstDateValues, target.PeriodId.Value);
                var valid = new List<DateValueArgs>();
                var holidayCodes = validateHolidays.Select(x => x.Code).ToList();
                foreach (var config in holidayCodes)
                {
                    if (config.Contains("*"))
                    {
                        string value = config.Split('*').FirstOrDefault();
                        var mappings = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY).Select(y => y.Code).ToList();
                        valid.AddRange(lstDateValues.Where(i => i.Value.StartsWith(value) && mappings.Any(x => x.Contains(i.Value))).ToList());
                    }
                    else
                    {
                        valid.AddRange(lstDateValues.Where(i => i.Value == config).ToList());
                    }
                }
                var lstErrors = lstDateValues.Where(x => valid.Count == 0 || !valid.Select(s => s.Value).Contains(x.Value)).ToList();
                foreach (var err in lstErrors)
                {
                    result.ErrorCodes.Add(3);
                    var messageHolidayCodes = validateHolidays.Where(x => !x.Code.StartsWith("V") && (isFullDay ? x.Line == ShiftLine.FULLDAY : x.Line == ShiftLine.HAFTDAY)).Select(p => p.Code).ToList();
                    if (isFullDay)
                        messageHolidayCodes.Add("V**");

                    result.Object = string.Join(", ", messageHolidayCodes);
                    result.Messages.Add($"{target.SAPCode} ({err.Date.ToDateTime().ToString("dd/MM/yyyy")})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ValidateHolidays: " + ex.Message);
            }
            return result;
        }

        public async Task<ResultDTO> SavePendingTargetPlan(bool isSend, TargetPlanArg arg)
        {
            var result = new ResultDTO { };
            var errorSAPCodes = new List<string>();

            try
            {
                var noPRD = await ValidateNoPRD(arg.List, result, arg.PeriodFromDate.Date, arg.PeriodToDate.Date);
                if (noPRD.ErrorCodes.Count > 0)
                {
                    return noPRD;
                }
                
                var halfDayInvalid = ValidateHalfDay(arg.List, result);
                if (halfDayInvalid.ErrorCodes.Count > 0)
                {
                    return halfDayInvalid;
                }

                //jira - 593
                var sapCodeInvalid_StartDate = await ValidateStartDate(arg.List, result);
                if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
                {
                    return sapCodeInvalid_StartDate;
                }
                //================
                //jira - 253
                var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(arg.List, result);
                if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
                {
                    return sapCodeInvalid_ShiftCodeHQ;
                }
                //================
                ResultDTO invalid = await ValidateTargetPlan(arg.List, arg.PeriodId);
                if (invalid.ErrorCodes.Count > 0)
                {
                    result.ErrorCodes.AddRange(invalid.ErrorCodes);
                    result.Messages.AddRange(invalid.Messages);
                    result.Object = invalid.Object;
                    //invalid target plan -> return;
                    return result;
                }
                //add/edit TargetPlan
                var existTarget = await _uow.GetRepository<PendingTargetPlan>().FindByIdAsync(arg.Id);
                var listTemp = Mapper.Map<List<PendingTargetPlanDetail>>(arg.List);
                var existSAPCode = listTemp.Select(x => x.SAPCode);
                if (existTarget != null)
                {
                    // check another person modified (not this user) -> no allow -> return error message 
                    existTarget.DeptId = arg.DeptId;
                    existTarget.DeptCode = arg.DeptCode;
                    existTarget.DeptName = arg.DeptName;
                    existTarget.DivisionId = arg.DivisionId;
                    existTarget.DivisionCode = arg.DivisionCode;
                    existTarget.DivisionName = arg.DivisionName;
                    existTarget.PeriodId = arg.PeriodId;
                    existTarget.PeriodName = arg.PeriodName;
                    existTarget.PeriodFromDate = arg.PeriodFromDate;
                    existTarget.PeriodToDate = arg.PeriodToDate;

                    //delete TargetPlanDetail (chỉ những sap code có nằm trong arg.List) những thằng khác ko đưa vào thì để nguyên.
                    // vì data của user nào sẽ edit của user đó
                    //var existItems = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.PendingTargetPlanId == existTarget.Id && existSAPCode.Contains(x.SAPCode));
                    var existItems = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => existSAPCode.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                    _uow.GetRepository<PendingTargetPlanDetail>().Delete(existItems);
                    var listAdded = new List<PendingTargetPlanDetail>();
                    foreach (var item in listTemp)
                    {
                        var exists = existItems.FirstOrDefault(x => x.SAPCode == item.SAPCode && x.Type == item.Type && x.PendingTargetPlanId == existTarget.Id);
                        item.PendingTargetPlanId = existTarget.Id;
                        item.PeriodId = existTarget.PeriodId;
                        if (exists != null && (item.TargetPlanDetailId.HasValue || item.TargetPlanDetailId != Guid.Empty))
                        {
                            item.TargetPlanDetailId = exists.TargetPlanDetailId;
                        }
                        listAdded.Add(item); //add item mới

                        //cập nhật về tagret plan AEON-3666
                        var invalidStatus = new[] { "Rejected", "Cancelled", "Completed" };
                        var matchedDetails = await _uow.GetRepository<TargetPlanDetail>()
                            .FindByAsync(y => y.SAPCode == item.SAPCode && y.Type == item.Type && y.TargetPlan !=null && y.TargetPlan.PeriodId == arg.PeriodId && !invalidStatus.Contains(y.TargetPlan.Status));

                        if (matchedDetails?.Any() == true)
                        {
                            var detailRepo = _uow.GetRepository<TargetPlanDetail>(true);
                            foreach (var detail in matchedDetails)
                            {
                                detail.ERDQuality = item.ERDQuality;
                                detail.PRDQuality = item.PRDQuality;
                                detail.ALHQuality = item.ALHQuality;
                                detail.DOFLQuality = item.DOFLQuality;
                                detail.JsonData = item.JsonData;

                                detailRepo.Update(detail);
                            }
                        }

                    }

                    _uow.GetRepository<PendingTargetPlanDetail>().Add(listAdded);
                    await _uow.CommitAsync();
                    existTarget.PendingTargetPlanDetails = listAdded;
                    result.Object = Mapper.Map<TargetPlanArg>(existTarget);
                }
                else
                {
                    var addTargetPlans = new List<TargetPlanArgDetail>();
                    var pendingTargetPlanDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => existSAPCode.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                    if (pendingTargetPlanDetails.Any())
                    {
                        var invalidStatus = new[] { "Rejected", "Cancelled", "Completed" };
                        var updatedUsers = pendingTargetPlanDetails.Select(x => x.SAPCode);
                        foreach (var sapCode in updatedUsers)
                        {
                            var target1 = pendingTargetPlanDetails.FirstOrDefault(x => x.SAPCode == sapCode && x.Type == TypeTargetPlan.Target1);
                            var updateTarget1Item = arg.List.FirstOrDefault(x => x.SAPCode == target1.SAPCode && x.Type == TypeTargetPlan.Target1);
                            var matchedDetails = await _uow.GetRepository<TargetPlanDetail>() .FindByAsync(y => y.SAPCode == sapCode && y.TargetPlan !=null && y.TargetPlan.PeriodId == arg.PeriodId && !invalidStatus.Contains(y.TargetPlan.Status));
                            if (updateTarget1Item != null)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(updateTarget1Item.JsonData);
                                target1.ALHQuality = targets.Where(x => x.value.Contains("AL")).Count();
                                target1.ERDQuality = targets.Where(x => x.value.Contains("ERD")).Count();
                                target1.PRDQuality = targets.Where(x => x.value.Contains("PRD")).Count();
                                target1.DOFLQuality = targets.Where(x => x.value.Contains("DOFL")).Count();
                                target1.JsonData = updateTarget1Item.JsonData;


                                //cập nhật về tagret plan AEON-366

                                var targetPlan1 = matchedDetails.Where(y => y.Type == TypeTargetPlan.Target1);
                                if (targetPlan1?.Any() == true)
                                {
                                    var detailRepo = _uow.GetRepository<TargetPlanDetail>(true);
                                    foreach (var detail in targetPlan1)
                                    {
                                        detail.ERDQuality = target1.ERDQuality;
                                        detail.PRDQuality = target1.PRDQuality;
                                        detail.ALHQuality = target1.ALHQuality;
                                        detail.DOFLQuality = target1.DOFLQuality;
                                        detail.JsonData = target1.JsonData;

                                        detailRepo.Update(detail);
                                    }
                                }
                            }

                            var target2 = pendingTargetPlanDetails.FirstOrDefault(x => x.SAPCode == sapCode && x.Type == TypeTargetPlan.Target2);
                            var updateTarget2Item = arg.List.FirstOrDefault(x => x.SAPCode == target2.SAPCode && x.Type == TypeTargetPlan.Target2);
                            if (updateTarget2Item != null)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(updateTarget2Item.JsonData);
                                target2.ALHQuality = (float)targets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2;
                                target2.ERDQuality = (float)targets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2;
                                target2.PRDQuality = (float)targets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2;
                                target2.DOFLQuality = (float)targets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2;
                                target2.JsonData = updateTarget2Item.JsonData;


                                //cập nhật về tagret plan AEON-366
                                var targetPlan2 = matchedDetails.Where(y => y.Type == TypeTargetPlan.Target2);
                                if (targetPlan2?.Any() == true)
                                {
                                    var detailRepo = _uow.GetRepository<TargetPlanDetail>(true);
                                    foreach (var detail in targetPlan2)
                                    {
                                        detail.ERDQuality = target2.ERDQuality;
                                        detail.PRDQuality = target2.PRDQuality;
                                        detail.ALHQuality = target2.ALHQuality;
                                        detail.DOFLQuality = target2.DOFLQuality;
                                        detail.JsonData = target2.JsonData;

                                        detailRepo.Update(detail);
                                    }
                                }
                            }

                           


                        }

                        //Remove all edit
                        var editedSapCodes = pendingTargetPlanDetails.Select(x => x.SAPCode).ToList().Distinct();
                        addTargetPlans = arg.List.Where(x => !editedSapCodes.Contains(x.SAPCode)).ToList();
                    }
                    else
                    {
                        addTargetPlans = arg.List;
                    }
                    //add new 
                    var targetPlan = Mapper.Map<PendingTargetPlan>(arg);
                    // dowork for TargetPlanDetail                   
                    _uow.GetRepository<PendingTargetPlan>().Add(targetPlan);
                    var detailsList = Mapper.Map<List<PendingTargetPlanDetail>>(addTargetPlans);
                    foreach (var item in detailsList)
                    {
                        item.PendingTargetPlanId = targetPlan.Id;
                        item.PeriodId = targetPlan.PeriodId;
                    }
                    _uow.GetRepository<PendingTargetPlanDetail>().Add(detailsList);
                    await _uow.CommitAsync();
                    detailsList.AddRange(pendingTargetPlanDetails);
                    targetPlan.PendingTargetPlanDetails = detailsList;
                    result.Object = Mapper.Map<TargetPlanArg>(targetPlan);
                }
                if (isSend)
                {
                    //var currentPendingTargetPlan = await GetPendingTargetPlan(new TargetPlanQueryArg { DepartmentId = arg.DeptId.Value, DivisionId = arg.DivisionId.Value, PeriodId = arg.PeriodId });
                    var validSAPCodes = arg.List.Select(x => x.SAPCode);
                    var allPendingTargetPlan = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => validSAPCodes.Contains(x.SAPCode) && arg.PeriodId == x.PeriodId);
                    if (allPendingTargetPlan.Any())
                    {
                        foreach (var rd in allPendingTargetPlan)
                        {
                            if (errorSAPCodes.Count > 0)
                            {
                                break;
                            }
                            else
                            {
                                if (rd.Type == TypeTargetPlan.Target1)
                                {
                                    List<TargetPlanFromImportDetailItemDTO> target1 = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(rd.JsonData);
                                    var hasEmptyTarget1 = await CheckEmptyTarget(target1, rd.SAPCode, arg.PeriodFromDate, arg.PeriodToDate);
                                    if (hasEmptyTarget1)
                                    {
                                        var hasEmptyTarget1_Cancelled = await CheckEmptyTarget_Cancelled(target1, rd.SAPCode, arg.PeriodFromDate, arg.PeriodToDate);
                                        if (!hasEmptyTarget1_Cancelled)
                                        {
                                            rd.IsSent = true;
                                            continue;
                                        }
                                        errorSAPCodes.Add(rd.SAPCode);
                                    }
                                    else
                                    {
                                        rd.IsSent = true;
                                    }
                                }
                                else
                                {
                                    //
                                    rd.IsSent = true;
                                }

                            }
                        }
                    }
                    if (errorSAPCodes.Count == 0)
                    {
                        _uow.GetRepository<PendingTargetPlanDetail>().Update(allPendingTargetPlan);
                        await _uow.CommitAsync();
                    }
                    else
                    {
                        result.ErrorCodes.Add(4);
                        result.Messages = result.Messages.Concat(errorSAPCodes).ToList();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SaveTargetPlan: " + ex.Message + ex.StackTrace);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> SavePendingBeforeSubmit(TargetPlanArg arg)
        {
            var result = new ResultDTO { };
            var errorSAPCodes = new List<string>();

            try
            {
     
                //add/edit TargetPlan
                var existTarget = await _uow.GetRepository<PendingTargetPlan>().FindByIdAsync(arg.Id);
                var listTemp = Mapper.Map<List<PendingTargetPlanDetail>>(arg.List);
                var existSAPCode = listTemp.Select(x => x.SAPCode);
                if (existTarget != null)
                {
                    // check another person modified (not this user) -> no allow -> return error message 
                    existTarget.DeptId = arg.DeptId;
                    existTarget.DeptCode = arg.DeptCode;
                    existTarget.DeptName = arg.DeptName;
                    existTarget.DivisionId = arg.DivisionId;
                    existTarget.DivisionCode = arg.DivisionCode;
                    existTarget.DivisionName = arg.DivisionName;
                    existTarget.PeriodId = arg.PeriodId;
                    existTarget.PeriodName = arg.PeriodName;
                    existTarget.PeriodFromDate = arg.PeriodFromDate;
                    existTarget.PeriodToDate = arg.PeriodToDate;

                    //delete TargetPlanDetail (chỉ những sap code có nằm trong arg.List) những thằng khác ko đưa vào thì để nguyên.
                    // vì data của user nào sẽ edit của user đó
                    //var existItems = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.PendingTargetPlanId == existTarget.Id && existSAPCode.Contains(x.SAPCode));
                    var existItems = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => existSAPCode.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                    _uow.GetRepository<PendingTargetPlanDetail>().Delete(existItems);
                    var listAdded = new List<PendingTargetPlanDetail>();
                    foreach (var item in listTemp)
                    {
                        var submittedDetail = existItems.FirstOrDefault(x => x.SAPCode == item.SAPCode && x.Type == item.Type && x.PeriodId == arg.PeriodId);
                        if(submittedDetail != null)
                        {
                            item.IsSent = submittedDetail.IsSent;
                            item.IsSubmitted = submittedDetail.IsSubmitted;
                        }
                        else
                        {
                            item.IsSent = false;
                            item.IsSubmitted = false;
                        }

                        var exists = existItems.FirstOrDefault(x => x.SAPCode == item.SAPCode && x.Type == item.Type && x.PendingTargetPlanId == existTarget.Id);
                        item.PendingTargetPlanId = existTarget.Id;
                        item.PeriodId = existTarget.PeriodId;
                        if (exists != null && (item.TargetPlanDetailId.HasValue || item.TargetPlanDetailId != Guid.Empty))
                        {
                            item.TargetPlanDetailId = exists.TargetPlanDetailId;
                        }
                        listAdded.Add(item); //add item mới              
                    }

                    _uow.GetRepository<PendingTargetPlanDetail>().Add(listAdded);
                    await _uow.CommitAsync();
                    existTarget.PendingTargetPlanDetails = listAdded;
                    result.Object = Mapper.Map<TargetPlanArg>(existTarget);
                }
                else
                {
                    var addTargetPlans = new List<TargetPlanArgDetail>();
                    var pendingTargetPlanDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => existSAPCode.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                    if (pendingTargetPlanDetails.Any())
                    {
                        var updatedUsers = pendingTargetPlanDetails.Select(x => x.SAPCode);
                        foreach (var sapCode in updatedUsers)
                        {
                            var target1 = pendingTargetPlanDetails.FirstOrDefault(x => x.SAPCode == sapCode && x.Type == TypeTargetPlan.Target1);
                            var updateTarget1Item = arg.List.FirstOrDefault(x => x.SAPCode == target1.SAPCode && x.Type == TypeTargetPlan.Target1);
                            if (updateTarget1Item != null)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(updateTarget1Item.JsonData);
                                target1.ALHQuality = targets.Where(x => x.value.Contains("AL")).Count();
                                target1.ERDQuality = targets.Where(x => x.value.Contains("ERD")).Count();
                                target1.PRDQuality = targets.Where(x => x.value.Contains("PRD")).Count();
                                target1.DOFLQuality = targets.Where(x => x.value.Contains("DOFL")).Count();
                                target1.JsonData = updateTarget1Item.JsonData;
                            }

                            var target2 = pendingTargetPlanDetails.FirstOrDefault(x => x.SAPCode == sapCode && x.Type == TypeTargetPlan.Target2);
                            var updateTarget2Item = arg.List.FirstOrDefault(x => x.SAPCode == target2.SAPCode && x.Type == TypeTargetPlan.Target2);
                            if (updateTarget2Item != null)
                            {
                                var targets = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(updateTarget2Item.JsonData);
                                target2.ALHQuality = (float)targets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2;
                                target2.ERDQuality = (float)targets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2;
                                target2.PRDQuality = (float)targets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2;
                                target2.DOFLQuality = (float)targets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2;
                                target2.JsonData = updateTarget2Item.JsonData;

                            }
                        }

                        //Remove all edit
                        var editedSapCodes = pendingTargetPlanDetails.Select(x => x.SAPCode).ToList().Distinct();
                        addTargetPlans = arg.List.Where(x => !editedSapCodes.Contains(x.SAPCode)).ToList();
                    }
                    else
                    {
                        addTargetPlans = arg.List;
                    }
                    //add new 
                    var targetPlan = Mapper.Map<PendingTargetPlan>(arg);
                    // dowork for TargetPlanDetail                   
                    _uow.GetRepository<PendingTargetPlan>().Add(targetPlan);
                    var detailsList = Mapper.Map<List<PendingTargetPlanDetail>>(addTargetPlans);
                    foreach (var item in detailsList)
                    {
                        item.PendingTargetPlanId = targetPlan.Id;
                        item.PeriodId = targetPlan.PeriodId;
                        var exist = await _uow.GetRepository<PendingTargetPlanDetail>().GetSingleAsync(x => x.SAPCode == item.SAPCode && x.Type == item.Type && x.PeriodId == item.PeriodId);
                        if (exist != null)
                        {
                            item.IsSent = exist.IsSent;
                            item.IsSubmitted = exist.IsSubmitted;
                        }
                        else
                        {
                            item.IsSent = false;
                            item.IsSubmitted = false;
                        }
                    }
                    _uow.GetRepository<PendingTargetPlanDetail>().Add(detailsList);
                    await _uow.CommitAsync();
                    detailsList.AddRange(pendingTargetPlanDetails);
                    targetPlan.PendingTargetPlanDetails = detailsList;
                    result.Object = Mapper.Map<TargetPlanArg>(targetPlan);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SaveTargetPlan: " + ex.Message + ex.StackTrace);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> SendRequest_TargetPlan(bool isSend, TargetPlanArg arg)
        {
            var result = new ResultDTO { };
            var errorSAPCodes = new List<string>();

            try
            {
                var noPRD = await ValidateNoPRD(arg.List, result, arg.PeriodFromDate.Date, arg.PeriodToDate.Date);
                if (noPRD.ErrorCodes.Count > 0)
                {
                    return noPRD;
                }

                var halfDayInvalid = ValidateHalfDay(arg.List, result);
                if (halfDayInvalid.ErrorCodes.Count > 0)
                {
                    return halfDayInvalid;
                }

                //jira - 593
                var sapCodeInvalid_StartDate = await ValidateStartDate(arg.List, result);
                if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
                {
                    return sapCodeInvalid_StartDate;
                }
                //================
                //jira - 253
                var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(arg.List, result);
                if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
                {
                    return sapCodeInvalid_ShiftCodeHQ;
                }
                //================

                ResultDTO invalid = await ValidateTargetPlan(arg.List, arg.PeriodId);
                if (invalid.ErrorCodes.Count > 0)
                {
                    result.ErrorCodes.AddRange(invalid.ErrorCodes);
                    result.Messages.AddRange(invalid.Messages);
                    result.Object = null;
                    //invalid target plan -> return;
                    return result;
                }
                //add/edit TargetPlan
                var existTarget = await _uow.GetRepository<TargetPlan>().FindByIdAsync(arg.Id);
                var listTemp = Mapper.Map<List<TargetPlanDetail>>(arg.List);
                var existSAPCode = listTemp.Select(x => x.SAPCode);

                if (isSend)
                {
                    //var currentPendingTargetPlan = await GetPendingTargetPlan(new TargetPlanQueryArg { DepartmentId = arg.DeptId.Value, DivisionId = arg.DivisionId.Value, PeriodId = arg.PeriodId });
                    var validSAPCodes = arg.List.Select(x => x.SAPCode);
                    var allPendingTargetPlan = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => validSAPCodes.Contains(x.SAPCode) && arg.PeriodId == x.PeriodId);
                    if (allPendingTargetPlan.Any())
                    {
                        foreach (var rd in allPendingTargetPlan)
                        {
                            if (errorSAPCodes.Count > 0)
                            {
                                break;
                            }
                            else
                            {
                                if (rd.Type == TypeTargetPlan.Target1)
                                {
                                    List<TargetPlanFromImportDetailItemDTO> target1 = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(rd.JsonData);
                                    var hasEmptyTarget1 = await CheckEmptyTarget(target1, rd.SAPCode, arg.PeriodFromDate, arg.PeriodToDate);
                                    if (hasEmptyTarget1)
                                    {
                                        errorSAPCodes.Add(rd.SAPCode);
                                    }
                                    else
                                    {
                                        rd.IsSent = true;
                                        rd.IsSubmitted = true;
                                    }
                                }
                                else
                                {
                                    //
                                    rd.IsSent = true;
                                    rd.IsSubmitted = true;
                                }
                                _uow.GetRepository<PendingTargetPlanDetail>().Update(rd);
                                _uow.Commit();
                            }
                        }
                    }
                    if (errorSAPCodes.Count == 0)
                    {
                        //_uow.GetRepository<TargetPlanDetail>().Update(allPendingTargetPlan);
                        //await _uow.CommitAsync();
                    }
                    else
                    {
                        result.ErrorCodes.Add(4);
                        result.Messages = result.Messages.Concat(errorSAPCodes).ToList();
                    }
                }
                if (existTarget != null)
                {
                    #region Update Target Plan
                    // check another person modified (not this user) -> no allow -> return error message 
                    existTarget.DeptId = arg.DeptId;
                    existTarget.DeptCode = arg.DeptCode;
                    existTarget.DeptName = arg.DeptName;
                    existTarget.DivisionId = arg.DivisionId;
                    existTarget.DivisionCode = arg.DivisionCode;
                    existTarget.DivisionName = arg.DivisionName;
                    existTarget.PeriodId = arg.PeriodId;
                    existTarget.PeriodName = arg.PeriodName;
                    existTarget.PeriodFromDate = arg.PeriodFromDate;
                    existTarget.PeriodToDate = arg.PeriodToDate;
                    existTarget.IsSent = isSend;
                    //delete TargetPlanDetail (chỉ những sap code có nằm trong arg.List) những thằng khác ko đưa vào thì để nguyên.
                    // vì data của user nào sẽ edit của user đó
                    var targetPlanDetailItems = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId.Equals(arg.Id));
                    List<TargetPlanDetail> existItems = targetPlanDetailItems.ToList();
                    List<Guid> oldTargetPlanDetailID = new List<Guid>();

                    var listAdded = new List<TargetPlanDetail>();
                    var itemCount = existItems.Count();
                    for (int i = 0; i < itemCount; i++)
                    {
                        var existItem = existItems[i];
                        var item = listTemp.FirstOrDefault(x => x.SAPCode == existItem.SAPCode && x.Type == existItem.Type);
                        if (item != null)
                        {

                            //Tìm phiếu chi tiết pending: điều kiện sapCode và isSent hoặc IsSubmit là false của periodId để cập nhật  //cập nhật về pending plan AEON-366
                            var matchedDetails = await _uow.GetRepository<PendingTargetPlanDetail>()
                            .FindByAsync(y => y.SAPCode == existItem.SAPCode && y.Type == existItem.Type &&
                                         (!y.IsSent || y.IsSubmitted) && y.PendingTargetPlan!=null && y.PendingTargetPlan.PeriodId == arg.PeriodId);

                            if (matchedDetails?.Any() == true)
                            {
                                var detailRepo = _uow.GetRepository<PendingTargetPlanDetail>(true);
                                foreach (var detail in matchedDetails)
                                {
                                    detail.ERDQuality = item.ERDQuality;
                                    detail.PRDQuality = item.PRDQuality;
                                    detail.ALHQuality = item.ALHQuality;
                                    detail.DOFLQuality = item.DOFLQuality;
                                    detail.JsonData = item.JsonData;

                                    detailRepo.Update(detail);
                                }
                                await _uow.CommitAsync(); 
                            }

                            existItem.ERDQuality = item.ERDQuality;
                            existItem.PRDQuality = item.PRDQuality;
                            existItem.ALHQuality = item.ALHQuality;
                            existItem.DOFLQuality = item.DOFLQuality;
                            existItem.JsonData = item.JsonData;
                            _uow.GetRepository<TargetPlanDetail>(true).Update(existItem);
                        }
                    }

                    _uow.Commit();
                    //_uow.GetRepository<TargetPlanDetail>().Delete(_uow.GetRepository<TargetPlanDetail>().FindBy(x => oldTargetPlanDetailID.Contains(x.Id)));
                    //_uow.Commit();
                    existTarget.TargetPlanDetails = listAdded;
                    result.Object = Mapper.Map<TargetPlanArg>(existTarget);
                    #endregion
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SaveTargetPlan: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        //code dưới đang dư -> comment lại tính sau
        //public async Task<ResultDTO> GetTargetPlanFromDepartment(Guid departmentId)
        //{
        //    var result = new ResultDTO { };
        //    var data = new ArrayResultDTO();
        //    try
        //    {
        //        var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(departmentId);
        //        IEnumerable<TargetPlanViewModel> targetPlans = Enumerable.Empty<TargetPlanViewModel>();
        //        if (currentDepartment.Type == Infrastructure.Enums.DepartmentType.Division)
        //        {
        //            targetPlans = await _uow.GetRepository<PendingTargetPlan>().FindByAsync<TargetPlanViewModel>(x => x.DivisionId.Value == departmentId);
        //        }
        //        else
        //        {
        //            targetPlans = await _uow.GetRepository<PendingTargetPlan>().FindByAsync<TargetPlanViewModel>(x => x.DeptId.Value == departmentId);
        //        }
        //        data.Data = targetPlans;
        //        result.Object = data;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error at GetTargetPlanFromDepartment: {0} " + ex.Message);
        //        result.Messages = new List<string> { ex.Message };
        //        return result;
        //    }
        //}
        public async Task<ResultDTO> GetTargetPlanPeriods()
        {
            var result = new ResultDTO { };
            try
            {
                var periods = await _uow.GetRepository<TargetPlanPeriod>().GetAllAsync<TargetPlanPeriodViewModel>("FromDate desc");
                result.Object = new ArrayResultDTO { Data = periods, Count = periods.Count() };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanPeriods: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        private DateValueArgs GetLeaveOrShiftCode(DateValueArgs dict, LeaveApplicationDetail leave, ShiftExchangeApplicationDetail shift, IEnumerable<LeaveApplication> listLeaves, IEnumerable<ShiftExchangeApplication> listShifts, TypeTargetPlan typeTarget, out int actualValue)
        {
            /*var jsonContent = JsonHelper.GetJsonContentFromFile("Mappings", "target-plan-valid.json");*/
            var shiftCode = _uow.GetRepository<ShiftCode>().FindBy<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
            /*var mappingsTarget1 = JsonConvert.DeserializeObject<List<TargetPlanValidMapping>>(jsonContent).Where(x => x.Type == TypeTargetPlan.Target1);
            var mappingsTarget2 = JsonConvert.DeserializeObject<List<TargetPlanValidMapping>>(jsonContent).Where(x => x.Type == TypeTargetPlan.Target2);*/
            var mappingsTarget1 = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY).ToList();
            var mappingsTarget2 = shiftCode.Where(x => x.Line == ShiftLine.HAFTDAY).ToList();
            DateValueArgs res = new DateValueArgs
            {
                Date = dict.Date,
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
                {
                    if (typeTarget == TypeTargetPlan.Target1)
                    {
                        var targetValid = mappingsTarget1.Where(x => x.Code == leave.LeaveCode);
                        if (targetValid.Any())
                            assignFrom = 1;
                        else
                            assignFrom = 2;
                    } else
                    {
                        var targetValid = mappingsTarget2.Where(x => x.Code == leave.LeaveCode);
                        if (targetValid.Any())
                            assignFrom = 1;
                        else
                            assignFrom = 2;
                    }
                } else
                {
                    assignFrom = 2;
                }
            }

            /*List<string> halfCodes = ConfigurationManager.AppSettings["validateTargetPlanTarget1"].Split(',').Select(x => x.Trim()).ToList();*/
            if (assignFrom == 1)
            {   //check is actual2 or 1
                if (mappingsTarget2.Any(x => x.Code == leave.LeaveCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.Date,
                    Value = leave.LeaveCode,
                    Status = leave.LeaveApplication.Status,
                    Type = "Leave"
                };
            }
            else if (assignFrom == 2)
            {
                if (mappingsTarget2.Any(x => x.Code == shift.NewShiftCode))
                    actualValue = 1;
                res = new DateValueArgs()
                {
                    Date = dict.Date,
                    Value = shift.NewShiftCode,
                    Status = shift.ShiftExchangeApplication.Status,
                    Type = "Shift"
                };
            }
            return res;
        }

        public async Task<ResultDTO> GetActualShiftPlan(ActualTargetPlanArg args)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();
            try
            {
                List<object> listResult = new List<object>();
                var targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>(true).FindByIdAsync(args.PeriodId);
                if (targetPlanPeriod != null)
                {
                    var listSapCode = JsonConvert.DeserializeObject<List<string>>(args.ListSAPCode);
                    var userList = await _uow.GetRepository<User>().FindByAsync(x => listSapCode.Contains(x.SAPCode));
                    var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft", "Requested To Change" };
                    var targetplanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(x => listSapCode.Contains(x.SAPCode) && x.TargetPlan.PeriodId == args.PeriodId);
                    var listLeaveDetails = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => listSapCode.Contains(x.LeaveApplication.UserSAPCode) && !statusToCheckes.Contains(x.LeaveApplication.Status) &&
                                ((targetPlanPeriod.FromDate <= x.FromDate && x.FromDate <= targetPlanPeriod.ToDate)
                                ||
                                (targetPlanPeriod.FromDate <= x.ToDate && x.ToDate <= targetPlanPeriod.ToDate)), "", y => y.LeaveApplication);
                    var listShiftDetails = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x => listSapCode.Contains(x.User.SAPCode) &&
                                (targetPlanPeriod.FromDate <= x.ShiftExchangeDate && x.ShiftExchangeDate <= targetPlanPeriod.ToDate) && !statusToCheckes.Contains(x.ShiftExchangeApplication.Status));

                    var tempIds = listLeaveDetails.Select(x => x.LeaveApplicationId).ToList();

                    var listLeaves = await _uow.GetRepository<LeaveApplication>(true).FindByAsync(x => tempIds.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));
                    var listLeavesId = listLeaves.Select(x => x.Id);
                    var tempIdShifts = listShiftDetails.Select(x => x.ShiftExchangeApplicationId).ToList();
                    var listShifts = await _uow.GetRepository<ShiftExchangeApplication>(true).FindByAsync(x => tempIdShifts.Contains(x.Id) && (x.Status.Equals("Completed") || !statusToCheckes.Contains(x.Status)));

                    // Remove item with case HR
                    if (args.NotInShiftExchange != null && args.NotInShiftExchange.Any(shiftExchanggId => shiftExchanggId != Guid.Empty))
                    {
                        listShifts = listShifts.Where(x => !args.NotInShiftExchange.Contains(x.Id)).ToList();
                        listShiftDetails = listShiftDetails.Where(x => x.ShiftExchangeApplicationId.HasValue && !args.NotInShiftExchange.Contains(x.ShiftExchangeApplicationId.Value)).ToList();
                    }

                    var listShiftsId = listShifts.Select(x => x.Id);
                    var groupTargetPlans = targetplanDetails.GroupBy(x => x.SAPCode);

                    /*var jsonContent = JsonHelper.GetJsonContentFromFile("Mappings", "target-plan-valid.json");
                    var mappings = JsonConvert.DeserializeObject<List<TargetPlanValidMapping>>(jsonContent);*/
                    var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
                    foreach (var group in groupTargetPlans)
                    {
                        List<DateValueArgs> jsonActual1 = new List<DateValueArgs>();
                        List<DateValueArgs> jsonActual2 = new List<DateValueArgs>();
                        dynamic _data = new ExpandoObject();
                        foreach (var target in group)
                        {
                            if (target.Type == TypeTargetPlan.Target1)
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    /*var listShiftCodeFullDateValid = mappings.Where(x => x.Type == TypeTargetPlan.Target1).FirstOrDefault();*/
                                    var listShiftCodeFullDateValid = shiftCode.Where(x => x.Line == ShiftLine.FULLDAY).Select(y => y.Code).ToList();
                                    var lUser = userList.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.Where(x => (!x.NewShiftCode.EndsWith("1") && !x.NewShiftCode.EndsWith("2"))).OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    if (listShiftCodeFullDateValid != null && listShiftCodeFullDateValid.Any())
                                        lShiftDetail = listShiftDetails.Where(x => listShiftCodeFullDateValid.Contains(x.NewShiftCode)).OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (actualValue == 0)
                                        jsonActual1.Add(res);
                                }
                                _data.Actual1 = JsonConvert.SerializeObject(jsonActual1);
                            }
                            else
                            {
                                var dictList = JsonConvert.DeserializeObject<List<DateValueArgs>>(target.JsonData);
                                foreach (var dict in dictList)
                                {
                                    /*var listShiftCodeHalfDateValid = mappings.Where(x => x.Type == TypeTargetPlan.Target2).FirstOrDefault();*/
                                    var listShiftCodeHalfDateValid = shiftCode.Where(x => x.Line == ShiftLine.HAFTDAY).Select(y => y.Code).ToList();
                                    var lUser = userList.FirstOrDefault(x => x.SAPCode == target.SAPCode);
                                    var lLeaveDetail = listLeaveDetails.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == target.SAPCode && listLeavesId.Contains(x.LeaveApplicationId) && CompareDateRangeAndStringDate(x.FromDate.ToLocalTime(), x.ToDate.ToLocalTime(), dict.Date));
                                    var lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    if (listShiftCodeHalfDateValid != null && listShiftCodeHalfDateValid.Any())
                                    {
                                        lShiftDetail = listShiftDetails.OrderByDescending(x => x.Created).FirstOrDefault(x => listShiftCodeHalfDateValid.Contains(x.NewShiftCode) && x.User.SAPCode == target.SAPCode && x.ShiftExchangeDate.ToString("yyyyMMdd").Equals(dict.Date));
                                    }
                                    int actualValue = 0;
                                    var res = GetLeaveOrShiftCode(dict, lLeaveDetail, lShiftDetail, listLeaves, listShifts, target.Type, out actualValue);
                                    if (actualValue == 1)
                                        jsonActual2.Add(res);
                                }
                                _data.Actual2 = JsonConvert.SerializeObject(jsonActual2);
                            }
                        }
                        var expandoDict = _data as IDictionary<string, object>;
                        try
                        {
                            listResult.Add(new
                            {
                                SAPCode = group.Key,
                                Actual1 = GetValue(expandoDict, "Actual1"),
                                Actual2 = GetValue(expandoDict, "Actual2")
                            });

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"ErrorMessage: {ex.Message}");
                        }
                    }
                }
                data.Data = listResult;
                data.Count = listResult.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanFromDepartmentAndPeriod: {0} " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }
        private object GetValue(IDictionary<string, object> queryWhere, string key)
        {
            object returnValue;
            if (!queryWhere.TryGetValue(key, out returnValue))
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }

        public async Task<ResultDTO> UpdatePermissionUserInTargetPlanItem(Guid itemId)
        {
            var result = new ResultDTO();
            var pendingDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(i => i.TargetPlanId == itemId);
            if (pendingDetails.Any())
            {
                pendingDetails = pendingDetails.GroupBy(x => new { x.SAPCode, x.TargetPlanId }).Select(y => y.FirstOrDefault());
                IEnumerable<string> sapCodes = pendingDetails.Select(s => s.SAPCode);
                var users = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(i => sapCodes.Contains(i.SAPCode));

                if (users.Any())
                {
                    var permissions = new List<Permission>();
                    foreach (var detail in pendingDetails)
                    {
                        var user = users.FirstOrDefault(i => i.SAPCode == detail.SAPCode);
                        if (user != null)
                        {
                            var permissionExits = await _uow.GetRepository<Permission>().AnyAsync(i => i.ItemId == itemId && i.UserId == user.Id);
                            if (!permissionExits)
                            {
                                permissions.Add(new Permission
                                {
                                    Id = Guid.NewGuid(),
                                    Created = DateTime.Now,
                                    Modified = DateTime.Now,
                                    DepartmentId = null,
                                    DepartmentType = 0,
                                    Perm = Right.View,
                                    ItemId = itemId,
                                    UserId = user.Id
                                });

                            }
                        }

                    }
                    _uow.GetRepository<Permission>().Add(permissions);
                    await _uow.CommitAsync();
                }

            }
            return result;
        }
        private int CompareString(string input1, string input2)
        {
            return String.Compare(input1, input2, comparisonType: StringComparison.OrdinalIgnoreCase);
        }
        private bool CompareDateRangeAndStringDate(DateTimeOffset fromDate, DateTimeOffset toDate, string compareStringDate)
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

        public async Task<ResultDTO> Submit(SubmitDataArg args)
        {
            var result = new ResultDTO { };
            var resultData = new List<object>();
            var isContinue = false;
            //var data = new ArrayResultDTO();
            if(args.PeriodId !=null)
            {
                TargetPlanPeriod targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync(args.PeriodId);
                args.PeriodFromDate = targetPlanPeriod.FromDate;
                args.PeriodToDate = targetPlanPeriod.ToDate;
            }

            List<SubmitTargetPlanArg> listSubmit = new List<SubmitTargetPlanArg>();
            try
            {
                var errorSAPCodes = new List<string>();
                var ignoreSAPCodes = new List<string>();
                List<string> listSap = args.ListSAPCode.Split(',').ToList();
                var statusToCheckes = new string[] { "Rejected", "Cancelled", "Draft" };
                var requestToChangeTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => listSap.Contains(x.SAPCode) && x.TargetPlan.PeriodId == args.PeriodId && !statusToCheckes.Contains(x.TargetPlan.Status));
                if (requestToChangeTargetPlanDetails.Any())
                {
                    ignoreSAPCodes = requestToChangeTargetPlanDetails.Select(x => x.SAPCode).ToList();
                    if (ignoreSAPCodes.Any())
                    {
                        listSap = listSap.Where(x => !ignoreSAPCodes.Contains(x)).ToList();
                    }
                }

                // HR-834: kiem tra phieu da tao target plan hay chua?
                var allTargetPlanSAPCode = (await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => listSap.Contains(x.SAPCode) && args.PeriodId == x.TargetPlan.PeriodId && x.TargetPlan.Status == "Completed")).Select(x => x.SAPCode).ToList().Distinct();
                if (allTargetPlanSAPCode.Any())
                {
                    var allPendingTargetPlanCompleted = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => allTargetPlanSAPCode.Contains(x.SAPCode) && args.PeriodId == x.PeriodId);
                    if (allPendingTargetPlanCompleted.Any())
                    {
                        foreach(var pendingCompleted in allPendingTargetPlanCompleted)
                        {
                            pendingCompleted.IsSubmitted = true;
                        }
                        await _uow.CommitAsync();
                    }
                    listSap = listSap.Where(x => !allTargetPlanSAPCode.Contains(x)).ToList();
                }

                if (listSap.Any())
                {
                    var allPendingTargetPlan = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => listSap.Contains(x.SAPCode) && args.PeriodId == x.PeriodId);
                    var lakeTarget = listSap.Where(x => !allPendingTargetPlan.Select(s => s.SAPCode).Contains(x));
                    foreach (var lake in lakeTarget)
                    {
                        errorSAPCodes.Add(lake);
                    }
                    if (allPendingTargetPlan.Any() && errorSAPCodes.Count == 0)
                    {
                        foreach (var rd in allPendingTargetPlan)
                        {
                            if (errorSAPCodes.Count > 0)
                            {
                                break;
                            }
                            else
                            {
                                if (rd.Type == TypeTargetPlan.Target1)
                                {
                                    List<TargetPlanFromImportDetailItemDTO> target1 = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(rd.JsonData);
                                    var hasEmptyTarget1 = await CheckEmptyTarget(target1, rd.SAPCode, args.PeriodFromDate, args.PeriodToDate);
                                    if (hasEmptyTarget1)
                                    {
                                        errorSAPCodes.Add(rd.SAPCode);
                                    }
                                    else
                                    {
                                        rd.IsSent = true;
                                    }
                                }
                                else
                                {
                                    rd.IsSent = true;
                                }

                            }
                        }
                    }
                    if (errorSAPCodes.Count == 0)
                    {
                        var detailTargetPlans = (await _uow.GetRepository<PendingTargetPlanDetail>(true).FindByAsync(x => x.PeriodId == args.PeriodId && listSap.Contains(x.SAPCode)));
                        if (detailTargetPlans.Count() > 0)
                        {
                            var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.Id == _uow.UserContext.CurrentUserId);
                            if (currentUser != null && (!currentUser.IsStore.Value && currentUser.JobGradeValue >= 5 || currentUser.IsStore.Value && currentUser.JobGradeValue >= 4))
                            {
                                listSap = new List<string>() { currentUser.SAPCode };
                            }
                            var pTrgetDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => listSap.Contains(x.SAPCode) && !x.IsSubmitted && x.PeriodId == args.PeriodId);
                            if (pTrgetDetails.Count() > 0) //có targetplandetail thì mới tạo TargetPlan cho workflow chạy
                            {
                                var period = await _uow.GetRepository<Period>().FindByIdAsync(args.PeriodId);
                                //tạo targetPlan để chạy workflow
                                var add = new TargetPlan()
                                {
                                    DeptCode = args.DeptCode,
                                    DeptId = args.DeptId,
                                    DeptName = args.DeptName,
                                    DivisionCode = args.DivisionCode,
                                    DivisionId = args.DivisionId,
                                    DivisionName = args.DivisionName,
                                    PeriodFromDate = args.PeriodFromDate,
                                    PeriodToDate = args.PeriodToDate,
                                    PeriodId = args.PeriodId,
                                    PeriodName = args.PeriodName,
                                    UserSAPCode = args.UserSAPCode,
                                    UserFullName = args.UserFullName,
                                    //PendingTargetPlanId = args.PendingTargetPlanId,
                                    IsStore = await CheckIsStoreFromDepartment(args.DivisionId.HasValue ? args.DivisionId.Value : args.DeptId.Value)
                                };
                                _uow.GetRepository<TargetPlan>().Add(add);
                                await _uow.CommitAsync();
                                List<TargetPlanDetail> listDetail = new List<TargetPlanDetail>();
                                foreach (var item in pTrgetDetails)
                                {

                                    var detailAdd = Mapper.Map<TargetPlanDetail>(item);
                                    item.IsSent = true;
                                    item.IsSubmitted = true;
                                    detailAdd.TargetPlanId = add.Id;
                                    listDetail.Add(detailAdd);
                                }
                                _uow.GetRepository<TargetPlanDetail>().Add(listDetail);
                                await _uow.CommitAsync();
                                //result.Object = Mapper.Map<TargetPlanViewModel>(add);
                                resultData.Add(add);
                            }
                        }
                    }
                    else
                    {
                        result.ErrorCodes.Add(4);
                        result.Messages = result.Messages.Concat(errorSAPCodes).ToList();
                    }
                }
                if (ignoreSAPCodes.Any())
                {
                    // cập nhật target Plan
                    var targetPlanDetailIds = requestToChangeTargetPlanDetails.Select(x => x.Id);
                    var targetPlanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => targetPlanDetailIds.Contains(x.Id));
                    foreach (var rd in requestToChangeTargetPlanDetails)
                    {
                        var currentItem = targetPlanDetails.FirstOrDefault(x => x.Id == rd.Id);
                        var pendingCurrentItem = _uow.GetRepository<PendingTargetPlanDetail>().GetSingle(x => x.SAPCode == currentItem.SAPCode && x.Type == currentItem.Type && x.PeriodId == currentItem.TargetPlan.PeriodId);
                        if (pendingCurrentItem != null)
                        {
                            currentItem.JsonData = pendingCurrentItem.JsonData;
                            currentItem.ALHQuality = pendingCurrentItem.ALHQuality;
                            currentItem.ERDQuality = pendingCurrentItem.ERDQuality;
                            currentItem.PRDQuality = pendingCurrentItem.PRDQuality;
                            currentItem.DOFLQuality = pendingCurrentItem.DOFLQuality;
                        }
                        else
                        {
                            result.ErrorCodes.Add(4);
                            result.Messages.Add("Can't find requested to change target plan");
                        }
                    }
                    // cập nhật trạng thái đã submit
                    var pTrgetDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => ignoreSAPCodes.Contains(x.SAPCode) && x.PeriodId == args.PeriodId);
                    if (pTrgetDetails.Count() > 0)
                    {
                        foreach (var item in pTrgetDetails)
                        {
                            item.IsSubmitted = true;
                            item.IsSent = true;
                        }
                    }
                    await _uow.CommitAsync();
                    /*var requestedTargetPlanDetailId = targetPlanDetailIds.FirstOrDefault();
                    var requestedTargetPlan = await _uow.GetRepository<TargetPlanDetail>().FindByIdAsync<TargetPlanDetailViewModel>(requestedTargetPlanDetailId);
                    if (requestedTargetPlan != null)
                    {
                        var targetPlanId = requestedTargetPlan.TargetPlanId;
                        var currentWorkFlow = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == targetPlanId, "Modified desc");
                        if (currentWorkFlow != null)
                        {
                            result.Object = new { workflowId = currentWorkFlow.TemplateId, itemId = targetPlanId, referenceNumber = currentWorkFlow.ItemReferenceNumber };
                        }
                    }*/
                    var requestedTargetPlanId = requestToChangeTargetPlanDetails.Select(x => x.TargetPlanId).Distinct().ToList();
                    if (requestedTargetPlanId.Any())
                    {
                        foreach(var targetPlanId in requestedTargetPlanId)
                        {
                            var currentWorkFlow = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == targetPlanId, "Modified desc");
                            if (currentWorkFlow != null)
                            {
                                //result.Object = new { workflowId = currentWorkFlow.TemplateId, itemId = targetPlanId, referenceNumber = currentWorkFlow.ItemReferenceNumber };
                                resultData.Add(new { workflowId = currentWorkFlow.TemplateId, itemId = targetPlanId, referenceNumber = currentWorkFlow.ItemReferenceNumber });
                            }
                        }
                    }
                }
                else
                {
                    foreach (var error in errorSAPCodes)
                    {
                        result.ErrorCodes.Add(4);
                        result.Messages.Add(error);
                    }
                }
                
                if (result.ErrorCodes.Count() == 0 && allTargetPlanSAPCode.Count() > 0 && result.Object == null)
                {
                    foreach (var error in allTargetPlanSAPCode)
                    {
                        result.ErrorCodes.Add(12);
                        result.Messages.Add(error);
                    }
                }
                result.Object = resultData;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanFromDepartmentAndPeriod: " + ex.Message);
                result.Messages = new List<string> { ex.Message
    };
                return result;
            }
        }


        private async Task<bool> CheckIsStoreFromDepartment(Guid id)
        {
            var currentDepartment = await _uow.GetRepository<Department>().FindByIdAsync(id);
            if (currentDepartment != null)
            {
                return currentDepartment.IsStore;
            }
            return false;
        }

        public async Task<ResultDTO> GetTargetPlanDetailIsCompleted(TargetPlanDetailQueryArg args)
        {
            var result = new ResultDTO { };
            var data = new ArrayResultDTO();
            try
            {
                //IEnumerable<TargetPlanArg> targetPlans = Enumerable.Empty<TargetPlanArg>();
                //targetPlans = await _uow.GetRepository<TargetPlan>(true).FindByAsync<TargetPlanArg>
                //    (x =>
                //    (x.DeptId.Value == args.DepartmentId)
                //    && x.PeriodId == args.PeriodId
                //    && (x.Status == "Completed" || x.Status == "Approved"));
                //if (targetPlans.Any())
                //{

                //}
                //var targetPlanIds = targetPlans.Select(x => x.Id).ToList();
                //List<TargetPlanArgDetail> targetPlanDetails = (await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanArgDetail>
                //       (x => targetPlanIds.Contains(x.TargetPlanId) && args.SAPCodes.Contains(x.SAPCode))).ToList();
                List<TargetPlanArgDetail> targetPlanDetails = (await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanArgDetail>
                       (x => args.SAPCodes.Contains(x.SAPCode) && x.TargetPlan.PeriodId == args.PeriodId && (x.TargetPlan.Status == "Completed" || x.TargetPlan.Status == "Approved"))).ToList();
                if (targetPlanDetails.Count > 2)
                {
                    var groups = targetPlanDetails.GroupBy(x => new { x.ReferenceNumber, x.SAPCode }).OrderByDescending(y => y.Key.ReferenceNumber);
                    var listData = new List<TargetPlanArgDetail>();
                    foreach (var group in groups)
                    {
                        if (!listData.Any(x => x.SAPCode == group.Key.SAPCode && !group.All(y => x.Type == y.Type)))
                        {
                            listData.AddRange(group.ToList());
                        }
                    }
                    data.Data = listData;
                }
                else
                {
                    data.Data = targetPlanDetails;
                }

                data.Count = targetPlanDetails.Count();
                result.Object = data;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetTargetPlanDetailIsCompleted: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> SetSubmittedStateForDetailPendingTargetPlan(SubmitDetailPendingTartgetPlanSAPArg arg)
        {
            var result = new ResultDTO { };
            List<SubmitTargetPlanArg> listSubmit = new List<SubmitTargetPlanArg>();
            try
            {
                List<string> listSap = arg.ListSAPCode.Split(',').ToList();
                var pTrgetDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => listSap.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                if (pTrgetDetails.Count() > 0) //có targetplandetail thì mới tạo TargetPlan cho workflow chạy
                {
                    foreach (var item in pTrgetDetails)
                    {
                        item.IsSubmitted = true;
                    }
                    await _uow.CommitAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at SetSubmittedStateForDetailPendingTargetPlan: " + ex.Message);
                result.Messages = new List<string> { ex.Message };
                return result;
            }
        }

        public async Task<ResultDTO> GetList(QueryArgs arg)
        {
            var items = await _uow.GetRepository<TargetPlan>().FindByAsync<TargetPlanViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);
            // loi k tim tay modified nen lay tu worflow histories
            if (!(items is null))
            {
                try
                {
                    foreach (var item in items)
                    {
                        var workflowHistories = await _uow.GetRepository<WorkflowHistory>().FindByAsync(x => x.Instance != null && x.Instance.ItemId == item.Id, "created desc");
                        if (!(workflowHistories is null) && workflowHistories.Any())
                            item.Modified = workflowHistories.FirstOrDefault().Modified;
                    }
                } catch (Exception e)
                {
                    _logger.LogError("Error GetList Target Plan: " + e.Message);
                }
            }
            var count = await _uow.GetRepository<TargetPlan>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = items,
                    Count = count
                }
            };
        }

        public async Task<ResultDTO> GetItem(Guid id)
        {
            var result = new ResultDTO();
            try
            {
                var currentTargetPlan = await _uow.GetRepository<TargetPlan>().FindByIdAsync<TargetPlanViewModel>(id);
                if (currentTargetPlan != null)
                {
                    var detailItems = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlanId == id);
                    if (detailItems.Any())
                    {
                        currentTargetPlan.TargetPlanDetails = detailItems;
                    }
                    result.Object = currentTargetPlan;
                }
                else
                {
                    result.Messages.Add("Not Found");
                    result.ErrorCodes.Add(1004);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0}-Error Message: Get Target Plan Item " + ex.Message));
            }
            return result;
        }

        public async Task<ResultDTO> GetSAPCode_PendingTargetPlanDetails(Guid id)
        {
            var result = new ResultDTO();
            try
            {
                var currentTargetPlan = await _uow.GetRepository<TargetPlan>().FindByIdAsync<TargetPlanViewModel>(id);
                if (currentTargetPlan != null)
                {
                    var detailItems = await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanDetailViewModel>(x => x.TargetPlanId == id);
                    if (detailItems.Any())
                    {
                        List<string> userSAPCode = detailItems.Select(x => x.SAPCode).Distinct().ToList();
                        var pendingTargetPlanDetails = await _uow.GetRepository<PendingTargetPlanDetail>()
                            .FindByAsync<PendingTargetPlanDetailViewModel>(x => x.PeriodId == currentTargetPlan.PeriodId
                            && userSAPCode.Contains(x.SAPCode)
                            && (x.IsSent == false || (x.IsSent == true && x.IsSubmitted==false) || currentTargetPlan.Status.ToLower() == "draft"));
                        //Get current pending user SAPCode
                        userSAPCode = pendingTargetPlanDetails?.Select(x => x.SAPCode).Distinct().ToList() ?? new List<string>();
                        if (userSAPCode.Count > 0)
                        {
                            result.Object = await _uow.GetRepository<User>(true).FindByAsync<UserListViewModel>(currentUser => currentUser.IsActivated && userSAPCode.Contains(currentUser.SAPCode));
                        }
                    }
                }
                else
                {
                    result.Messages.Add("Not Found");
                    result.ErrorCodes.Add(1004);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0}-Error Message: Get Target Plan Item " + ex.Message));
            }
            return result;
        }

        public async Task<ResultDTO> ValidateTargetPlan(TargetPlanArg arg)
        {
            var result = new ResultDTO();
            arg.PeriodFromDate = arg.PeriodFromDate.ToLocalTime();
            arg.PeriodToDate = arg.PeriodToDate.ToLocalTime();
            List<ResultValidateTargetPlanViewModel> objectValidate = new List<ResultValidateTargetPlanViewModel>();

            var findAllJobGrade = await _uow.GetRepository<JobGrade>().FindByAsync(x => !x.IsDeleted);

            var findJD1 = findAllJobGrade?.FirstOrDefault(x => x.Title.Equals("g1", StringComparison.OrdinalIgnoreCase))?.Grade ?? 1;
            var findJD2 = findAllJobGrade?.FirstOrDefault(x => x.Title.Equals("g2", StringComparison.OrdinalIgnoreCase))?.Grade ?? 2;
            var findJD3 = findAllJobGrade?.FirstOrDefault(x => x.Title.Equals("g3", StringComparison.OrdinalIgnoreCase))?.Grade ?? 3;

            var sapGroups = arg.List.GroupBy(x => x.SAPCode);
            var jobGrades = await _uow.GetRepository<JobGrade>().FindByAsync(x => !x.IsDeleted);

            foreach (var items in sapGroups)
            {
                var erdRemain = 0.0;
                var alRemain = 0.0;
                var doflRemain = 0.0;
                RedundantPRDDataDTO redundantPRD = null;
                var user = await GetInformationUser(items.Key);
                var quotaData = string.IsNullOrEmpty(user.QuotaDataJson) ? null : JsonConvert.DeserializeObject<QuotaDataJsonDTO>(user.QuotaDataJson);
                if (quotaData != null)
                {
                    var currentQuotaData = quotaData.JsonData.FirstOrDefault(x => x.Year == arg.PeriodToDate.Year);
                    if (currentQuotaData != null)
                    {
                        erdRemain = currentQuotaData.ERDRemain;
                        alRemain = currentQuotaData.ALRemain;
                        doflRemain = currentQuotaData.DOFLRemain;
                    }
                    if (arg.PeriodFromDate.Year != arg.PeriodToDate.Year)
                    {
                        var allRemain = quotaData.JsonData.Where(x => x.Year == arg.PeriodToDate.Year || x.Year == arg.PeriodFromDate.Year);
                        erdRemain = allRemain.Sum(x => x.ERDRemain);
                        alRemain = allRemain.Sum(x => x.ALRemain);
                        doflRemain = allRemain.Sum(x => x.DOFLRemain);
                    }
                }
                var resignationApplicationByUser = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(x => x.Status.Contains("Completed") && x.UserSAPCode == items.Key, "Created desc");
                var range = new List<DateTimeOffset>();
                var erdQuality = items.Sum(x => x.ERDQuality);
                var prdQuality = items.Sum(x => x.PRDQuality);
                var alQuality = items.Sum(x => x.ALHQuality);
                var doflQuality = items.Sum(x => x.DOFLQuality);
                foreach (var target in items)
                {
                    if (!target.IsSubmitted)
                    {
                        if (target.Type == TypeTargetPlan.Target1)
                        {
                            bool salaryCycle = false;
                            if (user.StartDate != null)
                            {
                                DateTimeOffset startDate = new DateTimeOffset(user.StartDate.Value);
                                //get khoảng ngày trong 1 kỳ
                                if (resignationApplicationByUser != null && resignationApplicationByUser.SuggestionForLastWorkingDay != null && resignationApplicationByUser.SuggestionForLastWorkingDay.HasValue && resignationApplicationByUser.SuggestionForLastWorkingDay <= arg.PeriodToDate && resignationApplicationByUser.SuggestionForLastWorkingDay >= arg.PeriodFromDate)
                                {
                                    if (startDate >= arg.PeriodFromDate)
                                    {
                                        if (startDate < resignationApplicationByUser.SuggestionForLastWorkingDay)
                                        {
                                            range = renderDate(startDate, resignationApplicationByUser.SuggestionForLastWorkingDay.Value);
                                            salaryCycle = true;
                                        }
                                        else
                                        {
                                            range = renderDate(startDate, arg.PeriodToDate);
                                            salaryCycle = true;
                                        }
                                    }
                                    else
                                    {
                                        range = renderDate(arg.PeriodFromDate, resignationApplicationByUser.SuggestionForLastWorkingDay.Value);
                                        salaryCycle = true;
                                    }
                                }
                                else
                                {
                                    if (startDate > arg.PeriodFromDate)
                                    {
                                        range = renderDate(startDate, arg.PeriodToDate);
                                        salaryCycle = true;
                                    }
                                    else
                                    {
                                        range = renderDate(arg.PeriodFromDate, arg.PeriodToDate);
                                        salaryCycle = true;
                                    }
                                }
                                //ERD
                                if (user.IsStore)
                                {
                                    if (!string.IsNullOrEmpty(user.RedundantPRD))
                                    {
                                        redundantPRD = JsonConvert.DeserializeObject<RedundantPRDDataDTO>(user.RedundantPRD);
                                    }
                                    var jd3 = jobGrades.FirstOrDefault(x => x.Title.ToLower().Trim().Equals("g3"))?.Grade ?? 3;
                                    var jd2 = jobGrades.FirstOrDefault(x => x.Title.ToLower().Trim().Equals("g2"))?.Grade ?? 2;
                                    if (user.JobGradeValue >= jd3)
                                    {
                                        double numberDaysInRange = range.Count;
                                        double numberDatsInPeriod = (arg.PeriodToDate - arg.PeriodFromDate).TotalDays + 1;
                                        if (numberDaysInRange == numberDatsInPeriod) // Trọn 1 chu kì lương -> tính rule cũ
                                        {
                                            int sumSunday = range.Where(x => x.DayOfWeek == DayOfWeek.Sunday).Count();
                                            double userERD = 6 - sumSunday + erdRemain;
                                            if (erdQuality > userERD)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                        else
                                        {
                                            var numberOfPrds = 0;
                                            var temp = arg.PeriodFromDate;
                                            while (temp <= arg.PeriodToDate)
                                            {
                                                if (temp.DayOfWeek == DayOfWeek.Sunday)
                                                {
                                                    numberOfPrds++;
                                                }
                                                temp = temp.AddDays(1);
                                            }
                                            if (numberOfPrds == 4) // 2 PRD là có 1 ERD
                                            {
                                                if (prdQuality >= 2 && erdQuality > 1)
                                                {
                                                    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                    objectValidate.Add(value);
                                                }
                                            }
                                            else if (numberDatsInPeriod == 5) // Không có ERD nào 
                                            {
                                                if (erdQuality > 0)
                                                {
                                                    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                    objectValidate.Add(value);
                                                }
                                            }
                                        }
                                    }
                                    else if (user.JobGradeValue == jd2)
                                    {
                                        double numberDaysInRange = range.Count;
                                        double numberDatsInPeriod = (arg.PeriodToDate - arg.PeriodFromDate).TotalDays + 1;
                                        if (numberDaysInRange == numberDatsInPeriod) // Trọn 1 chu kì lương -> tính rule cũ
                                        {
                                            int sumSunday = range.Where(x => x.DayOfWeek == DayOfWeek.Sunday).Count();
                                            double userERD = 5 - sumSunday + erdRemain;
                                            if (erdQuality > userERD)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                        else
                                        {
                                            var numberOfPrds = 0;
                                            var temp = arg.PeriodFromDate;
                                            while (temp <= arg.PeriodToDate)
                                            {
                                                if (temp.DayOfWeek == DayOfWeek.Sunday)
                                                {
                                                    numberOfPrds++;
                                                }
                                                temp = temp.AddDays(1);
                                            }
                                            if (numberOfPrds == 4) // 2 PRD là có 1 ERD
                                            {
                                                if (prdQuality >= 2 && erdQuality > 1)
                                                {
                                                    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                    objectValidate.Add(value);
                                                }
                                            }
                                            else if (numberDatsInPeriod == 5) // Không có ERD nào 
                                            {
                                                if (erdQuality > 0)
                                                {
                                                    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                    objectValidate.Add(value);
                                                }
                                            }
                                        }
                                    }

                                    //if (user.JobGradeValue == 2 || user.JobGradeValue == 1)
                                    if (user.JobGradeValue == 1)
                                    {
                                        if (erdQuality > 0)
                                        {
                                            // var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "G2" };
                                            var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "G2" };
                                            objectValidate.Add(value);
                                        }
                                    }

                                }
                                else
                                {
                                    int sumSaturday = range.Where(x => x.DayOfWeek == DayOfWeek.Saturday).Count();
                                    //Thanh add condition here
                                    //31/05/2021
                                    DateTime limitDateRD = new DateTime(arg.PeriodToDate.Year, arg.PeriodToDate.Month, 15);
                                    if (resignationApplicationByUser != null)
                                    {
                                        var lastWorkingDate = resignationApplicationByUser.OfficialResignationDate;

                                        if (lastWorkingDate.Date > limitDateRD.Date)
                                        {
                                            double userERD = sumSaturday - 1 + erdRemain;
                                            if (sumSaturday == 0)
                                            {
                                                userERD = erdRemain;
                                            }

                                            if (erdQuality > userERD)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        double userERD = sumSaturday - 1 + erdRemain;
                                        if (sumSaturday == 0)
                                        {
                                            userERD = erdRemain;
                                        }
                                        if (userERD == 0 && startDate.Date > limitDateRD.Date)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (erdQuality > userERD)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "ERD" };
                                                objectValidate.Add(value);
                                            }
                                        }

                                    }
                                }
                                //PRD
                                double daySun = range.Where(x => x.DayOfWeek == DayOfWeek.Sunday).Count();
                                /*if (redundantPRD != null)
                                {
                                    RedundantPRDDTO currentRedundantPRD = null;
                                    if (arg.PeriodToDate.Month == 1)
                                    {
                                        currentRedundantPRD = redundantPRD.JsonData.FirstOrDefault(x => x.Month == 12 && x.Year == arg.PeriodToDate.Year - 1);
                                        if (currentRedundantPRD != null)
                                        {
                                            daySun += currentRedundantPRD.PRDRemain;
                                        }
                                    }
                                    else
                                    {
                                        currentRedundantPRD = redundantPRD.JsonData.FirstOrDefault(x => x.Month == arg.PeriodToDate.Month - 1 && x.Year == arg.PeriodToDate.Year);
                                        if (currentRedundantPRD != null)
                                        {
                                            daySun += currentRedundantPRD.PRDRemain;
                                        }
                                    }
                                }*/
                                if (salaryCycle)
                                {
                                    var currentUser = user;
                                    var isSpecical = await CheckSpecialCase(user.Id);
                                    if (isSpecical== true)
                                    {
                                        if (prdQuality > daySun+1|| prdQuality < daySun - 1)// validate specical target plan  cr 9.9
                                        {
                                            var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "PRDSPECTICAL" };
                                            objectValidate.Add(value);
                                        }
                                    }
                                    else
                                    {
                                        if (prdQuality != daySun)// validate specical target plan  cr 9.9
                                        {
                                            var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "PRDSPECTICAL" };
                                            objectValidate.Add(value);
                                        }
                                    }
                                    //if (prdQuality > daySun)// root
                                    //{
                                    //    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "PRD" };
                                    //    objectValidate.Add(value);
                                    //}
                                }
                                else if (daySun == 1)
                                {
                                    if (prdQuality > 1)
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "PRD" };
                                        objectValidate.Add(value);
                                    }
                                }
                                else if (daySun == 3)
                                {
                                    if (prdQuality > 3)
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "PRD" };
                                        objectValidate.Add(value);
                                    }
                                }
                                //// AL
                                //if (alQuality != 0 && alQuality > alRemain)
                                //{
                                //    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "AL" };
                                //    objectValidate.Add(value);
                                //}
                                ////DOFL
                                //if (doflQuality != 0 && doflQuality > doflRemain)
                                //{
                                //    var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "DOFL" };
                                //    objectValidate.Add(value);
                                //}


                                //code moi
                                //check năm của Period có khác nhau ko -> nếu có thì check thêm alRemain có = 0 ko -> nếu có thì lấy lại alRemain của năm PeriodTo
                                if (arg.PeriodFromDate.Year == arg.PeriodToDate.Year)
                                {
                                    // AL
                                    //lay cac ngay AL trong TH co phieu leave cho duyet
                                    var alIsWaiting = 0.0;
                                    var inValidStatus = new string[] { "draft", "rejected", "requested to change", "cancelled", "completed" };
                                    var existItems = await _uow.GetRepository<LeaveApplicationDetail>().FindByAsync(x => (x.FromDate.Year == arg.PeriodFromDate.Year && x.ToDate.Year == arg.PeriodFromDate.Year) && x.LeaveCode == "AL"  && x.LeaveApplication.UserSAPCode == target.SAPCode && !inValidStatus.Contains(x.LeaveApplication.Status.ToLower()));
                                    if (existItems.Any())
                                    {
                                        alIsWaiting = existItems.Sum(x => x.Quantity);
                                    }

                                    var cfRemain = target.CFRemain != null ? target.CFRemain : 0;

                                    var alToChecking = alQuality + alIsWaiting;

                                    if (alQuality != 0 && alToChecking > (alRemain + cfRemain))
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "AL" };
                                        objectValidate.Add(value);
                                    }

                                    /*if (alQuality != 0 && alQuality > alRemain)
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "AL" };
                                        objectValidate.Add(value);
                                    }*/

                                    //DOFL
                                    if (doflQuality != 0 && doflQuality > doflRemain)
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "DOFL" };
                                        objectValidate.Add(value);
                                    }
                                }
                                else
                                {
                                    if (quotaData != null)
                                    {
                                        var currentQuotaDataPeriodLastYear = quotaData.JsonData.FirstOrDefault(x => x.Year == arg.PeriodFromDate.Year);
                                        var currentQuotaDataPeriodNewYear = quotaData.JsonData.FirstOrDefault(x => x.Year == arg.PeriodToDate.Year);
                                        // AL
                                        var alQualityLastYear = (JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(target.JsonData).Where(y => y.date.Contains(arg.PeriodFromDate.Year.ToString()) && y.value.Contains("AL"))).ToList().Count;
                                        var alQualityNewYear = (JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(target.JsonData).Where(y => y.date.Contains(arg.PeriodToDate.Year.ToString()) && y.value.Contains("AL"))).ToList().Count;
                                        if (currentQuotaDataPeriodLastYear != null && currentQuotaDataPeriodLastYear.ALRemain == 0)
                                        {
                                            //trường hợp data bên SAP trả về với năm cũ = 0 thì lấy tổng số AL user nhập lại check với data SAP trả về với năm mới
                                            if (alQuality != 0 && currentQuotaDataPeriodNewYear != null && currentQuotaDataPeriodNewYear != null && alQuality > currentQuotaDataPeriodNewYear.ALRemain)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "AL" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                        else
                                        {
                                            //trường hợp data bên SAP trả về với năm cũ != 0 thì lấy số AL năm cũ mà user đã nhập check với data SAP trả về của năm cũ và lấy số AL năm mới mà user đã nhập check với data SAP trả về của năm mới
                                            //ví dụ: AL-SAP-2020, thì lấy data của năm 2020 check với AL-SAP-2020/ AL-SAP-2021, thì lấy data của năm 2021 check với AL-SAP-2021
                                            if ((currentQuotaDataPeriodLastYear != null && (alQualityLastYear != 0 && alQualityLastYear > currentQuotaDataPeriodLastYear.ALRemain)) || (currentQuotaDataPeriodNewYear != null && (alQualityNewYear != 0 && alQualityNewYear > currentQuotaDataPeriodNewYear.ALRemain)))
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "AL" };
                                                objectValidate.Add(value);
                                            }

                                        }
                                        //DOFL
                                        var doflQualityLastYear = (JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(target.JsonData).Where(y => y.date.Contains(arg.PeriodFromDate.Year.ToString()) && y.value.Contains("DOFL"))).ToList().Count;
                                        var doflQualityNewYear = (JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(target.JsonData).Where(y => y.date.Contains(arg.PeriodToDate.Year.ToString()) && y.value.Contains("DOFL"))).ToList().Count;
                                        if (currentQuotaDataPeriodLastYear != null && currentQuotaDataPeriodLastYear.DOFLRemain == 0)
                                        {
                                            //trường hợp data bên SAP trả về với năm cũ = 0 thì lấy tổng số DOFL user nhập lại check với data SAP trả về với năm mới
                                            if (doflQuality != 0 && currentQuotaDataPeriodNewYear != null && doflQuality > currentQuotaDataPeriodNewYear.DOFLRemain)
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "DOFL" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                        else
                                        {
                                            //trường hợp data bên SAP trả về với năm cũ != 0 thì lấy số DOFL năm cũ mà user đã nhập check với data SAP trả về của năm cũ và lấy số DOFL năm mới mà user đã nhập check với data SAP trả về của năm mới
                                            //ví dụ: DOFL-SAP-2020, thì lấy data của năm 2020 check với DOFL-SAP-2020/ DOFL-SAP-2021, thì lấy data của năm 2021 check với DOFL-SAP-2021
                                            if ((currentQuotaDataPeriodLastYear != null && (doflQualityLastYear != 0 && doflQualityLastYear > currentQuotaDataPeriodLastYear.DOFLRemain)) || (currentQuotaDataPeriodNewYear != null && (doflQualityNewYear != 0 && doflQualityNewYear > currentQuotaDataPeriodNewYear.DOFLRemain)))
                                            {
                                                var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "DOFL" };
                                                objectValidate.Add(value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

                result.Object = new ArrayResultDTO { Data = objectValidate.OrderBy(x => x.TypeName), Count = objectValidate.Count() };
            return result;
        }

        public async Task<ResultDTO> ValidateTargetPlanV2(TargetPlanArg arg)
        {
            var result = new ResultDTO();
            arg.PeriodFromDate = arg.PeriodFromDate.ToLocalTime();
            arg.PeriodToDate = arg.PeriodToDate.ToLocalTime();
            List<ResultValidateTargetPlanViewModel> objectValidate = new List<ResultValidateTargetPlanViewModel>();
            var sapGroups = arg.List.GroupBy(x => x.SAPCode);
            var listWFHHaftDay = new List<string>() { "WFH2", "WFH1" };
            foreach (var items in sapGroups)
            {
                var user = await GetInformationUser(items.Key);
                var resignationApplicationByUser = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(x => x.Status.Contains("Completed") && x.UserSAPCode == items.Key);
                var range = new List<DateTimeOffset>();
                if (user.StartDate != null)
                {
                    DateTimeOffset startDate = new DateTimeOffset(user.StartDate.Value);
                    //get khoảng ngày trong 1 kỳ
                    if (resignationApplicationByUser != null && resignationApplicationByUser.SuggestionForLastWorkingDay != null && resignationApplicationByUser.SuggestionForLastWorkingDay.HasValue && resignationApplicationByUser.SuggestionForLastWorkingDay.Value <= arg.PeriodToDate && resignationApplicationByUser.SuggestionForLastWorkingDay.Value >= arg.PeriodFromDate)
                    {
                        if (startDate >= arg.PeriodFromDate)
                        {
                            if (startDate < resignationApplicationByUser.SuggestionForLastWorkingDay.Value)
                            {
                                range = renderDate(startDate, resignationApplicationByUser.SuggestionForLastWorkingDay.Value);
                            }
                            else
                            {
                                range = renderDate(startDate, arg.PeriodToDate);
                            }
                        }
                        else
                        {
                            range = renderDate(arg.PeriodFromDate, (resignationApplicationByUser.SuggestionForLastWorkingDay != null && resignationApplicationByUser.SuggestionForLastWorkingDay.HasValue ? resignationApplicationByUser.SuggestionForLastWorkingDay.Value : resignationApplicationByUser.OfficialResignationDate));
                        }
                    }
                    else
                    {
                        if (startDate > arg.PeriodFromDate)
                        {
                            range = renderDate(startDate, arg.PeriodToDate);
                        }
                        else
                        {
                            range = renderDate(arg.PeriodFromDate, arg.PeriodToDate);
                        }
                    }
                }

                var itemsTarget1 = items.Where(x => x.Type == TypeTargetPlan.Target1);
                var itemsTarget2 = items.Where(x => x.Type == TypeTargetPlan.Target2);

                var currentUser = _uow.GetRepository<User>().FindById(user.Id);
                // CR 10.2: AEONCR102-6: HR-1394: Set rule chặn WFH theo tuần + JG
                foreach (var target in itemsTarget1)
                {
                    List<TargetDateDetail> details = JsonConvert.DeserializeObject<List<TargetDateDetail>>(target.JsonData);
                    details = details.Where(x => !string.IsNullOrEmpty(x.value)).ToList();
                    DateTimeOffset startDate = range[0];
                    DateTimeOffset endDate = range[range.Count - 1];
                    DateTimeOffset from = range[0];
                    bool condition = true;
                    while (condition)
                    {
                        try
                        {
                            double countWFH = 0;
                            DateTimeOffset to = GetNextSunday(from) > endDate ? endDate : GetNextSunday(from);

                            // tinh chu ki truoc
                            if (from.Date == startDate && from.Date.DayOfWeek != DayOfWeek.Monday)
                            {
                                var previousSunday = GetPreviousSundayList(from.Date);
                                if (previousSunday.Any())
                                {
                                    foreach (var item in previousSunday)
                                    {
                                        var currentActual1 = currentUser.GetActualTarget1_ByDate(_uow, item.Date, true);
                                        if (currentActual1 != null)
                                        {
                                            if (currentActual1.value == "WFH")
                                            {
                                                countWFH += 1;
                                            }
                                            else if (listWFHHaftDay.Contains(currentActual1.value))
                                            {
                                                countWFH += 0.5;
                                            }
                                        }
                                        var currentActual2 = currentUser.GetActualTarget2_ByDate(_uow, item.Date, true);
                                        if (currentActual2 != null && !string.IsNullOrEmpty(currentActual2.value))
                                        {
                                            if (currentActual2.value == "WFH")
                                            {
                                                countWFH += 1;
                                            }
                                            else if (listWFHHaftDay.Contains(currentActual2.value))
                                            {
                                                countWFH += 0.5;
                                            }
                                        }
                                    }
                                }
                            } 

                            var validateWeek = details.Where(x => DateTimeOffset.ParseExact(x.date, "yyyyMMdd", null) >= from && DateTimeOffset.ParseExact(x.date, "yyyyMMdd", null) <= to).ToList();
                            if (validateWeek.Any())
                            {
                                foreach (var week in validateWeek)
                                {
                                    if (week.value == "WFH")
                                    {
                                        countWFH += 1;
                                    }
                                    else if (listWFHHaftDay.Contains(week.value))
                                    {
                                        countWFH += 0.5;
                                    }
                                }

                                foreach (var target2 in itemsTarget2)
                                {
                                    List<TargetDateDetail> details2 = JsonConvert.DeserializeObject<List<TargetDateDetail>>(target2.JsonData);
                                    if (details2.Any())
                                    {
                                        var validateWeekTarget2 = details2.Where(x => DateTimeOffset.ParseExact(x.date, "yyyyMMdd", null) >= from && DateTimeOffset.ParseExact(x.date, "yyyyMMdd", null) <= to).ToList();
                                        if (validateWeekTarget2.Any())
                                        {
                                            foreach (var week in validateWeekTarget2)
                                            {
                                                if (week.value == "WFH")
                                                {
                                                    countWFH += 1;
                                                }
                                                else if (listWFHHaftDay.Contains(week.value))
                                                {
                                                    countWFH += 0.5;
                                                }
                                            }
                                        }
                                    }
                                }
                                #region ACR-80 Điều chỉnh rule Validate ngày WFH trên phiếu TARGET
                                if (user.MaxWFH != null)
                                {
                                    if (countWFH > user.MaxWFH)
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "WFH", Description = (from.ToString("dd/MM/yyyy") + " - " + to.ToString("dd/MM/yyyy")) };
                                        objectValidate.Add(value);
                                    }
                                } else
                                {
                                    if ((user.JobGradeValue >= 4 && countWFH > 2) || (user.JobGradeValue < 4 && countWFH > 1))
                                    {
                                        var value = new ResultValidateTargetPlanViewModel { SAPCode = target.SAPCode, TypeName = "WFH", Description = (from.ToString("dd/MM/yyyy") + " - " + to.ToString("dd/MM/yyyy")) };
                                        objectValidate.Add(value);
                                    }
                                }
                                #endregion


                            }
                            // break;
                            if (to >= endDate)
                            {
                                condition = false;
                            } else
                            {
                                from = to.AddDays(1);
                            }    
                        } catch (Exception e)
                        {
                            condition = false;
                        }
                    }
                }
            }

            result.Object = new ArrayResultDTO { Data = objectValidate.OrderBy(x => x.TypeName), Count = objectValidate.Count() };
            return result;
        }


        public DateTimeOffset GetNextSunday(DateTimeOffset targetDate)
        {
            int daysUntilNextSunday = (int)DayOfWeek.Sunday - (int)targetDate.DayOfWeek;
            if (daysUntilNextSunday <= 0)
            {
                daysUntilNextSunday += 7;
            }
            DateTimeOffset nextSunday = targetDate.AddDays(daysUntilNextSunday);
            return nextSunday;
        }

        public List<DateTimeOffset> GetPreviousSundayList(DateTimeOffset targetDate)
        {
            List<DateTimeOffset> result = new List<DateTimeOffset>();
            // Start from the given date and go back to the previous Sundays, adding them to the list
            DateTimeOffset currentDay = targetDate;
            while (currentDay.DayOfWeek != DayOfWeek.Sunday)
            {
                currentDay = currentDay.AddDays(-1);
            }
            for (DateTimeOffset currentDate = currentDay; currentDate <= targetDate; currentDate = currentDate.AddDays(1))
            {
                if (currentDate.DayOfWeek != DayOfWeek.Sunday && targetDate != currentDate) 
                    result.Add(currentDate);
            }
            return result;
        }

        private async Task<EmployeeViewModel> GetInformationUser(string SAPCode)
        {
            var user = await _uow.GetRepository<User>().GetSingleAsync(x => x.SAPCode == SAPCode);
            var employeeInfo = new EmployeeViewModel();
            var edocEmployee = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == user.Id, string.Empty);
            if (edocEmployee != null)
            {
                employeeInfo.Id = edocEmployee.Id;
                employeeInfo.FullName = edocEmployee.FullName;
                employeeInfo.Email = edocEmployee.Email;
                employeeInfo.SAPCode = edocEmployee.SAPCode;
                employeeInfo.Role = edocEmployee.Role;
                employeeInfo.Type = edocEmployee.Type;
                employeeInfo.StartDate = edocEmployee.StartDate;
                employeeInfo.IsNotTargetPlan = edocEmployee.IsNotTargetPlan;
                //employeeInfo.ErdRemain = user.ErdRemain;
                //employeeInfo.DoflRemain = user.DoflRemain;
                //employeeInfo.AlRemain = user.AlRemain;
                employeeInfo.QuotaDataJson = user.QuotaDataJson;
                employeeInfo.RedundantPRD = user.RedundantPRD;
                //Ngan them
                employeeInfo.ProfilePictureId = edocEmployee.ProfilePictureId;
                employeeInfo.ProfilePicture = (edocEmployee.ProfilePicture != null && !string.IsNullOrEmpty(edocEmployee.ProfilePicture.FileUniqueName) ? " /Attachments/" + edocEmployee.ProfilePicture.FileUniqueName : string.Empty);

                //End
                var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == edocEmployee.Id && t.IsHeadCount), "", x => x.JobGrade);
                if (department != null)
                {
                    if (department.Type == DepartmentType.Department)
                    {
                        employeeInfo.DeptCode = department.Code;
                        employeeInfo.DeptName = department.Name;
                        employeeInfo.DeptId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                    }
                    else
                    {
                        employeeInfo.DivisionCode = department.Code;
                        employeeInfo.DivisionName = department.Name;
                        employeeInfo.DivisionId = department.Id;
                        employeeInfo.IsStore = department.IsStore;
                        employeeInfo.IsHR = department.IsHR;
                        var skip = false;
                        var departmentIdx = department.ParentId;
                        while (!skip)
                        {
                            if (departmentIdx.HasValue)
                            {
                                var dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == departmentIdx);
                                if (dept.Type == DepartmentType.Department)
                                {
                                    employeeInfo.DeptCode = dept.Code;
                                    employeeInfo.DeptName = dept.Name;
                                    employeeInfo.DeptId = dept.Id;
                                    skip = true;
                                }
                                else
                                {
                                    departmentIdx = dept.ParentId;
                                }
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                    }
                    employeeInfo.JobGradeName = department.JobGrade.Caption;
                    employeeInfo.JobGradeId = department.JobGrade.Id;
                    employeeInfo.JobGradeValue = department.JobGrade.Grade;
                    employeeInfo.MaxWFH = department.JobGrade.MaxWFH;
                    var submitPersonsMappings = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson && x.DepartmentId == department.Id);
                    if (submitPersonsMappings.Any())
                    {
                        employeeInfo.SubmitPersons = submitPersonsMappings.Select(x => x.SAPCode);
                    }
                }
            }
            return employeeInfo;
        }

        private List<DateTimeOffset> renderDate(DateTimeOffset from, DateTimeOffset to)
        {
            int days = (to - from).Days + 1;
            string day = to.Day.ToString();
            List<DateTimeOffset> range = Enumerable.Range(0, days).Select(i => from.AddDays(i)).ToList();
            return range;
        }

        public async Task<ResultDTO> UploadData(TargetPlanQuerySAPCodeArg arg, Stream stream)
        {
            var result = new ResultDTO();
            var data = new ArrayResultDTO();
            var errorSAPCode = new List<string>();
            List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
            if (!arg.VisibleSubmit)
            {
                var existsTarget = await _uow.GetRepository<PendingTargetPlanDetail>().GetSingleAsync<PendingTargetPlanDetailViewModel>(i => arg.SAPCodes.Contains(i.SAPCode) && i.IsSent && i.PendingTargetPlan.PeriodId == arg.PeriodId);
                if (existsTarget != null)
                {
                    result.ErrorCodes.Add(1001);
                    result.Messages.Add("TARGET_PLAN_VALIDATE_EXISTS_TARGET");
                    return result;
                }
            }
            var users = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => arg.SAPCodes.Contains(x.SAPCode) && x.IsActivated);
            var dataFromFile = ReadDataFromStream(stream, users);
            if (dataFromFile.Data.Count > 0)
            {
                var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                if (currentPeriod != null)
                {
                    if (dataFromFile.PeriodFromDate.Value.Date.ToSAPFormat().Equals(currentPeriod.FromDate.Date.ToSAPFormat()) && (dataFromFile.PeriodToDate.Value.Date.ToSAPFormat().Equals(currentPeriod.ToDate.Date.ToSAPFormat())))
                    {
                        var groupSapcodes = dataFromFile.Data.GroupBy(x => x.SAPCode);
                        foreach (var group in groupSapcodes)
                        {
                            foreach (var item in group)
                            {
                                var currentUser = users.Where(x => x.SAPCode == item.SAPCode).FirstOrDefault();
                                if (currentUser != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Target) && item.Target.Trim().ToLower().Contains("1-target"))
                                    {
                                        targetPlanDetails.Add(new TargetPlanArgDetail
                                        {
                                            SAPCode = item.SAPCode,
                                            FullName = currentUser.FullName,
                                            JsonData = JsonConvert.SerializeObject(item.Targets),
                                            Type = TypeTargetPlan.Target1,
                                            ALHQuality = item.Targets.Where(x => x.value.Contains("AL")).Count(),
                                            ERDQuality = item.Targets.Where(x => x.value.Contains("ERD")).Count(),
                                            PRDQuality = item.Targets.Where(x => x.value.Contains("PRD")).Count(),
                                            DOFLQuality = item.Targets.Where(x => x.value.Contains("DOFL")).Count(),
                                            DepartmentCode = currentUser?.DepartmentCode,
                                            DepartmentName = currentUser?.Department,
                                            PeriodId = arg.PeriodId
                                        });

                                    }
                                    else if (!string.IsNullOrEmpty(item.Target) && item.Target.Trim().ToLower().Contains("2-target"))
                                    {
                                        targetPlanDetails.Add(new TargetPlanArgDetail
                                        {
                                            SAPCode = item.SAPCode,
                                            FullName = currentUser.FullName,
                                            JsonData = JsonConvert.SerializeObject(item.Targets),
                                            Type = TypeTargetPlan.Target2,
                                            ALHQuality = (float)item.Targets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2,
                                            ERDQuality = (float)item.Targets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2,
                                            PRDQuality = (float)item.Targets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2,
                                            DOFLQuality = (float)item.Targets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2,
                                            DepartmentCode = currentUser?.DepartmentCode,
                                            DepartmentName = currentUser?.Department,
                                            PeriodId = arg.PeriodId
                                        });

                                    }

                                }

                            }
                        }
                        // Validate không có PRD
                        var noPRD = await ValidateNoPRD(targetPlanDetails, result, currentPeriod.FromDate.Date, currentPeriod.ToDate.Date);
                        if (noPRD.ErrorCodes.Count > 0)
                        {
                            return result;
                        }

                        // Validate ca nữa ngày chỉ được nhập khi Target1 có ca đầu V
                        var halfDayInvalid = ValidateHalfDay(targetPlanDetails, result);
                        if (halfDayInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }

                        //jira - 593
                        var sapCodeInvalid_StartDate = await ValidateStartDate(targetPlanDetails, result);
                        if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
                        {
                            return sapCodeInvalid_StartDate;
                        }
                        //================
                        //jira - 253
                        var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(targetPlanDetails, result);
                        if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
                        {
                            return sapCodeInvalid_ShiftCodeHQ;
                        }
                        //================

                        // validate mã ca có hợp lệ
                        var dataInvalid = await ValidateTargetPlanByJsonFile(targetPlanDetails, result, arg.PeriodId);
                        if (dataInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }
                        // validate Import File
                        var dataImportInvalid = ValidateImportData(targetPlanDetails, result);
                        if (dataImportInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }
                        var errors = await ValidateTargetPlanCustom(targetPlanDetails);
                        if (errors.ErrorCodes.Count == 0)
                        {
                            var res = await ValidateTargetPlan(new TargetPlanArg { PeriodFromDate = currentPeriod.FromDate, PeriodToDate = currentPeriod.ToDate, List = targetPlanDetails });
                            if (res.Object != null)
                            {
                                ArrayResultDTO arrayErrors = (ArrayResultDTO)res.Object;
                                if (arrayErrors.Count > 0)
                                {
                                    result.ErrorCodes.Add(8);
                                    result.Object = res.Object;
                                    return result;
                                } else
                                {
                                    var reCheck = await ValidateTargetPlanV2(new TargetPlanArg { PeriodFromDate = currentPeriod.FromDate, PeriodToDate = currentPeriod.ToDate, List = targetPlanDetails });
                                    if (res.Object != null)
                                    {
                                        arrayErrors = (ArrayResultDTO) res.Object;
                                        if (arrayErrors.Count > 0)
                                        {
                                            result.ErrorCodes.Add(8);
                                            result.Object = res.Object;
                                            return result;
                                        }
                                    }
                                }
                            }

                        }
                        if (errors.ErrorCodes.Count > 0)
                        {
                            result = errors;
                        }
                        else
                        {
                            data.Data = targetPlanDetails;
                            data.Count = targetPlanDetails.Count;
                        }

                    }
                    else
                    {
                        result.ErrorCodes.Add(1001);
                        result.Messages.Add("TARGET_PLAN_VALIDATE_PERIOD");
                    }
                }
                else
                {
                    result.ErrorCodes.Add(1001);
                    result.Messages.Add("Not Found Period");
                }

            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("TARGET_PLAN_VALIDATE_IMPORT_FILE");
            }
            if (data.Count > 0)
            {
                var addPendingTargetPlan = new List<PendingTargetPlanDetail>();
                arg.SAPCodes = targetPlanDetails.Select(x => x.SAPCode).Distinct().ToList();
                var currentPendingTargetPlans = await CheckExistPendingTargetPlans(new TargetPlanQueryArg { DepartmentId = arg.DeptId.Value, DivisionId = arg.DivisionId, PeriodId = arg.PeriodId, SAPCodes = arg.SAPCodes }); ;
                if (!currentPendingTargetPlans.Any()) // Chưa tạo pending Target Plan
                {

                    var pendingTarget = new PendingTargetPlan();
                    if (arg.DeptId.HasValue)
                    {
                        var deptLine = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DeptId.Value);
                        if (deptLine != null)
                        {
                            pendingTarget.DeptId = arg.DeptId.Value;
                            pendingTarget.DeptCode = deptLine.Code;
                            pendingTarget.DeptName = deptLine.Name;
                        }

                    }
                    if (arg.DivisionId.HasValue)
                    {
                        var divisions = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DivisionId.Value);
                        if (divisions != null)
                        {
                            pendingTarget.DivisionId = arg.DivisionId.Value;
                            pendingTarget.DivisionCode = divisions.Code;
                            pendingTarget.DivisionName = divisions.Name;
                        }
                    }
                    if (arg.PeriodId != Guid.Empty)
                    {
                        var period = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                        if (period != null)
                        {
                            pendingTarget.PeriodId = period.Id;
                            pendingTarget.PeriodName = period.Name;
                            pendingTarget.PeriodFromDate = period.FromDate;
                            pendingTarget.PeriodToDate = period.ToDate;
                        }
                    }
                    _uow.GetRepository<PendingTargetPlan>().Add(pendingTarget);
                    foreach (var item in targetPlanDetails)
                    {
                        item.PendingTargetPlanId = pendingTarget.Id;
                        addPendingTargetPlan.Add(new PendingTargetPlanDetail
                        {
                            Id = new Guid(),
                            DepartmentCode = item.DepartmentCode,
                            DepartmentName = item.DepartmentName,
                            SAPCode = item.SAPCode,
                            ALHQuality = item.ALHQuality,
                            ERDQuality = item.ERDQuality,
                            DOFLQuality = item.DOFLQuality,
                            PRDQuality = item.PRDQuality,
                            FullName = item.FullName,
                            JsonData = item.JsonData,
                            Type = item.Type,
                            PendingTargetPlanId = pendingTarget.Id,
                            PeriodId = arg.PeriodId,
                            IsSent = arg.VisibleSubmit

                        });
                    }
                    _uow.GetRepository<PendingTargetPlanDetail>().Add(addPendingTargetPlan);
                    await _uow.CommitAsync();
                }
                else
                {
                    //var existData = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => arg.SAPCodes.Contains(x.SAPCode) && x.PendingTargetPlan.PeriodId == arg.PeriodId);
                    var pendingPlanDetails = currentPendingTargetPlans.Where(x => (arg.VisibleSubmit ? !x.IsSubmitted : !x.IsSent));
                    var sapCodePendings = new List<string>();
                    if (pendingPlanDetails.Any())
                    {
                        foreach (var item in pendingPlanDetails)
                        {
                            var itemTarget = targetPlanDetails.FirstOrDefault(x => x.SAPCode == item.SAPCode && item.Type == x.Type);
                            if (itemTarget != null)
                            {
                                item.ALHQuality = itemTarget.ALHQuality;
                                item.ERDQuality = itemTarget.ERDQuality;
                                item.DOFLQuality = itemTarget.DOFLQuality;
                                item.PRDQuality = itemTarget.PRDQuality;
                                item.JsonData = itemTarget.JsonData;
                                itemTarget.PendingTargetPlanId = item.PendingTargetPlanId;
                                item.IsSent = arg.VisibleSubmit;
                            }
                            sapCodePendings.Add(item.SAPCode);
                        }
                        _uow.GetRepository<PendingTargetPlanDetail>().Update(pendingPlanDetails);
                    }
                    var notMadeTargetPlans = targetPlanDetails.Where(x => !sapCodePendings.Contains(x.SAPCode) && !currentPendingTargetPlans.Select(s => s.SAPCode).Contains(x.SAPCode));
                    if (notMadeTargetPlans.Any())
                    {
                        var pendingTarget = new PendingTargetPlan();
                        if (arg.DeptId.HasValue)
                        {
                            var deptLine = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DeptId.Value);
                            if (deptLine != null)
                            {
                                pendingTarget.DeptId = arg.DeptId.Value;
                                pendingTarget.DeptCode = deptLine.Code;
                                pendingTarget.DeptName = deptLine.Name;
                            }

                        }
                        if (arg.DivisionId.HasValue)
                        {
                            var divisions = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DivisionId.Value);
                            if (divisions != null)
                            {
                                pendingTarget.DivisionId = arg.DivisionId.Value;
                                pendingTarget.DivisionCode = divisions.Code;
                                pendingTarget.DivisionName = divisions.Name;
                            }
                        }
                        if (arg.PeriodId != Guid.Empty)
                        {
                            var period = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                            if (period != null)
                            {
                                pendingTarget.PeriodId = period.Id;
                                pendingTarget.PeriodName = period.Name;
                                pendingTarget.PeriodFromDate = period.FromDate;
                                pendingTarget.PeriodToDate = period.ToDate;
                            }
                        }
                        _uow.GetRepository<PendingTargetPlan>().Add(pendingTarget);
                        foreach (var item in notMadeTargetPlans)
                        {
                            //item.PendingTargetPlanId = currentPendingTargetPlan.Id;
                            addPendingTargetPlan.Add(new PendingTargetPlanDetail
                            {
                                DepartmentCode = item.DepartmentCode,
                                DepartmentName = item.DepartmentName,
                                SAPCode = item.SAPCode,
                                ALHQuality = item.ALHQuality,
                                ERDQuality = item.ERDQuality,
                                DOFLQuality = item.DOFLQuality,
                                PRDQuality = item.PRDQuality,
                                FullName = item.FullName,
                                Id = new Guid(),
                                JsonData = item.JsonData,
                                Type = item.Type,
                                PendingTargetPlanId = pendingTarget.Id,
                                PeriodId = arg.PeriodId,
                                IsSent = arg.VisibleSubmit
                            });
                        }
                        _uow.GetRepository<PendingTargetPlanDetail>().Add(addPendingTargetPlan);

                    }
                    await _uow.CommitAsync();
                }



            }
            result.Object = data;
            return result;
        }
        //private async Task<List<TargetPlanFromImportDetailItemDTO>> GetValidData(List<TargetPlanFromImportDetailItemDTO> data, UserForTreeViewModel user)
        //{
        //    var result = data;
        //    if (!user.OfficialResignationDate.HasValue)
        //    {
        //        user.OfficialResignationDate = new DateTime(3000, 10, 10);
        //    }
        //    if (!user.StartDate.HasValue)
        //    {
        //        foreach (var item in data)
        //        {
        //            if(item.date < user.StartDate.Value)
        //            {
        //                item.value = string.Empty;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var item in data)
        //        {
        //            item.value = string.Empty;
        //        }
        //    }
        //    return result;
        //}
        private async Task<ResultDTO> ValidateNoPRD(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO result, DateTime fromDate, DateTime toDate)
        {
            var errors = new List<ValidateHalfdayModel>();
            var sapCodeInvalid = new List<string>();
            if (targetPlanDetails.Any())
            {
                var sapCodes = targetPlanDetails.Select(s => s.SAPCode);
                var target1 = targetPlanDetails.Where(i => i.Type == TypeTargetPlan.Target1 && !i.IsSubmitted);
                foreach (var item in target1)
                {
                    var sunDate = string.Empty;
                    //// RULE CŨ Hơn or bằng 7 ngày
                    var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(i => i.SAPCode == item.SAPCode && i.IsActivated);
                    var resingation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(i => i.UserSAPCode == item.SAPCode && i.Status.Contains("Completed") && !i.IsCancelResignation, "Created Desc");
                    var startDate = fromDate;
                    var endDate = toDate;
                    if (resingation != null && resingation.OfficialResignationDate.Date < toDate)
                    {
                        endDate = resingation.OfficialResignationDate.Date;
                    }
                    if (currentUser.StartDate.HasValue && currentUser.StartDate.Value.Date > fromDate)
                    {
                        startDate = currentUser.StartDate.Value.Date;
                    }
                    while (startDate < endDate && sunDate == string.Empty)
                    {
                        if (startDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            sunDate = startDate.ToSAPFormat();
                        }
                        startDate = startDate.AddDays(1);
                    }
                    /// Trong khoảng có ngày chủ nhật

                    //var existSunday = mappings.Any(x => !String.IsNullOrEmpty(x.date) && x.date.ToDateTime().DayOfWeek == 0);
                    if (!string.IsNullOrEmpty(sunDate))
                    {
                        var mappings = JsonConvert.DeserializeObject<List<TargetPlanFromImportDetailItemDTO>>(item.JsonData);
                        bool existPRD = mappings.Any(i => i.value == "PRD");
                        if (!existPRD)
                        {
                            if (resingation != null)
                            {
                                bool isExistSundayinWorkingDays = mappings.Any(x => !String.IsNullOrEmpty(x.date) && x.date.ToDateTime().DayOfWeek == 0
                                && x.date.ToDateTime() < endDate && x.date.ToDateTime() >= startDate);
                                if (!isExistSundayinWorkingDays)
                                {
                                    continue;
                                }
                                else
                                {
                                    bool isExistSunDayPRD = mappings.Any(x => !String.IsNullOrEmpty(x.date) && x.date.ToDateTime() < endDate
                                    && x.date.ToDateTime() >= startDate && x.value == "PRD");
                                    if (isExistSunDayPRD)
                                    {
                                        continue;
                                    }
                                }
                            }
                            sapCodeInvalid.Add(item.SAPCode);
                        }
                    }

                }
            }
            foreach (var error in sapCodeInvalid)
            {
                result.ErrorCodes.Add(9);
                result.Messages.Add(error);
            }
            return result;
        }
        //=====jira - 593===========================
        private async Task<ResultDTO> ValidateStartDate(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO result)
        {
            var errors = new List<ValidateHalfdayModel>();
            var sapCodeInvalid = new List<string>();
            if (targetPlanDetails.Any())
            {
                var sapCodes = targetPlanDetails.Where(i => i.Type == TypeTargetPlan.Target1).Select(s => s.SAPCode);
                foreach (var cSapCode in sapCodes)
                {
                    var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == cSapCode && x.IsActivated);
                    if (currentUser.StartDate is null)
                    {
                        sapCodeInvalid.Add(currentUser.SAPCode);
                    }
                }
            }
            foreach (var error in sapCodeInvalid)
            {
                result.ErrorCodes.Add(10);
                result.Messages.Add(error);
            }
            return result;
        }
        //==========================================
        //=====jira 253===========================
        private async Task<ResultDTO> ValidateShiftCodeHQ(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO result)
        {
            var errors = new List<ValidateHalfdayModel>();
            var sapCodeInvalid = new List<string>();
            //List<string> shiftCodeStartWith_HQ = new List<string>() { "V8", "V9" };
            if (targetPlanDetails.Any())
            {
                var target1 = targetPlanDetails.Where(i => i.Type == TypeTargetPlan.Target1);
                foreach (var item in target1)
                {
                    var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == item.SAPCode && x.IsActivated);
                    if (currentUser.IsStore != null && currentUser.IsStore == false)// HQ
                    {
                        var lstTarget1 = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                        var startWithShiftCodeV = lstTarget1.Where(i => i.Value.StartsWith("V") && !i.Value.StartsWith("V8") && !i.Value.StartsWith("V9")).Select(s => new 
                        {
                            SAPCode = item.SAPCode,
                            Field = s.Date,
                            Value = s.Value
                        }).ToList();

                        if (startWithShiftCodeV.Any())
                        {
                            sapCodeInvalid.Add(currentUser.SAPCode);
                        }   
                    }
                }

            }
            foreach (var error in sapCodeInvalid)
            {
                result.ErrorCodes.Add(11);
                result.Messages.Add(error);
            }
            return result;
        }
        //==========================================
        private ResultDTO ValidateHalfDay(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO result)
        {
            var notFirstShiftV = new List<ValidateHalfdayModel>();
            var errors = new List<ValidateHalfdayModel>();
            if (targetPlanDetails.Any())
            {
                var target1 = targetPlanDetails.Where(i => i.Type == TypeTargetPlan.Target1);
                foreach (var item in target1)
                {
                    var lstTarget1 = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                    var notShiftV = lstTarget1.Where(i => !i.Value.StartsWith("V")).Select(s => new ValidateHalfdayModel
                    {
                        SAPCode = item.SAPCode,
                        Field = s.Date
                    }).ToList();
                    if (notShiftV.Any())
                        notFirstShiftV.AddRange(notShiftV);
                }
                if (notFirstShiftV.Any())
                {
                    var target2 = targetPlanDetails.Where(i => i.Type == TypeTargetPlan.Target2);
                    foreach (var item in target2)
                    {
                        var lstTarget2 = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                        var lstErrors = lstTarget2.Where(i => notFirstShiftV.Any(x => item.SAPCode == x.SAPCode && x.Field == i.Date && !String.IsNullOrEmpty(i.Value))).ToList();
                        if (lstErrors.Any())
                        {
                            var parseError = lstErrors.Select(s => new ValidateHalfdayModel { SAPCode = item.SAPCode, Field = s.Date.ToDateTime().ToString("dd/MM/yyyy") }).ToList();
                            if (parseError.Any())
                                errors.AddRange(parseError);
                        }
                    }
                }
            }
            foreach (var error in errors)
            {
                result.ErrorCodes.Add(7);
                result.Messages.Add($"{error.SAPCode}({error.Field})");
            }
            return result;
        }
        private ResultDTO ValidateImportData(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO result)
        {

            var groupBySapCode = targetPlanDetails.GroupBy(item => item.SAPCode).Select(s => new { item = s.Key, total = s.Count() });
            var groupBySapCodeAndType = targetPlanDetails.GroupBy(item => new { item.SAPCode, item.Type }).Select(s => new { item = s.Key, total = s.Count() });
            var duplicates = groupBySapCode.Where(x => x.total >= 3);
            foreach (var dulicate in duplicates)
            {
                // Duplicate dữ liệu
                result.ErrorCodes.Add(6);
                result.Messages.Add(dulicate.item);
            }
            var lacks = groupBySapCode.Where(x => x.total == 1);
            if (lacks != null && lacks.Any())
            {
                foreach (var lack in lacks)
                {
                    // thiếu target 1 or 2
                    var currentType = targetPlanDetails.FirstOrDefault(x => x.SAPCode == lack.item);
                    if (currentType != null)
                    {
                        var lackType = currentType.Type == TypeTargetPlan.Target1 ? TypeTargetPlan.Target2 : TypeTargetPlan.Target1;
                        result.ErrorCodes.Add(4);
                        result.Messages.Add($"{lack.item}|{lackType}"); // spilit "|" [0]: controller, [1]: message
                    }
                }
            }

            var dupplicateTypes = groupBySapCodeAndType.Where(x => x.total > 1);
            if (dupplicateTypes != null && dupplicateTypes.Any())
            {
                foreach (var lack in dupplicateTypes)
                {
                    // thiếu target 1 or 2
                    var currentType = targetPlanDetails.FirstOrDefault(x => x.SAPCode == lack.item.SAPCode);
                    if (currentType != null)
                    {
                        result.ErrorCodes.Add(6);
                        result.Messages.Add($"{lack.item.SAPCode}|{currentType.Type}"); // spilit "|" [0]: controller, [1]: message
                    }
                }
            }
            return result;
        }
        private async Task<ResultDTO> ValidateTargetPlanByJsonFile(List<TargetPlanArgDetail> targetPlanDetails, ResultDTO res, Guid periodId)
        {
            if (targetPlanDetails.Any())
            {
                /*var jsonContent = JsonHelper.GetJsonContentFromFile("Mappings", "target-plan-valid.json");
                var mappings = JsonConvert.DeserializeObject<List<TargetPlanValidMapping>>(jsonContent);*/
                var shiftCode = await _uow.GetRepository<ShiftCode>().FindByAsync<ShiftCodeViewModel>(x => x.IsActive && !x.IsDeleted && !x.Code.Contains("*"));
                var mappings = shiftCode.ToList();
                var holidays = new List<DateValueArgs>();
                foreach (var item in targetPlanDetails)
                {
                    var des = JsonConvert.DeserializeObject<List<DateValueArgs>>(item.JsonData);
                    if (item.Type == TypeTargetPlan.Target1)
                    {
                        holidays = await GetHolidaysInYear(des, periodId);
                    }

                    var shiftCodeDefine = mappings.Where(x => x.Line == (ShiftLine) item.Type).Select(y => y.Code).ToList();
                    var errors = des.Where(x => !String.IsNullOrEmpty(x.Value) && !holidays.Select(s => s.Date).Contains(x.Date) && !shiftCodeDefine.Contains(x.Value));

                    foreach (var error in errors)
                    {
                        res.ErrorCodes.Add(5);
                        res.Messages.Add($"{item.SAPCode}({error.Date.ToDateTime().ToString("dd/MM/yyyy")})|{item.Type}");
                    }
                }
            }
            return res;
        }
        private async Task<bool> CheckEmptyTarget_Cancelled(List<TargetPlanFromImportDetailItemDTO> targets, string sapCode, DateTimeOffset PeriodFrom, DateTimeOffset PeriodTo)
        {
            PeriodFrom = PeriodFrom.Date;
            PeriodTo = PeriodTo.Date;
            var validRange = new List<string>();
            var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == sapCode && x.IsActivated);
            if (currentUser != null)
            {
                var resignationByUser = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.UserSAPCode == currentUser.SAPCode && x.IsCancelResignation != true && x.Status.Contains("Cancelled"), "Created desc");
                if (resignationByUser != null)
                {
                    currentUser.OfficialResignationDate = resignationByUser.OfficialResignationDate;
                }
                var startDate = currentUser.StartDate.Value.Date;

                var resignationDate = currentUser.OfficialResignationDate;
                if (!resignationDate.HasValue)
                {
                    resignationDate = new DateTimeOffset(new DateTime(9999, 12, 31));
                }
                else
                {
                    resignationDate = resignationDate.Value.Date;
                }
                if (startDate >= PeriodFrom && resignationDate <= PeriodTo)
                {
                    while (startDate >= PeriodFrom && startDate < resignationDate)
                    {
                        validRange.Add(startDate.ToSAPFormat());
                        startDate = startDate.AddDays(1);
                    }
                }
                else if (startDate <= PeriodFrom && resignationDate <= PeriodTo)
                {
                    var temp = PeriodFrom;
                    while (temp >= PeriodFrom && temp < resignationDate)
                    {
                        validRange.Add(temp.DateTime.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
                else if (startDate <= PeriodFrom && resignationDate >= PeriodTo)
                {
                    var temp = PeriodFrom;
                    while (temp >= PeriodFrom && temp <= PeriodTo)
                    {
                        validRange.Add(temp.DateTime.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
                else if (startDate >= PeriodFrom && resignationDate >= PeriodTo)
                {
                    var temp = startDate;
                    var endTemp = PeriodTo;
                    while (temp >= PeriodFrom && temp <= endTemp)
                    {
                        validRange.Add(temp.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
            }
            return targets.Any(x => string.IsNullOrEmpty(x.value) && validRange.Contains(x.date));
        }
        private async Task<bool> CheckEmptyTarget(List<TargetPlanFromImportDetailItemDTO> targets, string sapCode, DateTimeOffset PeriodFrom, DateTimeOffset PeriodTo)
        {
            PeriodFrom = PeriodFrom.Date;
            PeriodTo = PeriodTo.Date;
            var validRange = new List<string>();
            var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == sapCode && x.IsActivated);
            if (currentUser != null)
            {
                var resignationByUser = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync<ResignationApplicationViewModel>(x => x.UserSAPCode == currentUser.SAPCode && x.IsCancelResignation != true && x.Status.Contains("Completed"), "Created desc");
                if (resignationByUser != null)
                {
                    currentUser.OfficialResignationDate = resignationByUser.OfficialResignationDate;
                }
                var startDate = currentUser.StartDate.Value.Date;
                var resignationDate = currentUser.OfficialResignationDate;
                if (!resignationDate.HasValue)
                {
                    resignationDate = new DateTimeOffset(new DateTime(9999, 12, 31));
                }
                else
                {
                    resignationDate = resignationDate.Value.Date;
                    if (resignationDate < PeriodFrom) //Khiem fix when user has resignation and back to work
                    {
                        resignationDate = new DateTimeOffset(new DateTime(9999, 12, 31));
                    }
                }
                if (startDate >= PeriodFrom && resignationDate <= PeriodTo)
                {
                    while (startDate >= PeriodFrom && startDate < resignationDate)
                    {
                        validRange.Add(startDate.ToSAPFormat());
                        startDate = startDate.AddDays(1);
                    }
                }
                else if (startDate <= PeriodFrom && resignationDate <= PeriodTo)
                {
                    var temp = PeriodFrom;
                    while (temp >= PeriodFrom && temp < resignationDate)
                    {
                        validRange.Add(temp.DateTime.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
                else if (startDate <= PeriodFrom && resignationDate >= PeriodTo)
                {
                    var temp = PeriodFrom;
                    while (temp >= PeriodFrom && temp <= PeriodTo)
                    {
                        validRange.Add(temp.DateTime.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
                else if (startDate >= PeriodFrom && resignationDate >= PeriodTo)
                {
                    var temp = startDate;
                    var endTemp = PeriodTo;
                    while (temp >= PeriodFrom && temp <= endTemp)
                    {
                        validRange.Add(temp.ToSAPFormat());
                        temp = temp.AddDays(1);
                    }
                }
            }
            return targets.Any(x => string.IsNullOrEmpty(x.value) && validRange.Contains(x.date));
        }
        public TargetPlanFromImportFileDTO ReadDataFromStream(Stream stream, IEnumerable<UserForTreeViewModel> users)
        {
            TargetPlanFromImportFileDTO result = new TargetPlanFromImportFileDTO();
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorkbook WorkBook = pck.Workbook;
                ExcelWorksheet workSheet = WorkBook.Worksheets.ElementAt(0);

                var stringFromDate = workSheet.Cells["C2"].Value.ToString();
                var stringToDate = workSheet.Cells["E2"].Value.ToString();

                var isNumFromDate = double.TryParse(stringFromDate, out double fromDateNum);
                var isNumToDate = double.TryParse(stringToDate, out double toDateNum);

                var periodFromDate = new DateTime();
                var periodToDate = new DateTime();
                if (isNumFromDate)
                {
                    periodFromDate = DateTime.FromOADate(fromDateNum);
                }
                else
                {
                    periodFromDate = DateTime.ParseExact(stringFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (isNumToDate)
                {
                    periodToDate = DateTime.FromOADate(toDateNum);
                }
                else
                {
                    periodToDate = DateTime.ParseExact(stringToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                }

                result.PeriodFromDate = periodFromDate;
                result.PeriodToDate = periodToDate;
                var numberDaysInPeriod = (periodToDate - periodFromDate).Days;
                var listData = new List<TargetPlanFromImportDetailDTO>();
                for (var rowNumber = 6; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    TargetPlanFromImportDetailDTO data = new TargetPlanFromImportDetailDTO();
                    data.SAPCode = workSheet.Cells[rowNumber, 2, rowNumber, workSheet.Dimension.End.Column][$"B{rowNumber}"].Text.ToSAPFormatString();
                    data.FullName = workSheet.Cells[rowNumber, 3, rowNumber, workSheet.Dimension.End.Column][$"C{rowNumber}"].Text;
                    data.Grade = workSheet.Cells[rowNumber, 4, rowNumber, workSheet.Dimension.End.Column][$"D{rowNumber}"].Text;
                    data.Target = workSheet.Cells[rowNumber, 5, rowNumber, workSheet.Dimension.End.Column][$"E{rowNumber}"].Text;
                    var currentUser = users.FirstOrDefault(x => x.SAPCode == data.SAPCode);
                    if (currentUser != null)
                    {
                        var officialResignationDate = new DateTime(3000, 10, 10);
                        var resignation = _uow.GetRepository<ResignationApplication>(true).GetSingle<ResignationApplicationViewModel>(i => i.UserSAPCode == currentUser.SAPCode && i.Status.Contains("Completed") && i.IsCancelResignation != true, "Created DESC");
                        if (!currentUser.StartDate.HasValue)
                        {
                            currentUser.StartDate = new DateTime(1970, 10, 10);
                        }
                        if (resignation != null && resignation.OfficialResignationDate.Date >= currentUser.StartDate.Value.Date)
                        {
                            officialResignationDate = resignation.OfficialResignationDate.ToLocalTime().Date;
                        }
                        var startDate = periodFromDate;
                        if (officialResignationDate.Date > startDate.Date || officialResignationDate.Date >= result.PeriodToDate.Value.Date)
                        {
                            for (var colNumber = 6; colNumber <= (numberDaysInPeriod + 6); colNumber++)
                            {
                                var cell = workSheet.Cells[rowNumber, colNumber];
                                var targetItem = new TargetPlanFromImportDetailItemDTO
                                {
                                    date = startDate.ToSAPFormat(),
                                    value = startDate >= currentUser.StartDate.Value && startDate < officialResignationDate ? cell.Text : string.Empty
                                };
                                startDate = startDate.AddDays(1);
                                data.Targets.Add(targetItem);
                            }
                            listData.Add(data);
                        }

                    }
                }
                result.Data = listData;
            }
            return result;
        }
        public async Task<ResultDTO> GetPermissionInfoOnPendingTargetPlan(TargetPlanQueryPermissionInfo arg)
        {
            var res = new ResultDTO();
            var currentUser = await _uow.GetRepository<User>().FindByIdAsync<UserForTreeViewModel>(_uow.UserContext.CurrentUserId);
            var jobGrade45 = await _uow.GetRepository<JobGrade>().GetAllAsync();
            var currentJobGrade = jobGrade45.FirstOrDefault(x => x.Id == currentUser.JobGradeId);
            
            var permissionInfoPendingTargetPlan = new PermissionInfoOnPendingTargetPlan();
            if (currentUser != null)
            {
                // if (currentUser.IsStore.Value && currentUser.JobGradeValue >= 4 || !currentUser.IsStore.Value && currentUser.JobGradeValue >= 5)
                if (currentUser.IsStore.Value && currentJobGrade.StorePosition == StorePositionType.MNG_STORE || !currentUser.IsStore.Value && currentJobGrade.HQPosition == HQPositionType.MNG_HQ)
                {
                    permissionInfoPendingTargetPlan.AllowToSubmit = true;
                    var validUsersForSubmit = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.SAPCode == currentUser.SAPCode && x.PeriodId == arg.PeriodId);
                    if (validUsersForSubmit.Any() && !validUsersForSubmit.Any(x => !x.IsSubmitted))
                    {
                        permissionInfoPendingTargetPlan.AllowToSubmit = false;
                    }
                }
                //arg.SAPCodes = await GetFullUsersForTargetPlan(new UserForTargetArg { ActiveUsers = arg.ActiveUsers, Ids = new Guid[] { arg.DepartmentId }, PeriodId = arg.PeriodId });    
                //var departmentId = arg.DivisionId.HasValue ? arg.DivisionId.Value : arg.DepartmentId;
                var departmentIds = arg.DivisionIds.Length > 0 ? arg.DivisionIds : new Guid[] { arg.DepartmentId };
                //var testDepartmentIds = new Guid[] { new Guid("D7FB9912-D108-41A4-9749-63D9140C02C9"), new Guid("DB60DB03-1015-4D4A-8238-6FB895F46C79"), new Guid("FBA114A0-AAA5-4CD7-ADF4-EDC04A17D661") };
                //var testDepartmentIds = new Guid[] { new Guid("2A33C2C6-A483-4087-A105-259FA477513C") };
                var checkedHeadcountDepartmentMappings = await _uow.GetRepository<UserDepartmentMapping>().FindByAsync<UserDepartmentMappingViewModel>(x => x.IsHeadCount && x.UserId == currentUser.Id && departmentIds.Contains(x.DepartmentId.Value));
                var setSubmitterDepartmentMappings = await _uow.GetRepository<UserSubmitPersonDeparmentMapping>().FindByAsync<UserSubmitPersonDepartmentMappingViewModel>(x => x.IsSubmitPerson && x.UserId == currentUser.Id && departmentIds.Contains(x.DepartmentId));
                IEnumerable<Guid> checkedHeadcountDepartmentIds = null;
                IEnumerable<Guid> setSubmitterDepartmentIds = null;
                if (checkedHeadcountDepartmentMappings.Any())
                {
                    checkedHeadcountDepartmentIds = checkedHeadcountDepartmentMappings.Select(x => x.DepartmentId.Value);
                }
                if (setSubmitterDepartmentMappings.Any())
                {
                    setSubmitterDepartmentIds = setSubmitterDepartmentMappings.Select(x => x.DepartmentId);
                }
                if (setSubmitterDepartmentIds != null)
                {
                    //permissionInfoPendingTargetPlan.AllowToSendData = false;
                    permissionInfoPendingTargetPlan.AllowToSubmit = true;
                    if (arg.ValidUsersSubmit.Count == 0)
                    {
                        permissionInfoPendingTargetPlan.AllowToSubmit = false;
                    }
                    else
                    {
                        var validUsersForSubmit = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => arg.ValidUsersSubmit.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                        if (validUsersForSubmit.Any() && !validUsersForSubmit.Any(x => !x.IsSubmitted))
                        {
                            permissionInfoPendingTargetPlan.AllowToSubmit = false;
                        }
                    }
                }
                if (checkedHeadcountDepartmentIds != null)
                {
                    permissionInfoPendingTargetPlan.AllowToSendData = true;
                    if (arg.ActiveUsers.Count == 0)
                    {
                        permissionInfoPendingTargetPlan.AllowToSendData = false;
                    }
                    else
                    {
                        var validUsersForActive = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => arg.ActiveUsers.Contains(x.SAPCode) && x.PeriodId == arg.PeriodId);
                        if (validUsersForActive.Any() && !validUsersForActive.Any(x => !x.IsSent))
                        {
                            permissionInfoPendingTargetPlan.AllowToSendData = false;
                        }
                    }

                }
            }
            res.Object = permissionInfoPendingTargetPlan;
            return res;
        }
        private async Task<List<string>> GetFullUsersForTargetPlan(UserForTargetArg arg)
        {
            var res = new List<UserForTreeViewModel>();
            var allUserData = await _settingBO.GetUsersForTargetPlanByDeptId(arg);
            if (allUserData.Count > 0)
            {
                res = (List<UserForTreeViewModel>)allUserData.Data;
            }
            return res.Select(x => x.SAPCode).ToList();
        }
        public async Task<ResultDTO> CancelPendingTargetPLan(CancelPendingTargetArg args)
        {
            IEnumerable<PendingTargetPlanDetail> targetPlans = null;
            var result = new ResultDTO { };
            //var targetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.PendingTargetPlanId == Id);
            //get G1
            var g1 = await _uow.GetRepository<JobGrade>().GetSingleAsync(x => x.Grade == 1);
            var isHeadCount = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(i => i.User.SAPCode == args.SAPCode && i.IsHeadCount);
            var listDeptG1 = (await _uow.GetRepository<Department>().FindByAsync<DepartmentViewModel>(x => x.ParentId == args.DeptId && x.JobGradeId == g1.Id && x.IsStore)).Select(s => s.Code);
            if (isHeadCount && listDeptG1.Any())
            {
                targetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => (listDeptG1.Contains(x.DepartmentCode) || x.SAPCode == args.SAPCode) && x.PendingTargetPlanId == args.PendingTargetId && x.PeriodId == args.PeriodId);
            }
            else
            {
                targetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => x.SAPCode == args.SAPCode && x.PeriodId == args.PeriodId);
            }

            if (targetPlans != null && targetPlans.Any())
            {
                foreach (var item in targetPlans)
                {
                    item.IsSent = false;
                }
                await _uow.CommitAsync();
            }
            return result;
        }
        public async Task<ResultDTO> RequestToChange(TargetPlanRequestToChangeArg args)
        {
            var sapCodes = args.TargetPlanDetails.Select(x => x.SAPCode);
            var result = new ResultDTO { };
            var pendingTargetDetails = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => sapCodes.Contains(x.SAPCode) && x.PeriodId == args.PeriodId && (x.IsSent || x.IsSubmitted));
            if (pendingTargetDetails != null && pendingTargetDetails.Any())
            {
                foreach (var item in pendingTargetDetails)
                {
                    var currentSelectedRow = args.TargetPlanDetails.FirstOrDefault(x => x.SAPCode == item.SAPCode && x.Type == item.Type);
                    if (currentSelectedRow != null)
                    {
                        item.IsSent = false;
                        item.IsSubmitted = false;
                        item.TargetPlanDetailId = currentSelectedRow.TargetPlanDetailId;
                        item.Comment = currentSelectedRow.Comment;
                    }
                }
                var currentTargetPlanDetail = args.TargetPlanDetails.FirstOrDefault();
                if (currentTargetPlanDetail != null)
                {
                    var currentTargetPlan = await _uow.GetRepository<TargetPlanDetail>().FindByIdAsync<TargetPlanDetailViewModel>(currentTargetPlanDetail.TargetPlanDetailId);
                    if (currentTargetPlan != null)
                    {
                        var currentPermission = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == currentTargetPlan.Id && x.UserId == currentTargetPlan.CreatedById);
                        if (currentPermission != null && currentPermission.Any())
                        {
                            foreach (var perm in currentPermission)
                            {
                                perm.Perm = Right.View;
                            }
                        }
                    }
                }
                await _uow.CommitAsync();
            }
            else
            {
                result.ErrorCodes = new List<int> { 1004 };
                result.Messages = new List<string> { "No Data" };
            }
            return result;
        }

        public async Task<byte[]> DownloadTemplate(DataToPrintTemplateArgs args)
        {
            var dataToPrint = new TargetPlanPrintFormViewModel();
            var items = new List<TargetPlanDetailPrintFormViewModel>();

            byte[] result = null;

            foreach (var sapCode in args.ListSAPCodes)
            {
                var user = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(x => x.SAPCode == sapCode);
                var target1 = new TargetPlanDetailPrintFormViewModel()
                {
                    DepartmentName = user.DepartmentName,
                    FullName = user.FullName,
                    SAPCode = user.SAPCode,
                    Type = TypeTargetPlan.Target1,
                    GradeCaption = user.JobGrade
                };

                var target2 = new TargetPlanDetailPrintFormViewModel()
                {
                    DepartmentName = user.DepartmentName,
                    FullName = user.FullName,
                    SAPCode = user.SAPCode,
                    Type = TypeTargetPlan.Target2,
                    GradeCaption = user.JobGrade
                };
                items.Add(target1);
                items.Add(target2);

            }

            if (dataToPrint != null)
            {
                try
                {

                    var department = await _uow.GetRepository<Department>().FindByIdAsync(args.DepartmentId);
                    if (args.DivisionId.HasValue)
                    {
                        var division = await _uow.GetRepository<Department>().FindByIdAsync(args.DivisionId.Value);
                        int divisionNameTo = division.Name.IndexOf("(");
                        dataToPrint.DivisionName = division.Name.Substring(0, divisionNameTo - 1);
                    }
                    int departmentNameTo = department.Name.IndexOf("(");
                    dataToPrint.DeptName = department.Name.Substring(0, departmentNameTo - 1);
                    dataToPrint.PeriodFromDate = args.PeriodFromDate;
                    var properties = typeof(TargetPlanPrintFormViewModel).GetProperties();
                    var pros = new Dictionary<string, string>();
                    foreach (var property in properties)
                    {
                        var value = Convert.ToString(property.GetValue(dataToPrint));
                        pros[property.Name] = SecurityElement.Escape(value);
                    }
                    var periodFromDate = DateTime.ParseExact(args.PeriodFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (periodFromDate != null)
                    {
                        _logger.LogInformation("Parse Period Fromn String From Client");
                        if (periodFromDate.Month == 2)
                        {
                            int year = periodFromDate.Year;
                            if (year % 4 == 0 && year % 100 != 0)
                            {
                                result = ExportXLS("TargetPlan2902.xlsx", pros, items);
                            }
                            else
                            {
                                result = ExportXLS("TargetPlan2802.xlsx", pros, items);
                            }
                        }
                        else
                        {
                            if (periodFromDate.Month == 4 || periodFromDate.Month == 6 || periodFromDate.Month == 9 || periodFromDate.Month == 11)
                            {
                                result = ExportXLS("TargetPlan30.xlsx", pros, items);
                            }
                            else
                            {
                                result = ExportXLS("TargetPlan.xlsx", pros, items);
                            }
                        }
                    }
                    else
                    {
                        result = ExportXLS("TargetPlan.xlsx", pros, items);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Download Template {0}", string.IsNullOrEmpty(ex.Message) ? ex.InnerException.Message : ex.Message);
                }


            }
            return result;
        }


        public async Task<byte[]> PrintForm(TargetPlanDetailQueryArg args)
        {
            var dataToPrint = new TargetPlanPrintFormViewModel();
            var items = new List<TargetPlanDetailPrintFormViewModel>();
            List<TargetPlanArgDetail> targetPlanDetails = (await _uow.GetRepository<TargetPlanDetail>().FindByAsync<TargetPlanArgDetail>
                       (x => args.SAPCodes.Contains(x.SAPCode) && x.TargetPlan.PeriodId == args.PeriodId && (x.TargetPlan.Status == "Completed" || x.TargetPlan.Status == "Approved"))).ToList();
            if (targetPlanDetails.Count > 2)
            {
                var groups = targetPlanDetails.GroupBy(x => new { x.ReferenceNumber, x.SAPCode }).OrderByDescending(y => y.Key.ReferenceNumber);
                var listData = new List<TargetPlanArgDetail>();
                foreach (var group in groups)
                {
                    if (!listData.Any(x => x.SAPCode == group.Key.SAPCode && !group.All(y => x.Type == y.Type)))
                    {
                        listData.AddRange(group.ToList());
                    }
                }
                items = Mapper.Map<List<TargetPlanDetailPrintFormViewModel>>(listData);
            }
            else
            {
                items = Mapper.Map<List<TargetPlanDetailPrintFormViewModel>>(targetPlanDetails);
            }

            byte[] result = null;


            if (dataToPrint != null)
            {
                var department = await _uow.GetRepository<Department>().FindByIdAsync(args.DepartmentId);
                if (args.DivisionId.HasValue)
                {
                    var division = await _uow.GetRepository<Department>().FindByIdAsync(args.DivisionId.Value);
                    int divisionNameTo = division.Name.IndexOf("(");
                    dataToPrint.DivisionName = division.Name.Substring(0, divisionNameTo - 1);
                }
                int departmentNameTo = department.Name.IndexOf("(");
                dataToPrint.DeptName = department.Name.Substring(0, departmentNameTo - 1);
                var printItems = new List<TargetPlanDetailPrintFormViewModel>();
                if (items.Any())
                {
                    try
                    {
                        var actualShiftPlanArg = new ActualTargetPlanArg();
                        actualShiftPlanArg.PeriodId = args.PeriodId;
                        actualShiftPlanArg.ListSAPCode = JsonConvert.SerializeObject(args.SAPCodes);
                        var actualShiftPlanRes = await GetActualShiftPlan(actualShiftPlanArg);
                        dynamic objShiftPlan = actualShiftPlanRes.Object;
                        var listActualShiftPlanJson = JsonConvert.SerializeObject(objShiftPlan.Data);
                        List<ActualShiftPlanPrintFormViewModel> listActualShiftPlan = JsonConvert.DeserializeObject<List<ActualShiftPlanPrintFormViewModel>>(listActualShiftPlanJson);
                        for (int i = 0; i < items.Count(); i += 2)
                        {
                            TargetPlanDetailPrintFormViewModel targetShiftPlan1 = new TargetPlanDetailPrintFormViewModel();
                            TargetPlanDetailPrintFormViewModel targetShiftPlan2 = new TargetPlanDetailPrintFormViewModel();
                            if (items.ElementAt(i).Type == TypeTargetPlan.Target1)
                            {
                                printItems.Add(items.ElementAt(i));
                                printItems.Add(items.ElementAt(i + 1));
                                targetShiftPlan1 = items.ElementAt(i);
                                targetShiftPlan2 = items.ElementAt(i + 1);
                            }
                            else
                            {
                                printItems.Add(items.ElementAt(i + 1));
                                printItems.Add(items.ElementAt(i));
                                targetShiftPlan1 = items.ElementAt(i + 1);
                                targetShiftPlan2 = items.ElementAt(i);
                            }

                            var actual = listActualShiftPlan.FirstOrDefault(x => x.SAPCode == items.ElementAt(i).SAPCode);

                            var actualShiftPlan1 = new TargetPlanDetailPrintFormViewModel()
                            {
                                DepartmentName = targetShiftPlan1.DepartmentName,
                                FullName = targetShiftPlan1.FullName,
                                SAPCode = targetShiftPlan1.SAPCode,
                                TargetPlanId = targetShiftPlan1.TargetPlanId,
                                Type = TypeTargetPlan.Actual1,
                                JsonData = targetShiftPlan1.JsonData,
                            };
                            var actualShiftPlan2 = new TargetPlanDetailPrintFormViewModel()
                            {
                                DepartmentName = targetShiftPlan2.DepartmentName,
                                FullName = targetShiftPlan2.FullName,
                                SAPCode = targetShiftPlan2.SAPCode,
                                TargetPlanId = targetShiftPlan2.TargetPlanId,
                                Type = TypeTargetPlan.Actual2,
                                JsonData = targetShiftPlan2.JsonData,
                            };
                            if (actual != null)
                            {
                                if (!string.IsNullOrEmpty(actual.Actual1))
                                {
                                    var actual1DataList = JsonConvert.DeserializeObject<List<DateValueArgs>>(actual.Actual1);
                                    var actualShiftPlan1DataList = JsonConvert.DeserializeObject<List<DateValueArgs>>(actualShiftPlan1.JsonData);
                                    for (int j = 0; j < actual1DataList.Count; j++)
                                    {
                                        for (int z = 0; z < actualShiftPlan1DataList.Count; z++)
                                        {
                                            if (actual1DataList.ElementAt(j).Date == actualShiftPlan1DataList.ElementAt(z).Date)
                                            {
                                                if (!string.IsNullOrEmpty(actual1DataList.ElementAt(j).Value))
                                                {
                                                    actualShiftPlan1DataList.ElementAt(z).Value = actual1DataList.ElementAt(j).Value;
                                                }
                                            }
                                        }
                                    }
                                    actualShiftPlan1.JsonData = JsonConvert.SerializeObject(actualShiftPlan1DataList);
                                }
                                if (!string.IsNullOrEmpty(actual.Actual2))
                                {
                                    var actual2DataList = JsonConvert.DeserializeObject<List<DateValueArgs>>(actual.Actual2);
                                    var actualShiftPlan2DataList = JsonConvert.DeserializeObject<List<DateValueArgs>>(actualShiftPlan2.JsonData);
                                    for (int j = 0; j < actual2DataList.Count; j++)
                                    {
                                        for (int z = 0; z < actualShiftPlan2DataList.Count; z++)
                                        {
                                            if (actual2DataList.ElementAt(j).Date == actualShiftPlan2DataList.ElementAt(z).Date)
                                            {
                                                if (!string.IsNullOrEmpty(actual2DataList.ElementAt(j).Value))
                                                {
                                                    actualShiftPlan2DataList.ElementAt(z).Value = actual2DataList.ElementAt(j).Value;
                                                }
                                            }
                                        }
                                    }
                                    actualShiftPlan2.JsonData = JsonConvert.SerializeObject(actualShiftPlan2DataList);
                                }
                            }

                            //actualShiftPlan1.JsonData = actual.Actual1;
                            //actualShiftPlan2.JsonData = actual.Actual2;

                            //items.Add(actualShiftPlan1);
                            //items.Add(actualShiftPlan2);
                            printItems.Add(actualShiftPlan1);
                            printItems.Add(actualShiftPlan2);

                            var firstShift = JsonConvert.DeserializeObject<List<DateValueArgs>>(items.ElementAt(0).JsonData);
                            var firstShiftDate = DateTimeOffset.ParseExact(firstShift.ElementAt(0).Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                            dataToPrint.PeriodFromDate = firstShiftDate;
                        }

                    }
                    catch (Exception e)
                    {

                    }
                }
                var properties = typeof(TargetPlanPrintFormViewModel).GetProperties();
                var pros = new Dictionary<string, string>();
                foreach (var property in properties)
                {
                    var value = Convert.ToString(property.GetValue(dataToPrint));
                    pros[property.Name] = SecurityElement.Escape(value);
                }

                TargetPlanPeriod targetPeriod = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.Id == args.PeriodId);
                if (targetPeriod != null)
                {
                    var periodFromDate = targetPeriod.FromDate;
                    if (periodFromDate != null)
                    {
                        if (periodFromDate.Month == 2)
                        {
                            int year = periodFromDate.Year;
                            if (year % 4 == 0 && year % 100 != 0)
                            {
                                result = ExportXLS("TargetPlan2902.xlsx", pros, printItems);
                            }
                            else
                            {
                                result = ExportXLS("TargetPlan2802.xlsx", pros, printItems);
                            }
                        }
                        else
                        {
                            if (periodFromDate.Month == 4 || periodFromDate.Month == 6 || periodFromDate.Month == 9 || periodFromDate.Month == 11)
                            {
                                result = ExportXLS("TargetPlan30.xlsx", pros, printItems);
                            }
                            else
                            {
                                result = ExportXLS("TargetPlan.xlsx", pros, printItems);
                            }
                        }
                    }
                    else
                    {
                        result = ExportXLS("TargetPlan.xlsx", pros, printItems);
                    }
                }
                else
                {
                    result = ExportXLS("TargetPlan.xlsx", pros, printItems);
                }
            }
            return result;
        }

        public byte[] ExportXLS(string template, Dictionary<string, string> pros, List<TargetPlanDetailPrintFormViewModel> tbTPDetail)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; // You get main rott
            var filePath = Path.Combine(path, "PrintDocument", template);
            var memoryStream = new MemoryStream();
            using (var stream = System.IO.File.OpenRead(filePath))
            {
                using (var pck = new ExcelPackage(stream))
                {
                    ExcelWorkbook WorkBook = pck.Workbook;
                    ExcelWorksheet worksheet = WorkBook.Worksheets.First();
                    //WorkBook.CalcMode = ExcelCalcMode.Manual;

                    InsertTargetData(worksheet, 6, tbTPDetail);
                    var regex = new Regex(@"\[\[[\d\w\s]*\]\]", RegexOptions.IgnoreCase);
                    var tokens = worksheet.Cells.Where(x => x.Value != null && regex.Match(x.Value.ToString()).Success);
                    foreach (var token in tokens)
                    {
                        var fieldToken = token.Value.ToString().Trim(new char[] { '[', ']' });
                        if (pros.ContainsKey(fieldToken))
                        {
                            token.Value = pros[fieldToken];
                        }
                    }
                    worksheet.Calculate();

                    pck.SaveAs(memoryStream);

                }
            }
            return memoryStream.ToArray();
        }

        private void InsertTargetData(ExcelWorksheet worksheet, int styleRow, List<TargetPlanDetailPrintFormViewModel> tblPros)
        {
            var index = 0;
            var fromRow = styleRow + 1;
            var toRow = fromRow + tblPros.Count;
            for (int i = fromRow; i < toRow; i++)
            {
                try
                {
                    var tpDetail = tblPros.ElementAt(index);
                    worksheet.InsertRow(i, 1, styleRow);
                    var row = worksheet.Row(i);
                    //row.Height = 26;
                    //int gradeFrom = tpDetail.DepartmentName.IndexOf("(") + 1;
                    //int gradeTo = tpDetail.DepartmentName.IndexOf(")");

                    var grade = tpDetail.GradeCaption;
                    // Style               
                    //worksheet.Cells[$"B{i}"].Style.WrapText = true;
                    //worksheet.Cells[$"B{i}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    //worksheet.Cells[$"B{i}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    //worksheet.Cells[$"B{i}"].AutoFitColumns();

                    string type = "";

                    switch (tpDetail.Type)
                    {
                        case TypeTargetPlan.Target1:
                            type = "1-Target";
                            break;
                        case TypeTargetPlan.Target2:
                            type = "2-Target";
                            break;
                        case TypeTargetPlan.Actual1:
                            type = "1-Actual";
                            break;
                        case TypeTargetPlan.Actual2:
                            type = "2-Actual";
                            break;
                    }

                    // Update Data
                    worksheet.Cells[$"B{i}"].Value = tpDetail.SAPCode;
                    worksheet.Cells[$"C{i}"].Value = tpDetail.FullName;
                    worksheet.Cells[$"D{i}"].Value = grade;
                    worksheet.Cells[$"E{i}"].Value = type;

                    if (tpDetail.JsonData != null)
                    {
                        var listShiftDate = JsonConvert.DeserializeObject<List<DateValueArgs>>(tpDetail.JsonData);
                        if (listShiftDate.Count > 0)
                        {
                            char cellChar1 = 'F';
                            int indexData = 0;
                            while (cellChar1 <= 'Z')
                            {
                                worksheet.Cells[$"{cellChar1}{i}"].Value = listShiftDate.ElementAt(indexData).Value;
                                cellChar1++;
                                indexData++;
                            }
                            cellChar1 = 'A';
                            char cellChar2 = 'A';
                            while (cellChar2 <= 'J')
                            {
                                if (indexData < listShiftDate.Count)
                                {
                                    worksheet.Cells[$"{cellChar1}{cellChar2}{i}"].Value = listShiftDate.ElementAt(indexData).Value;
                                }
                                cellChar2++;
                                indexData++;
                            }
                        }
                    }
                    index++;

                }
                catch (Exception ex)
                {

                }

            }
            worksheet.DeleteRow(styleRow);
        }
        public async Task<ResultDTO> GetPendingTargetPlans(QueryArgs arg)
        {
            var items = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(arg.Order, arg.Page, arg.Limit, arg.Predicate, arg.PredicateParameters);

            var count = await _uow.GetRepository<PendingTargetPlanDetail>().CountAsync(arg.Predicate, arg.PredicateParameters);
            return new ResultDTO
            {
                Object = new ArrayResultDTO
                {
                    Data = items,
                    Count = count
                }
            };
        }

        //export report target plan
        protected string Json => "export-mapping.json";
        protected string JsonGroupName => "ReportTargetPlan";
        public async Task<ResultDTO> TargetPlanReport(Guid departmentId, Guid periodId, int limit, int page, string searchText, bool isMade = false)
        {
            var fieldMappings = ReadConfigurationFromFile();
            var headers = fieldMappings.Select(y => y.DisplayName);
            // Create Headers
            DataTable tbl = new DataTable();
            foreach (var headerItem in headers)
            {
                tbl.Columns.Add(headerItem);
            }
            //Add Row
            limit = Int32.MaxValue;
            var resultIsMade = new List<PendingTargetPlanDetailViewModel>();
            var resultIsNotMade = new List<UserForTreeViewModel>();
            var dept = await _uow.GetRepository<Department>().FindByIdAsync(departmentId);
            var allDepts = await _uow.GetRepository<Department>().GetAllAsync();
            if (dept != null)
            {
                var departmentList = new List<Guid>();
                departmentList.Add(departmentId);
                ExpandAllNodes(departmentList, dept, allDepts);
                if (searchText == null)
                {
                    searchText = "";
                }
                var items = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())));
                var sapCodes = items.OrderByDescending(x => x.JobGradeValue).Select(x => x.SAPCode);
                if (sapCodes.Any())
                {
                    var pendingTargetPlans = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync<PendingTargetPlanDetailViewModel>(x => sapCodes.Contains(x.SAPCode) && x.PeriodId == periodId);

                    if (isMade)
                    {
                        pendingTargetPlans = pendingTargetPlans.Where(x => x.IsSent);
                        if (pendingTargetPlans.Any())
                        {
                            //code moi
                            var pendingGroupBySAPCode = pendingTargetPlans.GroupBy(x => x.SAPCode);
                            var data = pendingGroupBySAPCode.Skip((page - 1) * limit).Take(limit).SelectMany(x => x.ToList());
                            resultIsMade = data.ToList();
                        }
                    }
                    else
                    {
                        var hasMade = pendingTargetPlans.Where(x => x.IsSent).Select(x => x.SAPCode);
                        var notMade = pendingTargetPlans.Where(x => !x.IsSent).Select(x => x.SAPCode);
                        var notCreateTargets = sapCodes.Where(x => !hasMade.Contains(x) && !notMade.Contains(x));
                        sapCodes = notMade.Concat(notCreateTargets).Distinct();
                        //var cloneSAPCodes = sapCodes;
                        var currentUsersInPage = sapCodes.Skip((page - 1) * limit).Take(limit);
                        //var tempData = items.Where(x => currentUsersInPage.Contains(x.SAPCode)).ToList();
                        var tempData = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => x.UserDepartmentMappings.Any(y => departmentList.Contains(y.DepartmentId.Value)) && (x.SAPCode.Contains(searchText) || x.FullName.ToLower().Contains(searchText.ToLower())) && currentUsersInPage.Contains(x.SAPCode));
                        foreach (var item in tempData)
                        {
                            var existItem = pendingTargetPlans.FirstOrDefault(x => x.SAPCode == item.SAPCode);
                            if (existItem != null)
                            {
                                item.Created = existItem.Created;
                            }
                        }
                        resultIsNotMade = tempData.ToList();
                    }
                }
                if (resultIsMade.Any() || resultIsNotMade.Any())
                {
                    var exportReportTargetPlan = new List<ExportReportTargetPlan>();
                    if (resultIsMade.Any())
                    {
                        var arrayFilter = resultIsMade.GroupBy(x => x.SAPCode).Select(y => y.First()).ToList();
                        foreach (var record in arrayFilter)
                        {
                            exportReportTargetPlan.Add(new ExportReportTargetPlan
                            {
                                SAPCode = record.SAPCode,
                                FullName = record.FullName,
                                DepartmentName = record.DepartmentName,
                                PeriodName = record.PeriodName,
                                Created = record.Created.ToString("dd/MM/yyyy HH:mm:ss")
                            });
                        }
                    }
                    else if (resultIsNotMade.Any())
                    {
                        var arrayFilter = resultIsNotMade.GroupBy(x => x.SAPCode).Select(y => y.First()).ToList();
                        foreach (var record in resultIsNotMade)
                        {
                            exportReportTargetPlan.Add(new ExportReportTargetPlan
                            {
                                SAPCode = record.SAPCode,
                                FullName = record.FullName,
                                DepartmentName = record.DepartmentName,
                                PeriodName = record.PeriodName,
                                Created = record.Created.ToString("dd/MM/yyyy HH:mm:ss")
                            });
                        }
                    }

                    for (int rowNum = 0; rowNum < exportReportTargetPlan.Count(); rowNum++)
                    {
                        DataRow row = tbl.Rows.Add();
                        var data = exportReportTargetPlan.ElementAt(rowNum);
                        for (int j = 0; j < fieldMappings.Count; j++)
                        {
                            var fieldMapping = fieldMappings[j];
                            var value = data.GetType().GetProperty(fieldMapping.Name).GetValue(data);
                            HandleCommonType(row, value, j, fieldMapping);
                        }
                    }
                }
                else
                {
                    return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
                }
            }
            //
            var creatingExcelFileReslult = ExportExcel(tbl);
            if (creatingExcelFileReslult == null)
            {
                return new ResultDTO { ErrorCodes = { 1003 }, Messages = { "No Data" } };
            }
            return new ResultDTO { Object = creatingExcelFileReslult };
        }

        public async Task<ResultDTO> ValidateSubmitPendingTargetPlan(ValidateExistTargetPlanArgs args)
        {
            var result = new ResultDTO { Object = null };
            var statusToCheckes = new string[] { "Rejected", "Cancelled", "Requested To Change" };
            try
            {
                if (args.SapCodes.Any())
                {
                    // check period
                    var targetPlan = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync(x => x.TargetPlan != null && args.SapCodes.Contains(x.SAPCode) && x.TargetPlan.PeriodId == args.PeriodId && !statusToCheckes.Contains(x.TargetPlan.Status));
                    if (targetPlan.Any())
                    {
                        result = new ResultDTO { ErrorCodes = new List<int> { -1 }, Messages = new List<string> { "USER_IS_ALREADY_TARGET_PLAN" }, Object = string.Join(",", targetPlan.Select(x => x.TargetPlan.ReferenceNumber).ToList().Distinct()) };
                    }
                }
            } catch (Exception e)
            {
                _logger.LogError("Exception: " + e.Message);
            }
            return result;
        }

        protected void HandleCommonType(DataRow row, object value, int currentCell, MappingFieldToExport fieldMapping)
        {
            if (value != null)
            {
                row[currentCell] = value;
                if (fieldMapping.Type == FieldType.Date)
                {
                    row[currentCell] = DateTime.Parse(value.ToString()).ToLocalTime().ToString("dd/MM/yyyy");
                }
                else if (fieldMapping.Type == FieldType.Boolean)
                {
                    row[currentCell] = bool.Parse(value.ToString()) ? "Yes" : "No";
                }
                else if (fieldMapping.Type == FieldType.Enum)
                {
                    row[currentCell] = value.ToString();

                }
            }
        }

        private void ExpandAllNodes(List<Guid> deptList, Department parentNode, IEnumerable<Department> allNodes)
        {
            var childNodes = allNodes.Where(x => x.ParentId == parentNode.Id).ToList();
            if (childNodes.Count() > 0)
            {
                deptList.AddRange(childNodes.Select(x => x.Id).ToList());
                foreach (var childNode in childNodes)
                {
                    ExpandAllNodes(deptList, childNode, allNodes);
                }
            }
        }

        protected List<MappingFieldToExport> ReadConfigurationFromFile()
        {
            List<MappingFieldToExport> result = null;
            var fileName = Json;
            var fileContent = JsonHelper.GetJsonContentFromFile("Mappings", fileName);
            if (!string.IsNullOrEmpty(fileContent))
            {
                var jsonGroup = JsonHelper.GetGroupDataByName(fileContent, JsonGroupName);
                if (!string.IsNullOrEmpty(jsonGroup))
                {
                    result = JsonConvert.DeserializeObject<List<MappingFieldToExport>>(jsonGroup);
                }
            }
            return result.Where(x => x.Visible).ToList();
        }

        protected byte[] ExportExcel(DataTable tbl)
        {
            if (tbl.Rows.Count > 0)
            {
                using (var mStr = new MemoryStream())
                {
                    using (var pck = new ExcelPackage(mStr))
                    {
                        var ws = pck.Workbook.Worksheets.Add(JsonGroupName);
                        ws.Cells["A1"].LoadFromDataTable(tbl, true);
                        ws.Cells.AutoFitColumns();
                        StyleHeader(ws);
                        pck.Save();
                    }
                    return mStr.ToArray();
                }
            }
            return null;
        }

        private void StyleHeader(ExcelWorksheet pck)
        {
            var headerCells = pck.Cells[1, 1, 1, pck.Dimension.End.Column];
            var headerFont = headerCells.Style.Font;
            headerFont.SetFromFont(new Font("Times New Roman", 12));
            headerFont.Bold = true;
        }
        #region specical casse cr 9.9
        private async Task<bool> CheckSpecialCase(Guid Id)// kiem tra user co phai case dac biet khong 
        {
            var getListDepartment = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(i => i.UserId == Id &&i.IsHeadCount);// loc ra cac dept  co user la head count nen khi xuong duoi  khong can ktra dk headcount

            if (getListDepartment == null)// khong la headcount cua phong ban nao
            {
                return false;
            }
            else // truong hop co list department
            {
               
                    if (getListDepartment.Id != null)
                    {
                        try
                        {
                            var targetPlanSpecial = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == getListDepartment.DepartmentId); //B1
                            if (targetPlanSpecial != null)// có tồn tại trong bảng special và là headcount
                            {
                                return true;
                            }
                            //nếu không có trong bảng specical thì đệ quy tìm thằng cha để ktra thằng cha có nằm trong bảng specical  && tick children 
                            var departmentcurrent = await _uow.GetRepository<Department>().FindByAsync(x => x.Id == getListDepartment.DepartmentId);
                            if (departmentcurrent != null)
                            {
                                var isSpecical = await RecursionDepartment(departmentcurrent.First());// nếu không thỏa điều kiện nào thì làm b2 là tìm lên cha
                                if (isSpecical == true)
                                {
                                    return true;
                                }
                            }
                        } catch (Exception e)
                        {

                        }
                }
            }
            return false;
        }
        private async Task<bool> RecursionDepartment(Department department)// kiem tra user co phai case dac biet khong  department hien tai la department dau tien hoac cha
        {
            bool isspecial = false;
            var fatherDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id== department.ParentId);// tìm ra nó  tức là nó có cha nếu không tìm ra cha thì nó đã là ông nội 
            if(fatherDepartment != null) // nếu có cha thì sẽ ktra cha trong specical xem có tick include children 
            {
                var fatherIncludeChildren = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == fatherDepartment.Id); 
                if(fatherIncludeChildren!= null && fatherIncludeChildren.IsIncludeChildren == true)
                {
                    return true;
                }
                else if (fatherIncludeChildren != null && fatherIncludeChildren.IsIncludeChildren == false)
                {
                    return false;
                }
                else
                {
                    isspecial= await RecursionDepartment(fatherDepartment);
                }

            }
            return isspecial;
           
        }
        #endregion

        #region DWS Create Target
        public async Task<ResultDTO> ValidateForEditDWS_API(CreateTargetPlan_APIArgs agrs)
        {
            var result = new ResultDTO();
            var newLog = new TrackingAPIIntegrationLog()
            {
                Module = "DWS",
                APIName = "ValidateForEditDWS_API",
                Payload = JsonConvert.SerializeObject(agrs),
            };
            _uow.GetRepository<TrackingAPIIntegrationLog>().Add(newLog);
            await _uow.CommitAsync();

            if (agrs == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Cannot find any param!" } };
                goto Finish;
            }

            if (agrs.RequestorId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "RequestorId is required!" } };
                goto Finish;
            }
            var findRequestor = _uow.GetRepository<User>().GetSingle(x => x.Id == agrs.RequestorId);
            if (findRequestor == null || (findRequestor != null && !findRequestor.IsActivated))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Requestor is not exist!" } };
                goto Finish;
            }
            _uow.UserContext.CurrentUserId = findRequestor.Id;
            _uow.UserContext.CurrentUserName = findRequestor.LoginName;
            _uow.UserContext.CurrentUserFullName = findRequestor.FullName;

            if (agrs.DeptLineId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DeptLineId is required!" } };
                goto Finish;
            }
            var findDeptLine = _uow.GetRepository<Department>().GetSingle(x => x.Id == agrs.DeptLineId);
            if (findDeptLine == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DeptLineId is not exist!" } };
                goto Finish;
            }

            if (agrs.DivisionId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DivisionId is required!" } };
                goto Finish;
            }
            var findDivision = _uow.GetRepository<Department>().GetSingle(x => x.Id == agrs.DivisionId);
            if (findDivision == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Division is not exist!" } };
                goto Finish;
            }

            if (agrs.PeriodId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "PeriodId is required!" } };
                goto Finish;
            }
            var findPeriod = _uow.GetRepository<TargetPlanPeriod>().GetSingle(x => x.Id == agrs.PeriodId);
            if (findPeriod == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Target Period is not exist!" } };
                goto Finish;
            }

            if (agrs.List == null || !agrs.List.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "List is required!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => x.UserId == null || x.UserId == Guid.Empty))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "UserId in List is required!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => x.DepartmentId == null || x.DepartmentId == Guid.Empty))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DepartmentId in List is required!" } };
                goto Finish;
            }

            var targetPlanTypeValid = new List<TypeTargetPlan> { TypeTargetPlan.Target1, TypeTargetPlan.Target2 };
            if (agrs.List.Any(x => !targetPlanTypeValid.Contains(x.Type)))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Type in List is invalid!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => !x.Targets.Any() || x.Targets == null))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Targets in List is required!" } };
                goto Finish;
            }

            var findAllUserInDivisions = await _uow.GetRepository<UserDepartmentMapping>(true).FindByAsync(x => x.DepartmentId != null && x.UserId != null && x.DepartmentId == agrs.DivisionId);
            if (!findAllUserInDivisions.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Cannot find any user in division {agrs.DivisionId}" } };
                goto Finish;
            }

            var allUserInDivision = findAllUserInDivisions.Select(x => x.User).ToList();
            var allUserInPayload = agrs.List.Select(x => x.UserId).ToList();

            #region Validate Include Children
            /*var findUserSubmitPersion = _uow.GetRepository<UserSubmitPersonDeparmentMapping>().GetSingle(x => x.UserId == agrs.RequestorId && x.DepartmentId == agrs.DivisionId);
            if (findUserSubmitPersion == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findRequestor.SAPCode} is not submit person in division {findDivision.SAPCode}" } };
                goto Finish;
            }

            if (!findUserSubmitPersion.IsIncludeChildren)
            {
                if (allUserInDivision.Count <= allUserInDivision.Count)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User in division {findDivision.Code} is not match with user in payload!: [HR: {allUserInDivision.Count} <-> Payload: {allUserInDivision.Count}]" } };
                    goto Finish;
                }
            }*/
            #endregion

            /*var checkValidUserInPayload = agrs.List.Select(x => x.SAPCode).Except(allUserInDivision.Where(x => x == null || !x.IsActivated).Select(x => x.SAPCode)).ToList();
            if (checkValidUserInPayload.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User in payload: {JsonConvert.SerializeObject(string.Join(",", checkValidUserInPayload))} is Inactivated!" } };
                goto Finish;
            }*/

            #region Validate rule Edoc
            List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
            foreach (var item in agrs.List)
            {
                var findUserInDepartment = _uow.GetRepository<UserDepartmentMapping>().GetSingle(x =>
                x.DepartmentId != null && x.IsHeadCount && x.UserId != null && x.UserId == item.UserId && x.DepartmentId == item.DepartmentId
                && x.Department != null && x.User != null);
                if (findUserInDepartment == null)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"DepartmentId {item.DepartmentId} in List is invalid!" } };
                    goto Finish;
                }
                var existTargetPlan = _uow.GetRepository<TargetPlanDetail>(true).Count(x => x.SAPCode == findUserInDepartment.User.SAPCode && x.TargetPlan.PeriodId == findPeriod.Id);
                /*if (existTargetPlan > 0)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findUserInDepartment.User.SAPCode} is exists Target Plan!" } };
                    goto Finish;
                }*/

                var existPendingTarget = _uow.GetRepository<PendingTargetPlanDetail>(true).FindBy(x => x.SAPCode == findUserInDepartment.User.SAPCode && x.PendingTargetPlan.PeriodId == findPeriod.Id);
                /*if (existPendingTarget.Any(x => x.IsSent))
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findUserInDepartment.User.SAPCode} is exists Pending target!" } };
                    goto Finish;
                }*/

                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(i => i.Id == item.UserId && i.IsActivated);
                var resingation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(i => i.UserSAPCode == currentUser.SAPCode && i.Status.Contains("Completed") && !i.IsCancelResignation, "Created Desc");

                var startDate = findPeriod.FromDate;
                var endDate = findPeriod.ToDate;

                if (resingation != null && resingation.OfficialResignationDate.Date >= currentUser.StartDate.Value.Date && resingation.OfficialResignationDate.Date <= findPeriod.ToDate)
                {
                    endDate = resingation.OfficialResignationDate.Date.AddDays(-1);
                }
                if (currentUser != null && currentUser.StartDate.HasValue && currentUser.StartDate.Value.Date > findPeriod.FromDate)
                {
                    startDate = currentUser.StartDate.Value;
                }

                #region Validate format date
                foreach (var target in item.Targets)
                {
                    if (!DateTimeOffset.TryParseExact(target.date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {target.date} is invalid format!" } };
                        goto Finish;
                    }
                    else
                    {
                        if (parsedDate.Date >= startDate.Date && parsedDate.Date <= endDate.Date)
                        {
                            if (string.IsNullOrEmpty(target.value))
                            {
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Value in date {target.date} is required!" } };
                                goto Finish;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(target.value))
                            {
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {target.date} in user {currentUser.SAPCode} is invalid! Period from: {startDate.Date.ToString("yyyyMMdd")} - Period to: {endDate.Date.ToString("yyyyMMdd")}" } };
                                goto Finish;
                            }
                        }
                    }
                }
                #endregion

                // ràng đúng chu kì
                var validDates = item.Targets.Where(x => {
                    var dateStr = x.date.ToString();
                    var parsedDate = DateTimeOffset.ParseExact(dateStr, "yyyyMMdd", null);
                    return parsedDate < findPeriod.FromDate || parsedDate > findPeriod.ToDate;
                }).ToList();
                if (validDates.Any())
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {string.Join(",", validDates.Select(x => x.date))} in param Targets is not in target period!" } };
                    goto Finish;
                }

                var findIdPendingType1Id = Guid.Empty;
                var findIdPendingType2Id = Guid.Empty;

                if (existPendingTarget.Any())
                {
                    findIdPendingType1Id = existPendingTarget?.First(x => x.Type == TypeTargetPlan.Target1).Id ?? Guid.Empty;
                    findIdPendingType2Id = existPendingTarget?.First(x => x.Type == TypeTargetPlan.Target2).Id ?? Guid.Empty;
                }

                // Tao moi
                targetPlanDetails.Add(new TargetPlanArgDetail
                {
                    PenDetailId = findIdPendingType1Id,
                    SAPCode = findUserInDepartment.User.SAPCode,
                    FullName = findUserInDepartment.User.FullName,
                    JsonData = JsonConvert.SerializeObject(item.Targets),
                    Type = TypeTargetPlan.Target1,
                    ALHQuality = item.Targets.Where(x => x.value.Contains("AL")).Count(),
                    ERDQuality = item.Targets.Where(x => x.value.Contains("ERD")).Count(),
                    PRDQuality = item.Targets.Where(x => x.value.Contains("PRD")).Count(),
                    DOFLQuality = item.Targets.Where(x => x.value.Contains("DOFL")).Count(),
                    DepartmentCode = findUserInDepartment.Department.Code,
                    DepartmentName = findUserInDepartment.Department.Name,
                    PeriodId = findPeriod.Id
                });

                var createManualTarget2 = item.Targets.Select(x => new TargetPlanFromImportDetailItemDTO() { date = x.date, value = "" }).ToList();
                targetPlanDetails.Add(new TargetPlanArgDetail
                {
                    PenDetailId = findIdPendingType2Id,
                    SAPCode = findUserInDepartment.User.SAPCode,
                    FullName = findUserInDepartment.User.FullName,
                    JsonData = JsonConvert.SerializeObject(createManualTarget2),
                    Type = TypeTargetPlan.Target2,
                    ALHQuality = 0,
                    ERDQuality = 0,
                    PRDQuality = 0,
                    DOFLQuality = 0,
                    DepartmentCode = findUserInDepartment.Department.Code,
                    DepartmentName = findUserInDepartment.Department.Name,
                    PeriodId = findPeriod.Id
                });
            }

            var noPRD = await ValidateNoPRD(targetPlanDetails, result, findPeriod.FromDate.Date, findPeriod.ToDate.Date);
            if (noPRD.ErrorCodes.Count > 0)
            {
                goto Finish;
            }

            var halfDayInvalid = ValidateHalfDay(targetPlanDetails, result);
            if (halfDayInvalid.ErrorCodes.Count > 0)
            {
                goto Finish;
            }

            var sapCodeInvalid_StartDate = await ValidateStartDate(targetPlanDetails, result);
            if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
            {
                result = sapCodeInvalid_StartDate;
                goto Finish;
            }
            var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(targetPlanDetails, result);
            if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
            {
                result = sapCodeInvalid_ShiftCodeHQ;
                goto Finish;
            }

            var dataInvalid = await ValidateTargetPlanByJsonFile(targetPlanDetails, result, findPeriod.Id);
            if (dataInvalid.ErrorCodes.Count > 0)
            {
                List<string> errorsByJsonFile = new List<string>();
                foreach (var item in dataInvalid.Messages)
                {
                    var parts = item.Split('|');
                    string modifiedItem = $"{parts[0]} ShiftCode is not valid";
                    errorsByJsonFile.Add(modifiedItem);
                    errorsByJsonFile.Add(item);
                }
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = errorsByJsonFile };
                goto Finish;
            }

            var dataImportInvalid = ValidateImportData(targetPlanDetails, result);
            if (dataImportInvalid.ErrorCodes.Count > 0)
            {
                
                goto Finish;
            }
            var errors = await ValidateTargetPlanCustom(targetPlanDetails);
            if (errors.ErrorCodes.Count == 0)
            {
                var res = await ValidateTargetPlan(new TargetPlanArg { PeriodFromDate = findPeriod.FromDate, PeriodToDate = findPeriod.ToDate, List = targetPlanDetails });
                if (res.Object != null)
                {
                    ArrayResultDTO arrayErrors = (ArrayResultDTO)res.Object;
                    if (arrayErrors.Count > 0)
                    {
                        var objectValidate = Mapper.Map<List<ResultValidateTargetPlanViewModel>>(arrayErrors.Data);
                        //result.Messages = new List<string>() { $"{objectValidate[0].SAPCode} PRD code does not have enough numbers on Sundays" };
                        result = new ResultDTO { ErrorCodes = { -1 }, Messages = getErrorsListFromValidateTarget(objectValidate) };
                        goto Finish;
                    }
                    else
                    {
                        var reCheck = await ValidateTargetPlanV2(new TargetPlanArg { PeriodFromDate = findPeriod.FromDate, PeriodToDate = findPeriod.ToDate, List = targetPlanDetails });
                        if (reCheck.Object != null)
                        {
                            arrayErrors = (ArrayResultDTO)reCheck.Object;
                            if (arrayErrors.Count > 0)
                            {
                                var objectValidate = Mapper.Map<List<ResultValidateTargetPlanViewModel>>(arrayErrors.Data);
                                //result.Messages = new List<string>() { $"{objectValidate[0].SAPCode} PRD code does not have enough numbers on Sundays" };
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = getErrorsListFromValidateTarget(objectValidate) };
                                goto Finish;
                            }
                        }
                    }
                }
            }
            if (errors.ErrorCodes.Count > 0)
            {
                result = errors;
                return result;
            }
        #endregion

        Finish:

            if (result.ErrorCodes.Count > 0)
            {
                try
                {
                    newLog.Response = JsonConvert.SerializeObject(result);
                    await _uow.CommitAsync();
                }
                catch (Exception e)
                {
                    result.Messages.Add(e.Message);
                    newLog.Response = JsonConvert.SerializeObject(result);
                    await _uow.CommitAsync();
                }
            }
            return result;


        }

        public async Task<ResultDTO> CreateTargetPlan_API(CreateTargetPlan_APIArgs agrs)
        {
            var result = new ResultDTO();
            var newLog = new TrackingAPIIntegrationLog()
            {
                Module = "DWS",
                APIName = "CreateTargetPlan_API",
                Payload = JsonConvert.SerializeObject(agrs),
            };
            _uow.GetRepository<TrackingAPIIntegrationLog>().Add(newLog);
            await _uow.CommitAsync(); 

            if (agrs == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Cannot find any param!" } };
                goto Finish;
            }

            if (agrs.RequestorId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "RequestorId is required!" } };
                goto Finish;
            }
            var findRequestor = _uow.GetRepository<User>().GetSingle(x => x.Id == agrs.RequestorId);
            if (findRequestor == null || (findRequestor != null && !findRequestor.IsActivated))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Requestor is not exist!" } };
                goto Finish;
            }
            _uow.UserContext.CurrentUserId = findRequestor.Id;
            _uow.UserContext.CurrentUserName = findRequestor.LoginName;
            _uow.UserContext.CurrentUserFullName = findRequestor.FullName;

            if (agrs.DeptLineId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DeptLineId is required!" } };
                goto Finish;
            }
            var findDeptLine = _uow.GetRepository<Department>().GetSingle(x => x.Id == agrs.DeptLineId);
            if (findDeptLine == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DeptLineId is not exist!" } };
                goto Finish;
            }

            if (agrs.DivisionId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DivisionId is required!" } };
                goto Finish;
            }
            var findDivision = _uow.GetRepository<Department>().GetSingle(x => x.Id == agrs.DivisionId);
            if (findDivision == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Division is not exist!" } };
                goto Finish;
            }

            if (agrs.PeriodId == Guid.Empty)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "PeriodId is required!" } };
                goto Finish;
            }
            var findPeriod = _uow.GetRepository<TargetPlanPeriod>().GetSingle(x => x.Id == agrs.PeriodId);
            if (findPeriod == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Target Period is not exist!" } };
                goto Finish;
            }

            if (agrs.List == null || !agrs.List.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "List is required!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => x.UserId == null || x.UserId == Guid.Empty))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "UserId in List is required!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => x.DepartmentId == null || x.DepartmentId == Guid.Empty))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "DepartmentId in List is required!" } };
                goto Finish;
            }

            var targetPlanTypeValid = new List<TypeTargetPlan> { TypeTargetPlan.Target1, TypeTargetPlan.Target2 };
            if (agrs.List.Any(x => !targetPlanTypeValid.Contains(x.Type)))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Type in List is invalid!" } };
                goto Finish;
            }

            if (agrs.List.Any(x => !x.Targets.Any() || x.Targets == null))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { "Targets in List is required!" } };
                goto Finish;
            }

            var findAllUserInDivisions = await _uow.GetRepository<UserDepartmentMapping>(true).FindByAsync(x => x.DepartmentId != null && x.UserId != null && x.DepartmentId == agrs.DivisionId);
            if (!findAllUserInDivisions.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Cannot find any user in division {agrs.DivisionId}" } };
                goto Finish;
            }

            var allUserInDivision = findAllUserInDivisions.Select(x => x.User).ToList();
            var allUserInPayload = agrs.List.Select(x => x.UserId).ToList();

            #region Validate Include Children
            var findUserSubmitPersion = _uow.GetRepository<UserSubmitPersonDeparmentMapping>().GetSingle(x => x.UserId == agrs.RequestorId && x.DepartmentId == agrs.DivisionId);
            if (findUserSubmitPersion == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findRequestor.SAPCode} is not submit person in division {findDivision.Code}" } };
                goto Finish;
            }

            if (!findUserSubmitPersion.IsIncludeChildren)
            {
                if (allUserInDivision.Count <= allUserInDivision.Count)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User in division {findDivision.Code} is not match with user in payload!: [HR: {allUserInDivision.Count} <-> Payload: {allUserInDivision.Count}]" } };
                    goto Finish;
                }
            }
            #endregion

            /*var checkValidUserInPayload = agrs.List.Select(x => x.SAPCode).Except(allUserInDivision.Where(x => x == null || !x.IsActivated).Select(x => x.SAPCode)).ToList();
            if (checkValidUserInPayload.Any())
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User in payload: {JsonConvert.SerializeObject(string.Join(",", checkValidUserInPayload))} is Inactivated!" } };
                goto Finish;
            }*/

            #region Validate rule Edoc
            List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
            foreach (var item in agrs.List)
            {
                var findUserInDepartment = _uow.GetRepository<UserDepartmentMapping>().GetSingle(x =>
                x.DepartmentId != null && x.IsHeadCount && x.UserId != null && x.UserId == item.UserId && x.DepartmentId == item.DepartmentId
                && x.Department != null && x.User != null);
                if (findUserInDepartment == null)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"DepartmentId {item.DepartmentId} in List is invalid!" } };
                    goto Finish;
                }
                var existTargetPlan = _uow.GetRepository<TargetPlanDetail>(true).Count(x => x.SAPCode == findUserInDepartment.User.SAPCode && x.TargetPlan.PeriodId == findPeriod.Id);
                if (existTargetPlan > 0)
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findUserInDepartment.User.SAPCode} is exists Target Plan!" } };
                    goto Finish;
                }

                var existPendingTarget = _uow.GetRepository<PendingTargetPlanDetail>(true).FindBy(x => x.SAPCode == findUserInDepartment.User.SAPCode && x.PendingTargetPlan.PeriodId == findPeriod.Id);
                if (existPendingTarget.Any(x => x.IsSent))
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"User {findUserInDepartment.User.SAPCode} is exists Pending target!" } };
                    goto Finish;
                }

                var currentUser = await _uow.GetRepository<User>().GetSingleAsync<UserForTreeViewModel>(i => i.Id == item.UserId && i.IsActivated);
                var resingation = await _uow.GetRepository<ResignationApplication>(true).GetSingleAsync(i => i.UserSAPCode == currentUser.SAPCode && i.Status.Contains("Completed") && !i.IsCancelResignation, "Created Desc");
                
                var startDate = findPeriod.FromDate;
                var endDate = findPeriod.ToDate;

                if (resingation != null && resingation.OfficialResignationDate.Date >= currentUser.StartDate.Value.Date && resingation.OfficialResignationDate.Date <= findPeriod.ToDate)
                {
                    endDate = resingation.OfficialResignationDate.Date.AddDays(-1);
                }
                if (currentUser != null && currentUser.StartDate.HasValue && currentUser.StartDate.Value.Date > findPeriod.FromDate)
                {
                    startDate = currentUser.StartDate.Value;
                }

                #region Validate format date
                foreach (var target in item.Targets)
                {
                    if (!DateTimeOffset.TryParseExact(target.date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {target.date} is invalid format!" } };
                        goto Finish;
                    }
                    else
                    {
                        if (parsedDate.Date >= startDate.Date && parsedDate.Date <= endDate.Date)
                        {
                            if (string.IsNullOrEmpty(target.value))
                            {
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Value in date {target.date} is required!" } };
                                goto Finish;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(target.value))
                            {
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {target.date} in user {currentUser.SAPCode} is invalid! Period from: {startDate.Date.ToString("yyyyMMdd")} - Period to: {endDate.Date.ToString("yyyyMMdd")}" } };
                                goto Finish;
                            }
                        }
                    }
                }
                #endregion

                // ràng đúng chu kì
                var validDates = item.Targets.Where(x => {
                    var dateStr = x.date.ToString();
                    var parsedDate = DateTimeOffset.ParseExact(dateStr, "yyyyMMdd", null);
                    return parsedDate < findPeriod.FromDate || parsedDate > findPeriod.ToDate;
                }).ToList();
                if (validDates.Any())
                {
                    result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Date {string.Join(",", validDates.Select(x => x.date))} in param Targets is not in target period!" } };
                    goto Finish;
                }

                var findIdPendingType1Id = Guid.Empty;
                var findIdPendingType2Id = Guid.Empty;

                if (existPendingTarget.Any())
                {
                    findIdPendingType1Id = existPendingTarget?.First(x => x.Type == TypeTargetPlan.Target1).Id ?? Guid.Empty;
                    findIdPendingType2Id = existPendingTarget?.First(x => x.Type == TypeTargetPlan.Target2).Id ?? Guid.Empty;
                }
                
                // Tao moi
                targetPlanDetails.Add(new TargetPlanArgDetail
                {
                    PenDetailId = findIdPendingType1Id,
                    SAPCode = findUserInDepartment.User.SAPCode,
                    FullName = findUserInDepartment.User.FullName,
                    JsonData = JsonConvert.SerializeObject(item.Targets),
                    Type = TypeTargetPlan.Target1,
                    ALHQuality = item.Targets.Where(x => x.value.Contains("AL")).Count(),
                    ERDQuality = item.Targets.Where(x => x.value.Contains("ERD")).Count(),
                    PRDQuality = item.Targets.Where(x => x.value.Contains("PRD")).Count(),
                    DOFLQuality = item.Targets.Where(x => x.value.Contains("DOFL")).Count(),
                    DepartmentCode = findUserInDepartment.Department.Code,
                    DepartmentName = findUserInDepartment.Department.Name,
                    PeriodId = findPeriod.Id
                });

                var createManualTarget2 = item.Targets.Select(x => new TargetPlanFromImportDetailItemDTO() { date = x.date, value = "" }).ToList();
                targetPlanDetails.Add(new TargetPlanArgDetail
                {
                    PenDetailId = findIdPendingType2Id,
                    SAPCode = findUserInDepartment.User.SAPCode,
                    FullName = findUserInDepartment.User.FullName,
                    JsonData = JsonConvert.SerializeObject(createManualTarget2),
                    Type = TypeTargetPlan.Target2,
                    ALHQuality = 0,
                    ERDQuality = 0,
                    PRDQuality = 0,
                    DOFLQuality = 0,
                    DepartmentCode = findUserInDepartment.Department.Code,
                    DepartmentName = findUserInDepartment.Department.Name,
                    PeriodId = findPeriod.Id
                });
            }

            var noPRD = await ValidateNoPRD(targetPlanDetails, result, findPeriod.FromDate.Date, findPeriod.ToDate.Date);
            if (noPRD.ErrorCodes.Count > 0)
            {
                goto Finish;
            }

            var halfDayInvalid = ValidateHalfDay(targetPlanDetails, result);
            if (halfDayInvalid.ErrorCodes.Count > 0)
            {
                goto Finish;
            }

            var sapCodeInvalid_StartDate = await ValidateStartDate(targetPlanDetails, result);
            if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
            {
                result = sapCodeInvalid_StartDate;
                goto Finish;
            }
            var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(targetPlanDetails, result);
            if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
            {
                result = sapCodeInvalid_ShiftCodeHQ;
                goto Finish;
            }

            var dataInvalid = await ValidateTargetPlanByJsonFile(targetPlanDetails, result, findPeriod.Id);
            if (dataInvalid.ErrorCodes.Count > 0)
            {
                List<string> errorsByJsonFile = new List<string>();
                foreach (var item in dataInvalid.Messages)
                {
                    var parts = item.Split('|');
                    string modifiedItem = $"{parts[0]} ShiftCode is not valid";
                    errorsByJsonFile.Add(modifiedItem);
                    errorsByJsonFile.Add(item);
                }
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = errorsByJsonFile };
                goto Finish;
            }

            var dataImportInvalid = ValidateImportData(targetPlanDetails, result);
            if (dataImportInvalid.ErrorCodes.Count > 0)
            {
                goto Finish;
            }
            var errors = await ValidateTargetPlanCustom(targetPlanDetails);
            if (errors.ErrorCodes.Count == 0)
            {
                var res = await ValidateTargetPlan(new TargetPlanArg { PeriodFromDate = findPeriod.FromDate, PeriodToDate = findPeriod.ToDate, List = targetPlanDetails });
                if (res.Object != null)
                {
                    ArrayResultDTO arrayErrors = (ArrayResultDTO)res.Object;
                    if (arrayErrors.Count > 0)
                    {
                        var objectValidate = Mapper.Map<List<ResultValidateTargetPlanViewModel>>(arrayErrors.Data);
                        //result.Messages = new List<string>() { $"{objectValidate[0].SAPCode} PRD code does not have enough numbers on Sundays" };
                        result = new ResultDTO { ErrorCodes = { -1 }, Messages = getErrorsListFromValidateTarget(objectValidate) };
                        goto Finish;
                    }
                    else
                    {
                        var reCheck = await ValidateTargetPlanV2(new TargetPlanArg { PeriodFromDate = findPeriod.FromDate, PeriodToDate = findPeriod.ToDate, List = targetPlanDetails });
                        if (reCheck.Object != null)
                        {
                            arrayErrors = (ArrayResultDTO)reCheck.Object;
                            if (arrayErrors.Count > 0)
                            {
                                var objectValidate = Mapper.Map<List<ResultValidateTargetPlanViewModel>>(arrayErrors.Data);
                                //result.Messages = new List<string>() { $"{objectValidate[0].SAPCode} PRD code does not have enough numbers on Sundays" };
                                result = new ResultDTO { ErrorCodes = { -1 }, Messages = getErrorsListFromValidateTarget(objectValidate) };
                                goto Finish;
                            }
                        }
                    }
                }
            }
            if (errors.ErrorCodes.Count > 0)
            {
                result = errors;
                return result;
            }
            #endregion

            #region CREATE PENDING
            var headerPendingTargetPlan = new PendingTargetPlan()
            {
                DeptId = findDeptLine.Id,
                DeptCode = findDeptLine.Code,
                DeptName = findDeptLine.Name,
                DivisionId = findDivision.Id,
                DivisionCode = findDivision.Code,
                DivisionName = findDivision.Name,
                PeriodId = findPeriod.Id,
                PeriodName = findPeriod.Name,
                PeriodFromDate = findPeriod.FromDate,
                PeriodToDate = findPeriod.ToDate
            };
            _uow.GetRepository<PendingTargetPlan>().Add(headerPendingTargetPlan);

            var listPending = new List<PendingTargetPlanDetail>() { };
            foreach (var item in targetPlanDetails)
            {
                if (item.PenDetailId != Guid.Empty)
                {
                    var findExistPendingTargetPlanDetail = _uow.GetRepository<PendingTargetPlanDetail>(true).GetSingle(x => x.Id == item.PenDetailId);
                    if (findExistPendingTargetPlanDetail == null)
                    {
                        result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"PendingTargetDetailId: {findExistPendingTargetPlanDetail.Id} is not exists!" } };
                        goto Finish;
                    }

                    findExistPendingTargetPlanDetail.DepartmentCode = item.DepartmentCode;
                    findExistPendingTargetPlanDetail.DepartmentName = item.DepartmentName;
                    findExistPendingTargetPlanDetail.SAPCode = item.SAPCode;
                    findExistPendingTargetPlanDetail.FullName = item.FullName;
                    findExistPendingTargetPlanDetail.ALHQuality = item.Type == TypeTargetPlan.Target1 ? item.ALHQuality : 0;
                    findExistPendingTargetPlanDetail.ERDQuality = item.Type == TypeTargetPlan.Target1 ? item.ERDQuality : 0;
                    findExistPendingTargetPlanDetail.PRDQuality = item.Type == TypeTargetPlan.Target1 ? item.PRDQuality : 0;
                    findExistPendingTargetPlanDetail.DOFLQuality = item.Type == TypeTargetPlan.Target1 ? item.DOFLQuality : 0;
                    findExistPendingTargetPlanDetail.JsonData = item.JsonData;
                    findExistPendingTargetPlanDetail.PeriodId = findPeriod.Id;
                    findExistPendingTargetPlanDetail.Type = item.Type;
                    findExistPendingTargetPlanDetail.IsSent = true;
                    findExistPendingTargetPlanDetail.IsSubmitted = true;
                    _uow.GetRepository<PendingTargetPlanDetail>().Update(findExistPendingTargetPlanDetail);
                    listPending.Add(findExistPendingTargetPlanDetail);
                } else
                {
                    var newPendingTargetPlanDetailType1 = new PendingTargetPlanDetail
                    {
                        PendingTargetPlanId = headerPendingTargetPlan.Id,
                        DepartmentCode = item.DepartmentCode,
                        DepartmentName = item.DepartmentName,
                        SAPCode = item.SAPCode,
                        FullName = item.FullName,
                        ALHQuality = item.Type == TypeTargetPlan.Target1 ? item.ALHQuality : 0,
                        ERDQuality = item.Type == TypeTargetPlan.Target1 ? item.ERDQuality : 0,
                        PRDQuality = item.Type == TypeTargetPlan.Target1 ? item.PRDQuality : 0,
                        DOFLQuality = item.Type == TypeTargetPlan.Target1 ? item.DOFLQuality : 0,
                        JsonData = item.JsonData,
                        PeriodId = findPeriod.Id,
                        Type = item.Type,
                        IsSent = true,
                        IsSubmitted = true
                    };
                    _uow.GetRepository<PendingTargetPlanDetail>().Add(newPendingTargetPlanDetailType1);
                    listPending.Add(newPendingTargetPlanDetailType1);
                }
            }
            #endregion

            #region CREATE TARGET
            var newHeaderTargetPlan = new TargetPlan()
            {
                UserSAPCode = findRequestor.SAPCode,
                UserFullName = findRequestor.FullName,
                DeptId = findDeptLine.Id,
                DeptCode = findDeptLine.Code,
                DeptName = findDeptLine.Name,
                DivisionId = findDivision.Id,
                DivisionCode = findDivision.Code,
                DivisionName = findDivision.Name,
                PeriodId = findPeriod.Id,
                PeriodName = findPeriod.Name,
                PeriodFromDate = findPeriod.FromDate,
                PeriodToDate = findPeriod.ToDate,
                IsStore = await CheckIsStoreFromDepartment(agrs.DivisionId != null ? agrs.DivisionId : agrs.DeptLineId)
            };
            _uow.GetRepository<TargetPlan>().Add(newHeaderTargetPlan);

            foreach (var item in targetPlanDetails)
            {
                var newTargetPlanDetail = new TargetPlanDetail
                {
                    TargetPlanId = newHeaderTargetPlan.Id,
                    DepartmentCode = item.DepartmentCode,
                    DepartmentName = item.DepartmentName,
                    SAPCode = item.SAPCode,
                    FullName = item.FullName,
                    ALHQuality = item.Type == TypeTargetPlan.Target1 ? item.ALHQuality : 0,
                    ERDQuality = item.Type == TypeTargetPlan.Target1 ? item.ERDQuality : 0,
                    PRDQuality = item.Type == TypeTargetPlan.Target1 ? item.PRDQuality : 0,
                    DOFLQuality = item.Type == TypeTargetPlan.Target1 ? item.DOFLQuality : 0,
                    JsonData = item.JsonData,
                    Type = item.Type
                };
                _uow.GetRepository<TargetPlanDetail>().Add(newTargetPlanDetail);
            }
            #endregion

            var addParmission = new Permission()
            {
                ItemId = newHeaderTargetPlan.Id,
                Perm = Right.View,
                UserId = findRequestor.Id
            };
            _uow.GetRepository<Permission>().Add(addParmission);
            await _uow.CommitAsync();

            #region Start workflow
            var findWorklfow = await _workflowBO.GetWorkflowStatusByItemId(newHeaderTargetPlan.Id, true);
            if (!findWorklfow.IsSuccess)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Cannot find workflow!" } };

                foreach(var itemDe in listPending) _uow.GetRepository<PendingTargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<PendingTargetPlan>().Delete(headerPendingTargetPlan);

                var findTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId == newHeaderTargetPlan.Id);
                foreach (var itemDe in findTargetPlanDetails) _uow.GetRepository<TargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<TargetPlan>().Delete(newHeaderTargetPlan);

                await _uow.CommitAsync();
                goto Finish;
            }
            var parseResponseWorkflow = Mapper.Map<WorkflowStatusViewModel>(findWorklfow.Object);
            if (parseResponseWorkflow == null || (parseResponseWorkflow != null && parseResponseWorkflow.WorkflowButtons == null))
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Cannot find workflow!" } };
                foreach (var itemDe in listPending) _uow.GetRepository<PendingTargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<PendingTargetPlan>().Delete(headerPendingTargetPlan);

                var findTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId == newHeaderTargetPlan.Id);
                foreach (var itemDe in findTargetPlanDetails) _uow.GetRepository<TargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<TargetPlan>().Delete(newHeaderTargetPlan);

                await _uow.CommitAsync();
                goto Finish;
            }

            var findSendData = parseResponseWorkflow.WorkflowButtons.First(x => x.Name.Equals("Send Request"));
            if (findSendData == null)
            {
                result = new ResultDTO { ErrorCodes = { -1 }, Messages = { $"Cannot find workflow!" } };
                foreach (var itemDe in listPending) _uow.GetRepository<PendingTargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<PendingTargetPlan>().Delete(headerPendingTargetPlan);

                var findTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>().FindByAsync(x => x.TargetPlanId == newHeaderTargetPlan.Id);
                foreach (var itemDe in findTargetPlanDetails) _uow.GetRepository<TargetPlanDetail>().Delete(itemDe);
                _uow.GetRepository<TargetPlan>().Delete(newHeaderTargetPlan);

                await _uow.CommitAsync();
                goto Finish;
            }
            var startWorkflow = await _workflowBO.StartWorkflow(findSendData.Id, newHeaderTargetPlan.Id);

            result.Object = Mapper.Map<TargetPlanViewModel>(newHeaderTargetPlan);
            #endregion

            var data = new ArrayResultDTO();
            var errorSAPCode = new List<string>();
            /*List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
            if (!arg.VisibleSubmit)
            {
                var existsTarget = await _uow.GetRepository<PendingTargetPlanDetail>().GetSingleAsync<PendingTargetPlanDetailViewModel>(i => arg.SAPCodes.Contains(i.SAPCode) && i.IsSent && i.PendingTargetPlan.PeriodId == arg.PeriodId);
                if (existsTarget != null)
                {
                    result.ErrorCodes.Add(1001);
                    result.Messages.Add("TARGET_PLAN_VALIDATE_EXISTS_TARGET");
                    return result;
                }
            }
            var users = await _uow.GetRepository<User>().FindByAsync<UserForTreeViewModel>(x => arg.SAPCodes.Contains(x.SAPCode) && x.IsActivated);
            var dataFromFile = ReadDataFromStream(stream, users);
            if (dataFromFile.Data.Count > 0)
            {
                var currentPeriod = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                if (currentPeriod != null)
                {
                    if (dataFromFile.PeriodFromDate.Value.Date.ToSAPFormat().Equals(currentPeriod.FromDate.Date.ToSAPFormat()) && (dataFromFile.PeriodToDate.Value.Date.ToSAPFormat().Equals(currentPeriod.ToDate.Date.ToSAPFormat())))
                    {
                        var groupSapcodes = dataFromFile.Data.GroupBy(x => x.SAPCode);
                        foreach (var group in groupSapcodes)
                        {
                            foreach (var item in group)
                            {
                                var currentUser = users.Where(x => x.SAPCode == item.SAPCode).FirstOrDefault();
                                if (currentUser != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Target) && item.Target.Trim().ToLower().Contains("1-target"))
                                    {
                                        targetPlanDetails.Add(new TargetPlanArgDetail
                                        {
                                            SAPCode = item.SAPCode,
                                            FullName = currentUser.FullName,
                                            JsonData = JsonConvert.SerializeObject(item.Targets),
                                            Type = TypeTargetPlan.Target1,
                                            ALHQuality = item.Targets.Where(x => x.value.Contains("AL")).Count(),
                                            ERDQuality = item.Targets.Where(x => x.value.Contains("ERD")).Count(),
                                            PRDQuality = item.Targets.Where(x => x.value.Contains("PRD")).Count(),
                                            DOFLQuality = item.Targets.Where(x => x.value.Contains("DOFL")).Count(),
                                            DepartmentCode = currentUser?.DepartmentCode,
                                            DepartmentName = currentUser?.Department,
                                            PeriodId = arg.PeriodId
                                        });

                                    }
                                    else if (!string.IsNullOrEmpty(item.Target) && item.Target.Trim().ToLower().Contains("2-target"))
                                    {
                                        targetPlanDetails.Add(new TargetPlanArgDetail
                                        {
                                            SAPCode = item.SAPCode,
                                            FullName = currentUser.FullName,
                                            JsonData = JsonConvert.SerializeObject(item.Targets),
                                            Type = TypeTargetPlan.Target2,
                                            ALHQuality = (float)item.Targets.Where(x => x.value.Contains("ALH1") || x.value.Contains("ALH2")).Count() / 2,
                                            ERDQuality = (float)item.Targets.Where(x => x.value.Contains("ERD1") || x.value.Contains("ERD2")).Count() / 2,
                                            PRDQuality = (float)item.Targets.Where(x => x.value.Contains("PRD1") || x.value.Contains("PRD2")).Count() / 2,
                                            DOFLQuality = (float)item.Targets.Where(x => x.value.Contains("DOH1") || x.value.Contains("DOH2")).Count() / 2,
                                            DepartmentCode = currentUser?.DepartmentCode,
                                            DepartmentName = currentUser?.Department,
                                            PeriodId = arg.PeriodId
                                        });

                                    }

                                }

                            }
                        }
                        var noPRD = await ValidateNoPRD(targetPlanDetails, result, currentPeriod.FromDate.Date, currentPeriod.ToDate.Date);
                        if (noPRD.ErrorCodes.Count > 0)
                        {
                            return result;
                        }

                        var halfDayInvalid = ValidateHalfDay(targetPlanDetails, result);
                        if (halfDayInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }

                        var sapCodeInvalid_StartDate = await ValidateStartDate(targetPlanDetails, result);
                        if (sapCodeInvalid_StartDate.ErrorCodes.Count > 0)
                        {
                            return sapCodeInvalid_StartDate;
                        }
                        var sapCodeInvalid_ShiftCodeHQ = await ValidateShiftCodeHQ(targetPlanDetails, result);
                        if (sapCodeInvalid_ShiftCodeHQ.ErrorCodes.Count > 0)
                        {
                            return sapCodeInvalid_ShiftCodeHQ;
                        }

                        var dataInvalid = await ValidateTargetPlanByJsonFile(targetPlanDetails, result, arg.PeriodId);
                        if (dataInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }

                        var dataImportInvalid = ValidateImportData(targetPlanDetails, result);
                        if (dataImportInvalid.ErrorCodes.Count > 0)
                        {
                            return result;
                        }
                        var errors = await ValidateTargetPlanCustom(targetPlanDetails);
                        if (errors.ErrorCodes.Count == 0)
                        {
                            var res = await ValidateTargetPlan(new TargetPlanArg { PeriodFromDate = currentPeriod.FromDate, PeriodToDate = currentPeriod.ToDate, List = targetPlanDetails });
                            if (res.Object != null)
                            {
                                ArrayResultDTO arrayErrors = (ArrayResultDTO)res.Object;
                                if (arrayErrors.Count > 0)
                                {
                                    result.ErrorCodes.Add(8);
                                    result.Object = res.Object;
                                    return result;
                                }
                                else
                                {
                                    var reCheck = await ValidateTargetPlanV2(new TargetPlanArg { PeriodFromDate = currentPeriod.FromDate, PeriodToDate = currentPeriod.ToDate, List = targetPlanDetails });
                                    if (res.Object != null)
                                    {
                                        arrayErrors = (ArrayResultDTO)res.Object;
                                        if (arrayErrors.Count > 0)
                                        {
                                            result.ErrorCodes.Add(8);
                                            result.Object = res.Object;
                                            return result;
                                        }
                                    }
                                }
                            }

                        }
                        if (errors.ErrorCodes.Count > 0)
                        {
                            result = errors;
                        }
                        else
                        {
                            data.Data = targetPlanDetails;
                            data.Count = targetPlanDetails.Count;
                        }

                    }
                    else
                    {
                        result.ErrorCodes.Add(1001);
                        result.Messages.Add("TARGET_PLAN_VALIDATE_PERIOD");
                    }
                }
                else
                {
                    result.ErrorCodes.Add(1001);
                    result.Messages.Add("Not Found Period");
                }

            }
            else
            {
                result.ErrorCodes.Add(1001);
                result.Messages.Add("TARGET_PLAN_VALIDATE_IMPORT_FILE");
            }
            if (data.Count > 0)
            {
                var addPendingTargetPlan = new List<PendingTargetPlanDetail>();
                arg.SAPCodes = targetPlanDetails.Select(x => x.SAPCode).Distinct().ToList();
                var currentPendingTargetPlans = await CheckExistPendingTargetPlans(new TargetPlanQueryArg { DepartmentId = arg.DeptId.Value, DivisionId = arg.DivisionId, PeriodId = arg.PeriodId, SAPCodes = arg.SAPCodes }); ;
                if (!currentPendingTargetPlans.Any()) // Chưa tạo pending Target Plan
                {

                    var pendingTarget = new PendingTargetPlan();
                    if (arg.DeptId.HasValue)
                    {
                        var deptLine = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DeptId.Value);
                        if (deptLine != null)
                        {
                            pendingTarget.DeptId = arg.DeptId.Value;
                            pendingTarget.DeptCode = deptLine.Code;
                            pendingTarget.DeptName = deptLine.Name;
                        }

                    }
                    if (arg.DivisionId.HasValue)
                    {
                        var divisions = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DivisionId.Value);
                        if (divisions != null)
                        {
                            pendingTarget.DivisionId = arg.DivisionId.Value;
                            pendingTarget.DivisionCode = divisions.Code;
                            pendingTarget.DivisionName = divisions.Name;
                        }
                    }
                    if (arg.PeriodId != Guid.Empty)
                    {
                        var period = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                        if (period != null)
                        {
                            pendingTarget.PeriodId = period.Id;
                            pendingTarget.PeriodName = period.Name;
                            pendingTarget.PeriodFromDate = period.FromDate;
                            pendingTarget.PeriodToDate = period.ToDate;
                        }
                    }
                    _uow.GetRepository<PendingTargetPlan>().Add(pendingTarget);
                    foreach (var item in targetPlanDetails)
                    {
                        item.PendingTargetPlanId = pendingTarget.Id;
                        addPendingTargetPlan.Add(new PendingTargetPlanDetail
                        {
                            Id = new Guid(),
                            DepartmentCode = item.DepartmentCode,
                            DepartmentName = item.DepartmentName,
                            SAPCode = item.SAPCode,
                            ALHQuality = item.ALHQuality,
                            ERDQuality = item.ERDQuality,
                            DOFLQuality = item.DOFLQuality,
                            PRDQuality = item.PRDQuality,
                            FullName = item.FullName,
                            JsonData = item.JsonData,
                            Type = item.Type,
                            PendingTargetPlanId = pendingTarget.Id,
                            PeriodId = arg.PeriodId,
                            IsSent = arg.VisibleSubmit

                        });
                    }
                    _uow.GetRepository<PendingTargetPlanDetail>().Add(addPendingTargetPlan);
                    await _uow.CommitAsync();
                }
                else
                {
                    //var existData = await _uow.GetRepository<PendingTargetPlanDetail>().FindByAsync(x => arg.SAPCodes.Contains(x.SAPCode) && x.PendingTargetPlan.PeriodId == arg.PeriodId);
                    var pendingPlanDetails = currentPendingTargetPlans.Where(x => (arg.VisibleSubmit ? !x.IsSubmitted : !x.IsSent));
                    var sapCodePendings = new List<string>();
                    if (pendingPlanDetails.Any())
                    {
                        foreach (var item in pendingPlanDetails)
                        {
                            var itemTarget = targetPlanDetails.FirstOrDefault(x => x.SAPCode == item.SAPCode && item.Type == x.Type);
                            if (itemTarget != null)
                            {
                                item.ALHQuality = itemTarget.ALHQuality;
                                item.ERDQuality = itemTarget.ERDQuality;
                                item.DOFLQuality = itemTarget.DOFLQuality;
                                item.PRDQuality = itemTarget.PRDQuality;
                                item.JsonData = itemTarget.JsonData;
                                itemTarget.PendingTargetPlanId = item.PendingTargetPlanId;
                                item.IsSent = arg.VisibleSubmit;
                            }
                            sapCodePendings.Add(item.SAPCode);
                        }
                        _uow.GetRepository<PendingTargetPlanDetail>().Update(pendingPlanDetails);
                    }
                    var notMadeTargetPlans = targetPlanDetails.Where(x => !sapCodePendings.Contains(x.SAPCode) && !currentPendingTargetPlans.Select(s => s.SAPCode).Contains(x.SAPCode));
                    if (notMadeTargetPlans.Any())
                    {
                        var pendingTarget = new PendingTargetPlan();
                        if (arg.DeptId.HasValue)
                        {
                            var deptLine = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DeptId.Value);
                            if (deptLine != null)
                            {
                                pendingTarget.DeptId = arg.DeptId.Value;
                                pendingTarget.DeptCode = deptLine.Code;
                                pendingTarget.DeptName = deptLine.Name;
                            }

                        }
                        if (arg.DivisionId.HasValue)
                        {
                            var divisions = await _uow.GetRepository<Department>().FindByIdAsync<DepartmentViewModel>(arg.DivisionId.Value);
                            if (divisions != null)
                            {
                                pendingTarget.DivisionId = arg.DivisionId.Value;
                                pendingTarget.DivisionCode = divisions.Code;
                                pendingTarget.DivisionName = divisions.Name;
                            }
                        }
                        if (arg.PeriodId != Guid.Empty)
                        {
                            var period = await _uow.GetRepository<TargetPlanPeriod>().FindByIdAsync<TargetPlanPeriodViewModel>(arg.PeriodId);
                            if (period != null)
                            {
                                pendingTarget.PeriodId = period.Id;
                                pendingTarget.PeriodName = period.Name;
                                pendingTarget.PeriodFromDate = period.FromDate;
                                pendingTarget.PeriodToDate = period.ToDate;
                            }
                        }
                        _uow.GetRepository<PendingTargetPlan>().Add(pendingTarget);
                        foreach (var item in notMadeTargetPlans)
                        {
                            //item.PendingTargetPlanId = currentPendingTargetPlan.Id;
                            addPendingTargetPlan.Add(new PendingTargetPlanDetail
                            {
                                DepartmentCode = item.DepartmentCode,
                                DepartmentName = item.DepartmentName,
                                SAPCode = item.SAPCode,
                                ALHQuality = item.ALHQuality,
                                ERDQuality = item.ERDQuality,
                                DOFLQuality = item.DOFLQuality,
                                PRDQuality = item.PRDQuality,
                                FullName = item.FullName,
                                Id = new Guid(),
                                JsonData = item.JsonData,
                                Type = item.Type,
                                PendingTargetPlanId = pendingTarget.Id,
                                PeriodId = arg.PeriodId,
                                IsSent = arg.VisibleSubmit
                            });
                        }
                        _uow.GetRepository<PendingTargetPlanDetail>().Add(addPendingTargetPlan);
                    }
                    await _uow.CommitAsync();
                }
            }*/
            // result.Object = data;

            Finish:

            if (result.ErrorCodes.Count > 0)
            {
                try
                {
                    newLog.Response = JsonConvert.SerializeObject(result);
                    await _uow.CommitAsync();
                } catch (Exception e)
                {
                    result.Messages.Add(e.Message);
                    newLog.Response = JsonConvert.SerializeObject(result);
                    await _uow.CommitAsync();
                }
            }
            return result;
        }
        #endregion

        private List<string> getErrorsListFromValidateTarget(List<ResultValidateTargetPlanViewModel> data)
        {
            List<string> errors = new List<string>();
            foreach (var item in data)
            {
                switch (item.TypeName)
                {
                    case "PRD":
                        errors.Add($"{item.SAPCode} PRD codes exceed the number of Sundays");
                        break;
                    case "PRDSPECTICAL":
                        errors.Add($"{item.SAPCode} PRD code does not have enough numbers on Sundays");
                        break;
                    case "DOFL":
                        errors.Add($"{item.SAPCode} DOFL codes exceed quota");
                        break;
                    case "AL":
                        errors.Add($"{item.SAPCode} AL codes exceed quota");
                        break;
                    case "ERD":
                        errors.Add($"{item.SAPCode} ERD codes exceed the allowed number");
                        break;
                    case "WFH":
                        errors.Add($"{item.SAPCode} WFH codes exceed quota");
                        break;
                    case "G2":
                        errors.Add($"{item.SAPCode} ERD codes cannot be used in shift registration");
                        break;
                    default:
                        break;
                }
            }
            return errors;
        }
    }

}
