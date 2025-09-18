using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using static Aeon.HR.ViewModels.CommonViewModel;
using System.Configuration;
using ServiceStack;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class CBBO
    {
        public static List<string> ListWFHInvalid = new List<string>() { "WFH", "WFH1", "WFH2" };
        public async Task<ResultDTO> GetAllShiftExchange(QueryArgs args)
        {
            //var items = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync<ShiftExchangeRequestViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            //var totalItems = await _uow.GetRepository<ShiftExchangeApplication>().CountAsync(args.Predicate, args.PredicateParameters);


            var resultData = new List<ShiftExchangeListViewModel>();
            var records = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var totalItems = await _uow.GetRepository<ShiftExchangeApplication>().CountAsync(args.Predicate, args.PredicateParameters);
            if (records.Any())
            {
                foreach (var record in records)
                {
                    var item = Mapper.Map<ShiftExchangeListViewModel>(record);
                    resultData.Add(item);
                }

            }
            var resultDto = new ResultDTO
            {
                Object = new ArrayResultDTO { Data = resultData, Count = totalItems }
            };
            return resultDto;
        }

        public async Task<ResultDTO> GetShiftExchange(QueryArgs arg, Guid currentUserId)
        {
            var resultDto = new ResultDTO();
            var param = arg.PredicateParameters[0].ToString();
            Guid id = new Guid(param);
            var shiftExchange = await _uow.GetRepository<ShiftExchangeApplication>().GetSingleAsync<ShiftExchangeViewByReferenceNumberViewModel>(x
                => x.Id == id);

            if (shiftExchange == null)
            {
                resultDto.Messages.Add("ID IS IN VALID");
                resultDto.ErrorCodes.Add(0);
            }
            else
            {
                var groupSapCodes = shiftExchange.ExchangingShiftItems.GroupBy(x => x.UserSAPCode);
                var shiftExchangeDetails = new List<ShiftExchangeDetailForAddOrUpdateViewModel>();
                foreach (var group in groupSapCodes)
                {
                    group.OrderBy(x => x.ShiftExchangeDate);
                    shiftExchangeDetails.AddRange(group.ToList().OrderBy(x => x.ShiftExchangeDate));
                }
                shiftExchange.ExchangingShiftItems = shiftExchangeDetails.OrderBy(x => x.UserSAPCode).ToList();
            }
            resultDto.Object = shiftExchange;
            return resultDto;
        }

        public async Task<ResultDTO> SaveShiftExchange(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId)
        {
            var resultDto = new ResultDTO();
            List<string> newStatus = new List<string>() { Const.Status.draft, Const.Status.requestedToChange };
            bool isCreateShiftExchange = false;

            // validation model
            shiftExhangeArgs.TryValidateModel(resultDto);
            if (shiftExhangeArgs.Id == Guid.Empty)
            {
                shiftExhangeArgs.Status = "Draft";
                isCreateShiftExchange = true;
            }
            // có lỗi thì xuất kết thúc hàm và return
            if (resultDto.Messages.Count != 0)
            {
                goto Finish;
            }
            /*else if (string.IsNullOrEmpty(shiftExhangeArgs.Status) || newStatus.Contains(shiftExhangeArgs.Status))
            {
                CheckingResultCollection results = new CheckingResultCollection();
                results.AddRange(shiftExhangeArgs.ExchangingShiftItems.Select(esItem => esItem.Check_ShiftExchange_Valid(_uow, true)));
                if (!results.isSuccess())
                {
                    resultDto.ErrorCodes = new List<int>() { 1007 };
                    resultDto.Object = results;
                    goto Finish;
                }
            }*/

            CheckingResultCollection results = new CheckingResultCollection();
            results.AddRange(shiftExhangeArgs.ExchangingShiftItems.Select(esItem => esItem.Check_ShiftExchange_Valid(_uow, isCreateShiftExchange, true)));
            if (!results.isSuccess())
            {
                resultDto.ErrorCodes = new List<int>() { 1007 };
                resultDto.Object = results;
                goto Finish;
            }

            // validate target plan
            await this.ValidateShiftExchange(resultDto, shiftExhangeArgs);
            if (resultDto.ErrorCodes.Any())
            {
                goto Finish;
            }

            // cập nhật data

            await SaveShifExchangeToRepository(shiftExhangeArgs, currentUserId, resultDto);

            await _uow.CommitAsync();

        Finish:

            return resultDto;
        }

        // AEON_658
        public async Task<ResultDTO> CheckTargetPlanComplete(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId)
        {
            var resultDto = new ResultDTO();
            List<string> newStatus = new List<string>() { Const.Status.draft, Const.Status.requestedToChange };

            // validation model
            shiftExhangeArgs.TryValidateModel(resultDto);
            if (shiftExhangeArgs.Id == Guid.Empty)
            {
                shiftExhangeArgs.Status = "Draft";
            }
            // có lỗi thì xuất kết thúc hàm và return
            if (resultDto.Messages.Count != 0)
            {
                goto Finish;
            }
            else if (string.IsNullOrEmpty(shiftExhangeArgs.Status) || newStatus.Contains(shiftExhangeArgs.Status))
            {
                CheckingResultCollection results = new CheckingResultCollection();
                results.AddRange(shiftExhangeArgs.ExchangingShiftItems.Select(esItem => esItem.Check_ShiftExchangeComplete_Valid(_uow, true)));
                if (!results.isSuccess())
                {
                    resultDto.ErrorCodes = new List<int>() { 1007 };
                    resultDto.Object = results;
                    goto Finish;
                }
            }
        Finish:

            return resultDto;
        }
        public async Task<ResultDTO> SubmitShiftExchange(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId)
        {
            var resultDto = new ResultDTO();
            // validation model
            shiftExhangeArgs.TryValidateModel(resultDto);

            // có lỗi thì xuất kết thúc hàm và return
            if (resultDto.Messages.Count != 0)
            {
                goto Finish;
            }

            // cập nhật data
            await SaveShifExchangeToRepository(shiftExhangeArgs, currentUserId, resultDto);

            #region Đoạn này dành cho workflow

            #endregion

            await _uow.CommitAsync();

        Finish:

            return resultDto;
        }

        async Task SaveShifExchangeToRepository(ShifExchangeForAddOrUpdateViewModel shiftExhangeArgs, Guid currentUserId, ResultDTO resultDto)
        {
            // add
            if (shiftExhangeArgs.Id == Guid.Empty)
            {
                var nShiftExchange = Mapper.Map<ShiftExchangeApplication>(shiftExhangeArgs);
                _uow.GetRepository<ShiftExchangeApplication>().Add(nShiftExchange);
                resultDto.Object = Mapper.Map<ShifExchangeForAddOrUpdateViewModel>(nShiftExchange);
            }
            //update
            else
            {
                var shiftExchange = await FindShiftExchangeById(shiftExhangeArgs.Id, currentUserId, resultDto);
                //Mapper.Map(shiftExhangeArgs, shiftExchange);
                shiftExchange.ApplyDate = shiftExhangeArgs.ApplyDate;
                shiftExchange.DeptLineId = shiftExhangeArgs.DeptLineId;
                shiftExchange.DeptDivisionId = shiftExhangeArgs.DeptDivisionId;
                var shiftExchangeDetail = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync(x => x.ShiftExchangeApplicationId == shiftExchange.Id);
                _uow.GetRepository<ShiftExchangeApplicationDetail>().Delete(shiftExchangeDetail);
                foreach (var item in shiftExhangeArgs.ExchangingShiftItems)
                {
                    var itemDetail = new ShiftExchangeApplicationDetail
                    {
                        ShiftExchangeApplicationId = shiftExchange.Id,
                        UserId = item.UserId,
                        ShiftExchangeDate = item.ShiftExchangeDate,
                        CurrentShiftCode = item.CurrentShiftCode,
                        CurrentShiftName = item.CurrentShiftName,
                        NewShiftCode = item.NewShiftCode,
                        NewShiftName = item.NewShiftName,
                        ReasonCode = item.ReasonCode,
                        ReasonName = item.ReasonName,
                        OtherReason = item.OtherReason,
                        IsERD = item.IsERD,
                    };
                    _uow.GetRepository<ShiftExchangeApplicationDetail>().Add(itemDetail);
                }
                _uow.GetRepository<ShiftExchangeApplication>().Update(shiftExchange);
                resultDto.Object = Mapper.Map<ShifExchangeForAddOrUpdateViewModel>(shiftExchange);
            }
        }

        public async Task<ShiftExchangeApplication> FindShiftExchangeById(Guid id, Guid currentUserId, ResultDTO resultDto)
        {
            //var shiftExchange = await _uow.GetRepository<ShiftExchangeApplication>().GetSingleAsync(x => x.Id == id && x.CreatedById == currentUserId);
            var shiftExchange = await _uow.GetRepository<ShiftExchangeApplication>().GetSingleAsync(x => x.Id == id);
            resultDto.Object = shiftExchange;
            if (shiftExchange == null)
            {
                resultDto.Messages.Add("ID IS IN VALID");
                resultDto.ErrorCodes.Add(0);
            }
            return shiftExchange;
        }

        public async Task<ResultDTO> GetShiftExchanges(QueryArgs args, Guid currentUserId)
        {

            // args.AddPredicate("", currentUserId);
            var items = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync<ShiftExchangeViewByReferenceNumberViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);

            var totalItems = await _uow.GetRepository<ShiftExchangeApplication>().CountAsync(args.Predicate, args.PredicateParameters);

            var resultDto = new ResultDTO
            {
                Object = new ArrayResultDTO { Data = items, Count = totalItems }
            };
            return resultDto;
        }
        public async Task<ResultDTO> GetShiftExchangeDetailById(Guid id)
        {

            // args.AddPredicate("", currentUserId);
            var items = await _uow.GetRepository<ShiftExchangeApplicationDetail>().FindByAsync<ShiftExchangeDetailSimpleViewModel>(x => x.ShiftExchangeApplicationId == id);

            var totalItems = items.Count();

            var resultDto = new ResultDTO
            {
                Object = new ArrayResultDTO { Data = items, Count = totalItems }
            };
            return resultDto;
        }

        public async Task<ResultDTO> ValidateERDShiftExchangeDetail(List<ValidateERDShiftExchangeViewModel> shiftExhangeArgs)
        {
            var result = new ResultDTO();
            List<string> SAPCode = new List<string>();
            List<string> statusTicket = new List<string> { "Completed", "Draft", "Rejected", "Cancelled" };
            List<string> checkNewShift = new List<string> { "ERD", "ERD1", "ERD2" };
            var shiftExchangeInProgress = await _uow.GetRepository<ShiftExchangeApplication>().FindByAsync<ShiftExchangeViewByReferenceNumberViewModel>(x
                => !statusTicket.Contains(x.Status));
            if (shiftExchangeInProgress.Any())
            {
                var arrayFilterNewShift = new List<ShiftExchangeDetailForAddOrUpdateViewModel>();
                foreach (var x in shiftExhangeArgs)
                {
                    foreach (var item in shiftExchangeInProgress)
                    {
                        var itemHaveNewShiftERD = item.ExchangingShiftItems.Where(y => checkNewShift.Contains(y.NewShiftCode) && y.UserSAPCode == x.SAPCode);
                        if (itemHaveNewShiftERD.Any())
                        {
                            arrayFilterNewShift.AddRange(itemHaveNewShiftERD);
                        }
                    }
                    if (arrayFilterNewShift.Any())
                    {
                        float total = x.SumNewShiftERD;
                        foreach (var item in arrayFilterNewShift)
                        {
                            total += ParseValue(item.NewShiftCode);
                        };
                        if (total > x.QuotaERD)
                        {
                            SAPCode.Add(x.SAPCode);
                        }
                    }
                }
                result.Object = new ArrayResultDTO { Data = SAPCode, Count = SAPCode.Count() };
            }
            return result;
        }

        private float ParseValue(string newShiftCode)
        {
            float result = 0;
            switch (newShiftCode)
            {
                case "ERD":
                    result = 1;
                    break;
                case "ERD1":
                    result = (float)(1) / 2;
                    break;
                case "ERD2":
                    result = (float)(1) / 2;
                    break;
                default:
                    break;
            }
            return result;
        }

        public async Task<ArrayResultDTO> GetCurrentShiftCodeFromShiftPlan(List<CurrentShiftArg> shiftExhangeArgs)
        {
            var result = new ArrayResultDTO();
            var data = new List<object>();
            var minShiftDate = shiftExhangeArgs.Min(x => x.ShiftExchangeDate);
            var maxShiftDate = shiftExhangeArgs.Max(x => x.ShiftExchangeDate);
            var sapCodes = shiftExhangeArgs.Select(x => x.SAPCode);
            var tempLeaves = await _uow.GetRepository<LeaveApplicationDetail>(true).FindByAsync(x => x.LeaveApplication.Status == "Completed" && sapCodes.Contains(x.LeaveApplication.UserSAPCode));
            List<LeaveApplicationDetail> leaves = tempLeaves.ToList();
            #region Get revoke leaves
            /*var revokeLeaveApplications = sapCodes.ToList().GetRevokeLeaveApplicationDetails(_uow, minShiftDate, maxShiftDate, true);
            if (revokeLeaveApplications.Any())
            {
                leaves.AddRange(revokeLeaveApplications);
            }*/
            #endregion
            var tempShiftExchanges = await _uow.GetRepository<ShiftExchangeApplicationDetail>(true).FindByAsync(x => x.ShiftExchangeApplication.Status == "Completed" && sapCodes.Contains(x.User.SAPCode));
            List<ShiftExchangeApplicationDetail> shiftExchanges = tempShiftExchanges.ToList();
            #region Get revoke shiftExchanges
            /*var revokeShiftExchangeApplications = sapCodes.ToList().GetRevokeShiftExchangeApplicationDetails(_uow, minShiftDate.ToLocalTime(), maxShiftDate.ToLocalTime(), true);
            if (revokeShiftExchangeApplications.Any())
            {
                shiftExchanges.AddRange(revokeShiftExchangeApplications);
            }*/
            #endregion
            var allTargetPlans = await _uow.GetRepository<TargetPlan>(true).FindByAsync<TargetPlanViewModel>(x => x.PeriodFromDate <= minShiftDate && x.PeriodToDate >= maxShiftDate
            && (x.Status == "Completed" || x.Status == "Approved"));
            var allTargetPlanDetails = await _uow.GetRepository<TargetPlanDetail>(true).FindByAsync<TargetPlanDetailViewModel>(x => sapCodes.Contains(x.SAPCode) && (x.TargetPlan.Status == "Completed" || x.TargetPlan.Status == "Approved"));
            if (allTargetPlanDetails.Any())
            {
                foreach (var shiftArg in shiftExhangeArgs)
                {
                    var currentTargetPlanDetails = allTargetPlanDetails.Where(x => x.SAPCode == shiftArg.SAPCode && x.PeriodFrom <= shiftArg.ShiftExchangeDate && x.PeriodTo >= shiftArg.ShiftExchangeDate);
                    var code = GetCurrentShiftCodeFromTargetPlanDetail(currentTargetPlanDetails, shiftArg, leaves, shiftExchanges);
                    if (!string.IsNullOrEmpty(code))
                    {
                        data.Add(new { shiftArg.SAPCode, shiftArg.ShiftExchangeDate, CurrentCode = code });
                    }
                }
            }
            result.Data = data;
            result.Count = data.Count;
            return result;
        }
        private string GetCurrentShiftCodeFromTargetPlanDetail(IEnumerable<TargetPlanDetailViewModel> details, CurrentShiftArg shiftArg, IEnumerable<LeaveApplicationDetail> leaves, IEnumerable<ShiftExchangeApplicationDetail> shifts)
        {
            var code = "";
            var target1 = JsonConvert.DeserializeObject<List<DateValueArgs>>(details.Where(x => x.Type == TypeTargetPlan.Target1).Select(y => y.JsonData).FirstOrDefault());
            //var target2 = JsonConvert.DeserializeObject<List<DateValueArgs>>(details.Where(x => x.Type == TypeTargetPlan.Target2).Select(y => y.JsonData).FirstOrDefault());
            //code = target2.Where(x => x.Date == shiftArg.ShiftExchangeDate.DateTime.ToLocalTime().ToSAPFormat()).Select(y => y.Value).FirstOrDefault();

            //if (string.IsNullOrEmpty(code))
            //{
            //    code = target1.Where(x => x.Date == shiftArg.ShiftExchangeDate.DateTime.ToLocalTime().ToSAPFormat()).Select(y => y.Value).FirstOrDefault();
            //}
            code = target1.Where(x => x.Date == shiftArg.ShiftExchangeDate.DateTime.ToLocalTime().ToSAPFormat()).Select(y => y.Value).FirstOrDefault();
            var currentLeave = leaves.FirstOrDefault(x => x.LeaveApplication.UserSAPCode == shiftArg.SAPCode && x.FromDate <= shiftArg.ShiftExchangeDate && x.ToDate >= shiftArg.ShiftExchangeDate);

            var currentShift = shifts.OrderByDescending(x => x.Modified).FirstOrDefault(x => x.User.SAPCode == shiftArg.SAPCode && x.ShiftExchangeDate.DateTime.ToLocalTime().ToSAPFormat() == shiftArg.ShiftExchangeDate.DateTime.ToLocalTime().ToSAPFormat());
            if (currentLeave != null && currentShift != null)
            {
                if (currentLeave.Modified > currentShift.Modified)
                {
                    code = currentLeave.LeaveCode;
                }
                else
                {
                    code = currentShift.NewShiftCode;
                }
            }
            else if (currentLeave != null)
            {
                code = currentLeave.LeaveCode;
            }
            else if (currentShift != null)
            {
                code = currentShift.NewShiftCode;
            }
            return code;
        }

        private async Task ValidateShiftExchange(ResultDTO result, ShifExchangeForAddOrUpdateViewModel shiftExchangeDetails)
        {
            // HR-482
            await ValidatePRD(result, shiftExchangeDetails);

            // todo ....
            if (!result.ErrorCodes.Any())
            {
                // HR-1372 
                await ValidateShiftCodeV(result, shiftExchangeDetails);
            }

            if (!result.ErrorCodes.Any())
            {
                // Task #15979  
                await ValidateRuleHoliday(result, shiftExchangeDetails);
            }

            if (!result.ErrorCodes.Any() && shiftExchangeDetails.ExchangingShiftItems != null 
                && shiftExchangeDetails.ExchangingShiftItems.Any(x => ListWFHInvalid.Contains(x.NewShiftCode)))
            {
                await ValidateWFHInWeek(result, shiftExchangeDetails);
            }
        }

        public List<ShiftCode> GetShiftCodes(string shiftCode)
        {
            var shiftCodeTbl = _uow.GetRepository<ShiftCode>().FindBy(x => x.IsActive && x.Code.ToLower().Equals(shiftCode.ToLower()));
            return shiftCodeTbl.Any() ? shiftCodeTbl.ToList() : new List<ShiftCode>();
        }

        public bool IsExistShiftCodeTbl(string shiftCode)
        {
            bool returnValue = false;
            try
            {
                var shiftCodeTbl = _uow.GetRepository<ShiftCode>().GetSingle(x => x.IsActive && x.Code.ToLower().Equals(shiftCode.ToLower()));
                returnValue = shiftCodeTbl != null ? true : false;
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
        }


        private async Task ValidateShiftCodeV(ResultDTO result, ShifExchangeForAddOrUpdateViewModel shiftExchangeDetails)
        {
            HashSet<CheckingResult> errors = new HashSet<CheckingResult>();
            try
            {
                string userIgnorValidateStr = ConfigurationManager.AppSettings["UserIgnoreValidate"] != null ? ConfigurationManager.AppSettings["UserIgnoreValidate"] : "00406464";
                var userIgnorValidate = new List<string>() { };
                if (userIgnorValidate.Contains(","))
                {
                    userIgnorValidate = userIgnorValidateStr.Split(',').ToList();
                } else
                {
                    userIgnorValidate.Add(userIgnorValidateStr);
                }
                var savedErd = new HashSet<ValidationShiftExchangePRD>();
                foreach (var itemDetail in shiftExchangeDetails.ExchangingShiftItems)
                {
                    string currentSapCode = (!string.IsNullOrEmpty(itemDetail.UserSAPCode) ? itemDetail.UserSAPCode : itemDetail.EmployeeCode);
                    TargetPlanPeriod targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= itemDetail.ShiftExchangeDate && x.ToDate >= itemDetail.ShiftExchangeDate);
                    if (!(targetPlanPeriod is null))
                    {
                        var shiftExchangeDateStr = itemDetail.ShiftExchangeDate.ToLocalTime().ToString("dd/MM/yyyy");
                        if (!string.IsNullOrEmpty(itemDetail.CurrentShiftCode) && !string.IsNullOrEmpty(itemDetail.NewShiftCode))
                        {
                            if (!IsExistShiftCodeTbl(itemDetail.CurrentShiftCode))
                            {
                                errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode) , errorCode = string.Concat("Current Shift Code " ,itemDetail.CurrentShiftCode, " not yet defined!") });
                                continue;
                            }

                            if (!IsExistShiftCodeTbl(itemDetail.NewShiftCode))
                            {
                                errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode), errorCode = string.Concat("New Shift Code ", itemDetail.NewShiftCode, " not yet defined!") });
                                continue;
                            }

                            var currentShiftCodeList = GetShiftCodes(itemDetail.CurrentShiftCode);
                            ShiftCode currentShifCode = null;
                            if (currentShiftCodeList.Any())
                            {
                                currentShifCode = currentShiftCodeList.FirstOrDefault();
                            }

                            var newShiftCodeLists = GetShiftCodes(itemDetail.NewShiftCode);
                            ShiftCode newShifCode = null;
                            if (newShiftCodeLists.Any())
                            {
                                newShifCode = newShiftCodeLists.FirstOrDefault();
                                if (newShiftCodeLists.Any(x => x.Line == ShiftLine.HAFTDAY) && newShiftCodeLists.Any(x => x.Line == ShiftLine.FULLDAY))
                                    newShifCode = newShiftCodeLists.Where(x => x.Line == ShiftLine.HAFTDAY).FirstOrDefault();
                            }

                            // case + ca full khác đầu V -> ca half khác đầu V thì chặn
                            if (currentShifCode != null && !currentShifCode.Code.StartsWith("V") && currentShifCode.Line == ShiftLine.FULLDAY
                                && newShifCode != null && !newShifCode.Code.StartsWith("V") && newShifCode.Line == ShiftLine.HAFTDAY)
                            {
                                errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode), errorCode = "SHIFT_EXCHANGE_EROR_SHIFT_V", description = itemDetail.NewShiftCode });
                                continue;
                            }

                            // case + ca half khác đầu V -> ca full khác đầu V thì chặn
                            if (currentShifCode != null && !currentShifCode.Code.StartsWith("V") && currentShifCode.Line == ShiftLine.HAFTDAY
                                && !newShifCode.Code.StartsWith("V") && newShifCode.Line == ShiftLine.FULLDAY)
                            {
                                errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode), errorCode = "SHIFT_EXCHANGE_EROR_SHIFT_V", description = itemDetail.NewShiftCode });
                                continue;
                            }

                            ActualTargetPlanArg actualArgs = new ActualTargetPlanArg()
                            {
                                ListSAPCode = JsonConvert.SerializeObject(new List<string>() { currentSapCode }),
                                PeriodId = targetPlanPeriod.Id,
                                NotInShiftExchange = new List<Guid> { itemDetail.ShiftExchangeApplicationId.HasValue ? itemDetail.ShiftExchangeApplicationId.Value : Guid.Empty }
                            };

                            var getActualShiftPlan = await _targetPlanBO.GetActualShiftPlan(actualArgs);
                            var atualShiftCode = new ArrayResultDTO();
                            List<object> actualShift = new List<object>();
                            Dictionary<string, string> key = new Dictionary<string, string>();
                            Dictionary<string, string> keyAtual2 = new Dictionary<string, string>();
                            if (getActualShiftPlan.IsSuccess)
                            {
                                atualShiftCode = Mapper.Map<ArrayResultDTO>(getActualShiftPlan.Object);
                                if (!(atualShiftCode.Data is null))
                                {
                                    actualShift = Mapper.Map<List<object>>(atualShiftCode.Data);
                                    foreach (var item in actualShift)
                                    {
                                        string ob1 = item.GetPropertyValue("SAPCode").ToString();
                                        string ob2 = item.GetPropertyValue("Actual1").ToString();
                                        string ob3 = item.GetPropertyValue("Actual2").ToString();
                                        key.Add(ob1, ob2);
                                        keyAtual2.Add(ob1, ob3);
                                    }
                                }

                                TargetPlanDetailQueryArg targetPlanCompletedArgs = new TargetPlanDetailQueryArg()
                                {
                                    DepartmentId = shiftExchangeDetails.DeptLineId.Value,
                                    DivisionId = shiftExchangeDetails.DeptDivisionId,
                                    //SAPCodes = shiftExchangeDetails.ExchangingShiftItems.Select(x => !string.IsNullOrEmpty(x.UserSAPCode) ? x.UserSAPCode : x.EmployeeCode).ToList(),
                                    SAPCodes = new List<string>() { currentSapCode },
                                    PeriodId = targetPlanPeriod.Id
                                };
                                var targetPlanCompleted = await _targetPlanBO.GetTargetPlanDetailIsCompleted(targetPlanCompletedArgs);
                                var dataTGCompleted = new ArrayResultDTO();
                                List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
                                if (targetPlanCompleted.IsSuccess)
                                {
                                    dataTGCompleted = Mapper.Map<ArrayResultDTO>(targetPlanCompleted.Object);
                                    if (!(dataTGCompleted.Data is null))
                                        targetPlanDetails = Mapper.Map<List<TargetPlanArgDetail>>(dataTGCompleted.Data);
                                }

                                var currentTargetPlanTarget1 = targetPlanDetails.Where(x => x.SAPCode == currentSapCode && x.Type == TypeTargetPlan.Target1).FirstOrDefault();
                                List<TargetDateDetail> detailsTarget1 = JsonConvert.DeserializeObject<List<TargetDateDetail>>(currentTargetPlanTarget1.JsonData);

                                var currentTargetPlanTarget2 = targetPlanDetails.Where(x => x.SAPCode == currentSapCode && x.Type == TypeTargetPlan.Target2).FirstOrDefault();
                                List<TargetDateDetail> detailsTarget2 = JsonConvert.DeserializeObject<List<TargetDateDetail>>(currentTargetPlanTarget2.JsonData);


                                string shiftExchangeDate = itemDetail.ShiftExchangeDate.ToLocalTime().ToString("yyyyMMdd");
                                string fullShiftCode = "";
                                string valueJsonFullDay = "";
                                List<DateValueArgs> convertFullDay = new List<DateValueArgs>();
                                DateValueArgs datasFullDay = null;
                                // check case full date

                                fullShiftCode = detailsTarget1.Where(x => x.date == shiftExchangeDate).Select(p => p.value).FirstOrDefault();
                                if (key.TryGetValue(currentSapCode, out valueJsonFullDay))
                                {
                                    convertFullDay = JsonConvert.DeserializeObject<List<DateValueArgs>>(valueJsonFullDay);
                                    if (convertFullDay.Any())
                                    {
                                        datasFullDay = convertFullDay.Where(y => y.Date == shiftExchangeDate).ToList().FirstOrDefault();
                                        if (!(datasFullDay is null) && !string.IsNullOrEmpty(datasFullDay.Value))
                                            fullShiftCode = datasFullDay.Value;
                                    }
                                }

                                string halfShiftCode = "";
                                string valueJsonHalfDay = "";
                                List<DateValueArgs> convertHalfDay = new List<DateValueArgs>();
                                DateValueArgs datasHalfDay = null;
                                // check case full date
                                halfShiftCode = detailsTarget2.Where(x => x.date == shiftExchangeDate).Select(p => p.value).FirstOrDefault();
                                if (keyAtual2.TryGetValue(currentSapCode, out valueJsonHalfDay))
                                {
                                    convertHalfDay = JsonConvert.DeserializeObject<List<DateValueArgs>>(valueJsonHalfDay);
                                    datasHalfDay = convertHalfDay.Where(y => y.Date == shiftExchangeDate).ToList().FirstOrDefault();
                                    if (!(datasHalfDay is null) && !string.IsNullOrEmpty(datasHalfDay.Value))
                                        halfShiftCode = datasHalfDay.Value;
                                }

                                /*if (!string.IsNullOrEmpty(fullShiftCode) && fullShiftCode.StartsWith("V") && !string.IsNullOrEmpty(halfShiftCode) // có 2 shiftcode full/half
                                    && currentShifCode != null && currentShifCode.Code.StartsWith("V") && currentShifCode.Line == ShiftLine.FULLDAY
                                    && newShifCode != null && !newShifCode.Code.StartsWith("V") && newShifCode.Line == ShiftLine.FULLDAY
                                    && !userIgnorValidate.Contains(currentSapCode)
                                    )
                                {
                                    errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode), errorCode = "TARGET_PLAN_VALIDATE_HAS_HALF_DAY", description = itemDetail.NewShiftCode });
                                    continue;
                                }*/
                            }
                        }
                    }
                }

                if (errors.Any())
                {
                    result.ErrorCodes = new List<int> { 1001 };
                    result.Object = errors;
                }
            }
            catch (Exception e) { }
        }

        private async Task ValidateRuleHoliday(ResultDTO result, ShifExchangeForAddOrUpdateViewModel shiftExchangeDetails)
        {
            HashSet<CheckingResult> errors = new HashSet<CheckingResult>();
            var shiftCodeHoliday = await _uow.GetRepository<ShiftCode>().FindByAsync(x => x.IsHoliday && x.IsActive);
            try
            {
                var savedErd = new HashSet<ValidationShiftExchangePRD>();
                foreach (var itemDetail in shiftExchangeDetails.ExchangingShiftItems)
                {
                    DateTime shiftExchangeDate = itemDetail.ShiftExchangeDate.Date.ToLocalTime();
                    bool isHoliday = (shiftExchangeDate != DateTime.MinValue && shiftExchangeDate.IsPublicHoliday(_uow));
                    if (isHoliday)
                    {
                        string currentSapCode = (!string.IsNullOrEmpty(itemDetail.UserSAPCode) ? itemDetail.UserSAPCode : itemDetail.EmployeeCode);
                        var shiftExchangeDateStr = itemDetail.ShiftExchangeDate.ToLocalTime().ToString("dd/MM/yyyy");
                        if (!shiftCodeHoliday.Any(x => x.Code == itemDetail.NewShiftCode))
                        {
                            errors.Add(new CheckingResult() { message = string.Concat(shiftExchangeDateStr, " - ", currentSapCode), errorCode = "NEW_SHIFT_CODE_IS_NOT_HOLIDAY_SHIFT_CODE", description = " (" + itemDetail.NewShiftCode + ")" });
                            continue;
                        }
                    }
                }

                if (errors.Any())
                {
                    result.ErrorCodes = new List<int> { 1001 };
                    result.Object = errors;
                }
            }
            catch (Exception e) { }
        }

        private async Task ValidateWFHInWeek(ResultDTO result, ShifExchangeForAddOrUpdateViewModel shiftExchangeDetails)
        {
            HashSet<CheckingResult> errors = new HashSet<CheckingResult>();
            try
            {
                var savedErd = new HashSet<ValidationShiftExchangePRD>();
                foreach (var itemDetail in shiftExchangeDetails.ExchangingShiftItems)
                {
                    string currentSapCode = (!string.IsNullOrEmpty(itemDetail.UserSAPCode) ? itemDetail.UserSAPCode : itemDetail.EmployeeCode);
                    if (!string.IsNullOrEmpty(currentSapCode))
                    {
                        var currentUser = _uow.GetRepository<User>().GetSingle(x => x.SAPCode == currentSapCode && !x.IsDeleted && x.IsActivated, "created desc");
                        if (currentUser != null)
                        {
                            double countWFH = 0;
                            if (ListWFHInvalid.Contains(itemDetail.NewShiftCode))
                            {
                                this.CounWFH(true, itemDetail.NewShiftCode, ref countWFH);
                                var weekDayList = GetWeekDaysList(itemDetail.ShiftExchangeDate.Date); // ignore current day
                                if (weekDayList.Any())
                                {
                                    // tinh luoi hien tai cung tuan
                                    var currentExchangeDetailInWeekDay = shiftExchangeDetails.ExchangingShiftItems.Where(x => 
                                    x.ShiftExchangeDate != itemDetail.ShiftExchangeDate && currentSapCode == ((!string.IsNullOrEmpty(x.UserSAPCode) ? x.UserSAPCode : x.EmployeeCode))
                                    && weekDayList.Contains(x.ShiftExchangeDate));
                                    if (currentExchangeDetailInWeekDay.Any())
                                    {
                                        foreach (var item in currentExchangeDetailInWeekDay)
                                        {
                                            if (ListWFHInvalid.Contains(item.NewShiftCode))
                                            {
                                                this.CounWFH(true, item.NewShiftCode, ref countWFH);
                                            } else if (ListWFHInvalid.Contains(item.CurrentShiftCode))
                                            {
                                                this.CounWFH(false, item.CurrentShiftCode, ref countWFH);
                                            }
                                        }
                                    }
                                    // All day
                                    var allWeekDayList = GetAllWeekDaysList(itemDetail.ShiftExchangeDate.Date);
                                    var ignoreCheckActualShiftCodeWeekDay = shiftExchangeDetails.ExchangingShiftItems.Where(x => ListWFHInvalid.Contains(x.NewShiftCode) &&
                                    allWeekDayList.Any(y => y.Date == x.ShiftExchangeDate.Date) && currentSapCode == ((!string.IsNullOrEmpty(x.UserSAPCode) ? x.UserSAPCode : x.EmployeeCode)))
                                    .Select(y => y.ShiftExchangeDate).ToList();

                                    // tinh Actual hien tai
                                    foreach (var item in weekDayList)
                                    {
                                        if (!ignoreCheckActualShiftCodeWeekDay.Contains(item.Date))
                                        {
                                            var currentShift1 = currentUser.GetActualTarget1_ByDate(_uow, item.Date, true);
                                            if (currentShift1 != null && !string.IsNullOrEmpty(currentShift1.value))
                                            {
                                                this.CounWFH(true, currentShift1.value, ref countWFH);
                                            }
                                            var currentShift2 = currentUser.GetActualTarget2_ByDate(_uow, item.Date, true);
                                            if (currentShift2 != null && !string.IsNullOrEmpty(currentShift2.value))
                                            {
                                                this.CounWFH(false, currentShift2.value, ref countWFH);
                                            }
                                        }
                                    }

                                    // check jobgrade
                                    var currentJobgrade = _uow.GetRepository<UserDepartmentMapping>().GetSingle(x => x.IsHeadCount && x.UserId == currentUser.Id && x.Department != null && x.Department.JobGrade != null, "Created desc");
                                    /*if ((currentJobgrade.Department.JobGrade.Grade >= 4 && countWFH > 2) || (currentJobgrade.Department.JobGrade.Grade < 4 && countWFH > 1))
                                    {
                                        errors.Add(new CheckingResult() {  message = string.Concat(itemDetail.ShiftExchangeDate.ToString("dd/MM/yyyy"), " - ", currentSapCode), errorCode = "TARGET_PLAN_VALIDATE_WFH", description = " (" + itemDetail.NewShiftCode + ")" });
                                        continue;
                                    }*/
                                    #region ACR-80 Điều chỉnh rule Validate ngày WFH trên phiếu TARGET
                                    if (currentJobgrade.Department.JobGrade.MaxWFH != null)
                                    {
                                        if (countWFH > currentJobgrade.Department.JobGrade.MaxWFH)
                                        {
                                            errors.Add(new CheckingResult() { message = string.Concat(itemDetail.ShiftExchangeDate.ToString("dd/MM/yyyy"), " - ", currentSapCode), errorCode = "TARGET_PLAN_VALIDATE_WFH", description = " (" + itemDetail.NewShiftCode + ")" });
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if ((currentJobgrade.Department.JobGrade.Grade >= 4 && countWFH > 2) || (currentJobgrade.Department.JobGrade.Grade < 4 && countWFH > 1))
                                        {
                                            errors.Add(new CheckingResult() { message = string.Concat(itemDetail.ShiftExchangeDate.ToString("dd/MM/yyyy"), " - ", currentSapCode), errorCode = "TARGET_PLAN_VALIDATE_WFH", description = " (" + itemDetail.NewShiftCode + ")" });
                                            continue;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                if (errors.Any())
                {
                    result.ErrorCodes = new List<int> { 1001 };
                    result.Object = errors;
                }
            }
            catch (Exception e) { }
        }

        public void CounWFH(bool isAddWFH, string shiftCode, ref double countWFH)
        {
            switch (shiftCode)
            {
                case "WFH":
                    countWFH = (isAddWFH ? (countWFH + 1) : (countWFH - 1));
                    break;
                case "WFH1":
                case "WFH2":
                    countWFH = (isAddWFH ? (countWFH + 0.5) : (countWFH - 0.5));
                    break;
            }
        }

        public List<DateTimeOffset> GetWeekDaysList(DateTimeOffset targetDate)
        {
            List<DateTimeOffset> result = new List<DateTimeOffset>();
            DateTimeOffset monday = targetDate;
            while (monday.DayOfWeek != DayOfWeek.Monday)
            {
                monday = monday.AddDays(-1);
            }
            for (DateTimeOffset currentDate = monday; currentDate < monday.AddDays(7); currentDate = currentDate.AddDays(1))
            {
                if (currentDate != targetDate)
                    result.Add(currentDate);
            }
            return result;
        }

        public List<DateTimeOffset> GetAllWeekDaysList(DateTimeOffset targetDate)
        {
            List<DateTimeOffset> result = new List<DateTimeOffset>();
            DateTimeOffset monday = targetDate;
            while (monday.DayOfWeek != DayOfWeek.Monday)
            {
                monday = monday.AddDays(-1);
            }
            for (DateTimeOffset currentDate = monday; currentDate < monday.AddDays(7); currentDate = currentDate.AddDays(1))
            {
                result.Add(currentDate);
            }
            return result;
        }


        private async Task ValidatePRD(ResultDTO result, ShifExchangeForAddOrUpdateViewModel shiftExchangeDetails)
        {
            HashSet<CheckingResult> errors = new HashSet<CheckingResult>();
            try
            {
                 List<UserShiftCount> changePRD = TotalPRDCShiftExchange(shiftExchangeDetails);
                var savedErd = new HashSet<ValidationShiftExchangePRD>();
                shiftExchangeDetails.ExchangingShiftItems = shiftExchangeDetails.ExchangingShiftItems.OrderByDescending(x => x.CurrentShiftCode == "PRD").ToList();
                foreach (var itemDetail in shiftExchangeDetails.ExchangingShiftItems)
                {
                    if (!itemDetail.NewShiftCode.Equals("PRD") && !itemDetail.CurrentShiftCode.Equals("PRD")) continue;
                    string shiftExchangeDate = itemDetail.ShiftExchangeDate.ToLocalTime().ToString("yyyyMMdd");
                    TargetPlanPeriod targetPlanPeriod = await _uow.GetRepository<TargetPlanPeriod>().GetSingleAsync(x => x.FromDate <= itemDetail.ShiftExchangeDate && x.ToDate >= itemDetail.ShiftExchangeDate);
                    if (!(targetPlanPeriod is null))
                    {
                        var validPRD = new ValidationShiftExchangePRD();
                        string currentSapCode = (!string.IsNullOrEmpty(itemDetail.UserSAPCode) ? itemDetail.UserSAPCode : itemDetail.EmployeeCode);
                        TargetPlanDetailQueryArg targetPlanCompletedArgs = new TargetPlanDetailQueryArg()
                        {
                            DepartmentId = shiftExchangeDetails.DeptLineId.Value,
                            DivisionId = shiftExchangeDetails.DeptDivisionId,
                            //SAPCodes = shiftExchangeDetails.ExchangingShiftItems.Select(x => !string.IsNullOrEmpty(x.UserSAPCode) ? x.UserSAPCode : x.EmployeeCode).ToList(),
                            SAPCodes = new List<string>() { currentSapCode },
                            PeriodId = targetPlanPeriod.Id
                        };
                        var targetPlanCompleted = await _targetPlanBO.GetTargetPlanDetailIsCompleted(targetPlanCompletedArgs);
                        var dataTGCompleted = new ArrayResultDTO();
                        List<TargetPlanArgDetail> targetPlanDetails = new List<TargetPlanArgDetail>();
                        if (targetPlanCompleted.IsSuccess)
                        {
                            dataTGCompleted = Mapper.Map<ArrayResultDTO>(targetPlanCompleted.Object);
                            if (!(dataTGCompleted.Data is null))
                                targetPlanDetails = Mapper.Map<List<TargetPlanArgDetail>>(dataTGCompleted.Data);
                        }

                        ActualTargetPlanArg actualArgs = new ActualTargetPlanArg()
                        {
                            ListSAPCode = JsonConvert.SerializeObject(new List<string>() { currentSapCode }),
                            PeriodId = targetPlanPeriod.Id,
                            NotInShiftExchange = new List<Guid> { itemDetail.ShiftExchangeApplicationId.HasValue ? itemDetail.ShiftExchangeApplicationId.Value : Guid.Empty }
                        };

                        var getActualShiftPlan = await _targetPlanBO.GetActualShiftPlan(actualArgs);
                        var atualShiftCode = new ArrayResultDTO();
                        List<object> actualShift = new List<object>();
                        Dictionary<string, string> key = new Dictionary<string, string>();
                        Dictionary<string, string> keyAtual2 = new Dictionary<string, string>();
                        if (getActualShiftPlan.IsSuccess)
                        {
                            atualShiftCode = Mapper.Map<ArrayResultDTO>(getActualShiftPlan.Object);
                            if (!(atualShiftCode.Data is null))
                            {
                                actualShift = Mapper.Map<List<object>>(atualShiftCode.Data);
                                foreach (var item in actualShift)
                                {
                                    string ob1 = item.GetPropertyValue("SAPCode").ToString();
                                    string ob2 = item.GetPropertyValue("Actual1").ToString();
                                    string ob3 = item.GetPropertyValue("Actual2").ToString();
                                    key.Add(ob1, ob2);
                                    keyAtual2.Add(ob1, ob3);
                                }
                            }
                        }
                        if (!targetPlanDetails.Any()) return;

                        var currentShiftExchangDetail = shiftExchangeDetails.ExchangingShiftItems.Where(p => (string.IsNullOrEmpty(p.EmployeeCode) ? p.UserSAPCode : p.EmployeeCode).Equals(currentSapCode) && DateTimeOffset.Compare(p.ShiftExchangeDate, itemDetail.ShiftExchangeDate) == 0).FirstOrDefault();
                        if (!(currentShiftExchangDetail is null))
                        {
                            var currentTargetPlan = targetPlanDetails.Where(x => x.SAPCode == currentSapCode && x.Type == TypeTargetPlan.Target1).FirstOrDefault();
                            List<TargetDateDetail> details = JsonConvert.DeserializeObject<List<TargetDateDetail>>(currentTargetPlan.JsonData);
                            string fromDateStr = "";
                            string toDateStr = "";
                            foreach (var x in details)
                            {
                                if (x.value is null)
                                {
                                    toDateStr = x.date;
                                    continue;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(fromDateStr) && !string.IsNullOrEmpty(x.value))
                                        fromDateStr = x.date;

                                    string valueJson = "";
                                    List<DateValueArgs> convert = new List<DateValueArgs>();
                                    DateValueArgs datas = null;
                                    // check case full date
                                    if (key.TryGetValue(currentSapCode, out valueJson))
                                    {
                                        convert = JsonConvert.DeserializeObject<List<DateValueArgs>>(valueJson);
                                        if (convert.Any())
                                        {
                                            datas = convert.Where(y => y.Date == x.date).ToList().FirstOrDefault();
                                            if (!(datas is null) && !string.IsNullOrEmpty(datas.Value))
                                                x.value = datas.Value;
                                        }
                                    }
                                    // check case haft date
                                    if (x.value.ToLower().Equals("prd") && keyAtual2.TryGetValue(currentSapCode, out valueJson))
                                    {
                                        convert = JsonConvert.DeserializeObject<List<DateValueArgs>>(valueJson);
                                        datas = convert.Where(y => y.Date == x.date).ToList().FirstOrDefault();
                                        if (!(datas is null) && !string.IsNullOrEmpty(datas.Value))
                                            x.value = datas.Value;
                                    }
                                }
                            }

                            // GET DATA
                            int countPRD = details.Count(x => x.value.Equals("PRD"));
                            var fromDate = targetPlanPeriod.FromDate.DateTime;
                            var toDate = targetPlanPeriod.ToDate.DateTime;
                            if (!string.IsNullOrEmpty(fromDateStr))
                            {
                                fromDate = DateTime.ParseExact(fromDateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddDays(-1);
                            }
                            if (!string.IsNullOrEmpty(toDateStr))
                            {
                                toDate = DateTime.ParseExact(toDateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddDays(-1);
                            }
                            var countSunDay = CountDays(fromDate, toDate, DayOfWeek.Sunday);

                            // Condition
                            int conditionCountPRD = 0;
                            // add or update PRD
                            var savedErdList = savedErd.Where(x => x.SAPCode == currentSapCode && x.TargetPeriod == targetPlanPeriod.Id).ToList().FirstOrDefault();
                            // case substract if current shift eq PRD
                            if (!(savedErdList is null))
                            {
                                if (itemDetail.CurrentShiftCode == "PRD")
                                    savedErdList.PRD--;
                                else if (itemDetail.NewShiftCode == "PRD")
                                    savedErdList.PRD++;
                                conditionCountPRD = savedErdList.PRD;
                            }
                            else
                            {
                                validPRD.TargetPeriod = targetPlanPeriod.Id;
                                validPRD.PRD = countPRD;
                                if (itemDetail.CurrentShiftCode == "PRD")
                                    validPRD.PRD--;
                                else if (itemDetail.NewShiftCode == "PRD")
                                    validPRD.PRD++;
                                validPRD.SAPCode = currentSapCode;
                                savedErd.Add(validPRD);
                                conditionCountPRD = validPRD.PRD;
                            }

                            var isSpecial = await CheckSpecialCase(itemDetail.UserId);
                            var idUserChangePRD = changePRD.First(x => x.IdUser == itemDetail.UserId); //tìm con user và lấy số lượng prd thay đổi 
                            if (isSpecial == true)
                            {
                                //if ((conditionCountPRD) > countSunDay+1|| (conditionCountPRD) < countSunDay - 1)
                                if(idUserChangePRD.PRDCount+countPRD> countSunDay + 1|| idUserChangePRD.PRDCount + countPRD < countSunDay - 1)
                                {
                                    if (!errors.Any(n => n.message.Equals(currentSapCode)))
                                        errors.Add(new CheckingResult() { message = currentSapCode, errorCode = "TARGET_PLAN_VALIDATE_PRD_SPECIAL" });
                                }
                            }
                            else 
                            if (idUserChangePRD.PRDCount + countPRD > countSunDay)
                            {
                                if (!errors.Any(n => n.message.Equals(currentSapCode)))
                                    errors.Add(new CheckingResult() { message = currentSapCode, errorCode = "TARGET_PLAN_VALIDATE_PRD_SPECIAL" });
                            }
                            //if ((conditionCountPRD) > countSunDay)root
                            //{
                            //    if (!errors.Any(n => n.message.Equals(currentSapCode)))
                            //        errors.Add(new CheckingResult() { message = currentSapCode, errorCode = "TARGET_PLAN_VALIDATE_PRD" });
                            //}
                        }
                    }

                    if (errors.Any())
                    {
                        result.ErrorCodes = new List<int> { 1001 };
                        result.Object = errors;
                    }
                }
            }
            catch (Exception e) { }
        }

        private int CountDays(DateTime start, DateTime end, DayOfWeek selectedDay)
        {
            if (start.Date > end.Date)
            {
                return 0;
            }
            int totalDays = (int)end.Date.Subtract(start.Date).TotalDays;
            DayOfWeek startDay = start.DayOfWeek;
            DayOfWeek endDay = end.DayOfWeek;
            ///look if endDay appears before or after the selectedDay when we start from startDay.
            int startToEnd = (int)endDay - (int)startDay;
            if (startToEnd < 0)
            {
                startToEnd += 7;
            }
            int startToSelected = (int)selectedDay - (int)startDay;
            if (startToSelected < 0)
            {
                startToSelected += 7;
            }
            bool isSelectedBetweenStartAndEnd = startToEnd >= startToSelected;
            if (isSelectedBetweenStartAndEnd)
            {
                return totalDays / 7 + 1;
            }
            else
            {
                return totalDays / 7;
            }
        }
        #region specical casse cr 9.9
        private async Task<bool> CheckSpecialCase(Guid Id)// kiem tra user co phai case dac biet khong 
        {
            //var currentUser = await _uow.GetRepository<User>().GetSingleAsync(x => x.Id == _uow.UserContext.CurrentUserId);

            //var getDepartment = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(i => i.User.SAPCode == user.SAPCode && i.IsHeadCount);
            var getListDepartment = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(i => i.UserId == Id && i.IsHeadCount);// loc ra cac dept  co user la head count nen khi xuong duoi  khong can ktra dk headcount
            //List<UserDepartmentMapping> listdepartmentMapping = getListDepartment.ToList();

            if (getListDepartment == null)// khong la headcount cua phong ban nao
            {
                return false;
            }
            else // truong hop co list department
            {
                //for(int i = 0; i< listdepartmentMapping.Count(); i++)
                //{
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
                    }
                    catch (Exception e)
                    {

                    }
                    //}
                }
            }
            return false;
        }
        private List<UserShiftCount> TotalPRDCShiftExchange(ShifExchangeForAddOrUpdateViewModel data)
        {
            List<UserShiftCount> result = new List<UserShiftCount>();

            foreach (var currentShiftExchange in data.ExchangingShiftItems)
            {
                var dataExsit = result.Find((x) => x.IdUser == currentShiftExchange.UserId);
                if (dataExsit is null)
                {
                    result.Add(new UserShiftCount { IdUser = currentShiftExchange.UserId, PRDCount = 0 });
                }

               
                if (currentShiftExchange.CurrentShiftCode != null && currentShiftExchange.CurrentShiftCode == "PRD")
                {
                    var check = result.Find((x) => x.IdUser == currentShiftExchange.UserId);
                    if (!(check is null))
                    {
                        var check1 = result.IndexOf(check);
                        result[check1].PRDCount -= 1;
                    }

                }
               
                if (currentShiftExchange.NewShiftCode != null && currentShiftExchange.NewShiftCode == "PRD")
                {
                var check =    result.Find((x) => x.IdUser == currentShiftExchange.UserId);
                    if(!(check is null))
                    {
                        var check1=  result.IndexOf(check) ;
                        result[check1].PRDCount += 1;
                    }
                    
                }
            }
            return result;
        }
        private async Task<bool> RecursionDepartment(Department department)// kiem tra user co phai case dac biet khong  department hien tai la department dau tien hoac cha
        {
            bool isspecial = false;
            var fatherDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == department.ParentId);// tìm ra nó  tức là nó có cha nếu không tìm ra cha thì nó đã là ông nội 
            if (fatherDepartment != null) // nếu có cha thì sẽ ktra cha trong specical xem có tick include children 
            {
                var fatherIncludeChildren = await _uow.GetRepository<TargetPlanSpecialDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == fatherDepartment.Id);
                if (fatherIncludeChildren != null && fatherIncludeChildren.IsIncludeChildren == true)
                {
                    return true;
                }
                else if (fatherIncludeChildren != null && fatherIncludeChildren.IsIncludeChildren == false)
                {
                    return false;
                }
                else
                {
                    isspecial = await RecursionDepartment(fatherDepartment);
                }

            }
            return isspecial;

        }
        #endregion
    }
}
