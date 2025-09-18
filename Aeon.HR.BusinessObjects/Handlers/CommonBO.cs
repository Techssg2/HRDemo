using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Handlers.ExternalBO;
using AutoMapper;
using Aeon.HR.ViewModels;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class CommonBO : ICommonBO
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IWorkflowBO _workflowBO;

        protected readonly ITrackingBO _trackingBO;
        protected readonly IMasterDataB0 _masterDataB0;
        protected readonly IEmployeeBO _employeeBO;
        private readonly ITargetPlanBO _targetPlanBO;
        //private readonly ISettingBO _settingBO;
        //private readonly ITrackingHistoryBO _trackingHistoryBO;

        public CommonBO(IUnitOfWork uow, ILogger logger, IWorkflowBO workflowBO, ITrackingBO trackingBO, IMasterDataB0 masterDataB0, IEmployeeBO employeeBO, ITargetPlanBO targetPlanBO)
        {
            _uow = uow;
            _logger = logger;
            _workflowBO = workflowBO;
            _trackingBO = trackingBO;
            _masterDataB0 = masterDataB0;
            _employeeBO = employeeBO;
            _targetPlanBO = targetPlanBO;
        }

        public async Task<ResultDTO> DeleteItemById(Guid id)
        {
            try
            {
                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                var itemTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                foreach (var itemType in itemTypes)
                {
                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", itemType);
                    var item = (IWorkflowEntity)(await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", id));
                    if (item != null)
                    {

                        if (!string.IsNullOrEmpty(item.Status) && item.Status.Equals("draft", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (item is MissingTimeClock)
                            {
                                //remove missing time lock details firts Then remove parent item
                                MissingTimeClock currentMTC = (MissingTimeClock)item;
                                List<MissingTimeClockDetail> mtcDetails = _uow.GetRepository<MissingTimeClockDetail>().FindBy(currentItem => currentItem.MissingTimeClockId == currentMTC.Id).ToList();
                                int itemCount = mtcDetails.Count;
                                for (int i = itemCount - 1; i >= 0; i--)
                                {
                                    _uow.GetRepository<MissingTimeClockDetail>().Delete(mtcDetails[i]);
                                }
                                _uow.GetRepository<MissingTimeClock>().Delete(currentMTC);
                                await _uow.CommitAsync();
                            } else if (item is ShiftExchangeApplication)
                            {
                                //remove missing time lock details firts Then remove parent item
                                ShiftExchangeApplication currentSE = (ShiftExchangeApplication)item;
                                List<ShiftExchangeApplicationDetail> sEDetails = _uow.GetRepository<ShiftExchangeApplicationDetail>().FindBy(currentItem => currentItem.ShiftExchangeApplicationId == currentSE.Id).ToList();
                                int itemCount = sEDetails.Count;
                                for (int i = itemCount - 1; i >= 0; i--)
                                {
                                    _uow.GetRepository<ShiftExchangeApplicationDetail>().Delete(sEDetails[i]);
                                }
                                _uow.GetRepository<ShiftExchangeApplication>().Delete(currentSE);
                                await _uow.CommitAsync();
                            }
                            else if (item is ShiftExchangeApplication)
                            {
                                //remove missing time lock details firts Then remove parent item
                                ShiftExchangeApplication currentSE = (ShiftExchangeApplication)item;
                                List<ShiftExchangeApplicationDetail> sEDetails = _uow.GetRepository<ShiftExchangeApplicationDetail>().FindBy(currentItem => currentItem.ShiftExchangeApplicationId == currentSE.Id).ToList();
                                int itemCount = sEDetails.Count;
                                for (int i = itemCount - 1; i >= 0; i--)
                                {
                                    _uow.GetRepository<ShiftExchangeApplicationDetail>().Delete(sEDetails[i]);
                                }
                                _uow.GetRepository<ShiftExchangeApplication>().Delete(currentSE);
                                await _uow.CommitAsync();
                            }
                            else
                            {
                                DynamicInvoker.Invoke(repository, "Delete", item);
                                //var rData = await DynamicInvoker.InvokeAsync(repository, "CommitAsync");
                                await _uow.CommitAsync();
                            }
                        }
                        else
                        {
                            return new ResultDTO() { Messages = new List<string>() { "Status is not in draft" }, ErrorCodes = new List<int>() { 404 }, Object = false };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ResultDTO() { Messages = new List<string>() { ex.Message }, ErrorCodes = new List<int>() { 404 }, Object = false };
            }

            return new ResultDTO() { Object = true };
        }

        public async Task<ResultDTO> UpdateStatusByReferenceNumber(UpdateStatusArgs args)
        {
            var result = new ResultDTO() { };
            bool isError = false;
            try
            {
                if (args is null)
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Param is invalid!" };
                }
                else if (string.IsNullOrEmpty(args.Status))
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Status is require!" };
                }
                else if (!(new List<string>() { Const.Status.completed, Const.Status.cancelled }).Contains(args.Status))
                {
                    result.ErrorCodes = new List<int> { -1 };
                    result.Messages = new List<string> { "Status is invalid!" };
                }

                if (isError)
                    goto Finish;

                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                var itemTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                foreach (var itemType in itemTypes)
                {
                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", itemType);
                    var item = (IWorkflowEntity)(await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", args.Id));
                    if (!(item is null))
                    {
                        item.Status = args.Status;
                        var workflowInstaces = await _uow.GetRepository<WorkflowInstance>(true).FindByAsync(x => x.ItemId == args.Id);
                        if (workflowInstaces != null)
                        {
                            foreach (var instance in workflowInstaces)
                            {
                                instance.IsITUpdate = true;
                            }

                        }
                        await _uow.CommitAsync();
                        await this.UpdateWorkflowByItemId(args.Id, args.Status, args.Comment);
                        // revoke item
                        if (item.Status == "Cancelled" && (args.ReferenceNumber.StartsWith("LEA-") || args.ReferenceNumber.StartsWith("SHI-")))
                        {
                            await this.Revoke(args.ReferenceNumber, args.Id, args.Status);
                        }
                        else if (item.Status == "Cancelled" && (args.ReferenceNumber.StartsWith("RES-")))
                        {
                            // delete payload 
                            var trackingLogInitData = await _uow.GetRepository<TrackingLogInitData>().GetSingleAsync(x => x.ReferenceNumber == args.ReferenceNumber);
                            if (trackingLogInitData != null)
                            {
                                var trackingRequest = await _uow.GetRepository<TrackingRequest>().GetSingleAsync(x => x.TrackingLogInitDatas.Any(y => y.TrackingLogId == trackingLogInitData.TrackingLogId));
                                if (trackingRequest != null)
                                {
                                    _uow.GetRepository<TrackingLogInitData>().Delete(trackingLogInitData);
                                    _uow.GetRepository<TrackingRequest>().Delete(trackingRequest);
                                    await _uow.CommitAsync();
                                }
                            } 
                        }
                        result.Object = args;
                        break; // thoat khoi vong lap khi da tim thay phieu
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = new ResultDTO() { Messages = new List<string>() { ex.Message }, ErrorCodes = new List<int>() { 404 }, Object = false };
            }
        Finish:
            return result;
        }

        public async Task<bool> UpdateWorkflowByItemId(Guid ItemId, string status, string comment)
        {
            bool rs = false;
            var voteType = status.Equals(Const.Status.completed) ? Infrastructure.Enums.VoteType.Approve : Infrastructure.Enums.VoteType.Cancel;
            var workflowInstace = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId == ItemId, "Created desc");
            if (!(workflowInstace is null))
            {
                workflowInstace.IsCompleted = true;
                // xoa khoi to do list
                var workflowTask = await _uow.GetRepository<WorkflowTask>(true).GetSingleAsync(x => x.ItemId == ItemId, "Created desc");
                if (!(workflowTask is null))
                {
                    workflowTask.IsCompleted = true;
                    workflowTask.Vote = voteType;
                }

                // update workflow histories
                var workflowHistories = await _uow.GetRepository<WorkflowHistory>(true).GetSingleAsync(x => x.InstanceId == workflowInstace.Id, "Created desc");
                if (!(workflowHistories is null))
                {
                    workflowHistories.IsStepCompleted = true;
                    workflowHistories.VoteType = voteType;
                    workflowHistories.Outcome = status;
                    workflowHistories.Comment = comment;
                    workflowHistories.Approver = _uow.UserContext.CurrentUserName;
                    workflowHistories.ApproverId = _uow.UserContext.CurrentUserId;
                    workflowHistories.ApproverFullName = _uow.UserContext.CurrentUserFullName;
                }
            }

            return rs;
        }

        public async Task Revoke(string referenceNumber, Guid itemId, string status)
        {
            IIntegrationEntity data = null;
            if (referenceNumber.StartsWith("LEA-") && status == "Cancelled")
            {
                data = await _uow.GetRepository<LeaveApplication>(true).FindByIdAsync(itemId);
            }
            else if (referenceNumber.StartsWith("SHI-") && status == "Cancelled")
            {
                data = await _uow.GetRepository<ShiftExchangeApplication>(true).FindByIdAsync(itemId);
            }
            if (!(data is null))
            {
                ExternalExcution excution = null;
                if (data is LeaveApplication leaveBalanceItem)
                {
                    excution = new LeaveBalanceBO(_logger, _uow, leaveBalanceItem, _trackingBO, _targetPlanBO);
                    excution.AdditionalItem = Mapper.Map<AdditionalItem>(leaveBalanceItem);
                }
                if (data is ShiftExchangeApplication shiftExchange)
                {
                    excution = new ShiftExchangeBO(_logger, _uow, shiftExchange, _trackingBO);
                    excution.AdditionalItem = Mapper.Map<AdditionalItem>(shiftExchange);
                }
                if (excution != null)
                {
                    excution.ConvertToPayload();
                    await excution.SubmitData(true);
                }
            }
        }

        public async Task<ResultDTO> GetItemById(Guid id)
        {
            try
            {
                var vReturn = new EntityItemViewModel();
                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                var itemTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                foreach (var itemType in itemTypes)
                {
                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", itemType);
                    var item = (IWorkflowEntity)(await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", id));
                    if (!(item is null))
                    {
                        vReturn.Status = item.Status;
                        vReturn.ReferenceNumber = item.GetPropertyValue("ReferenceNumber").ToString();
                        vReturn.Id = new Guid(item.GetPropertyValue("Id").ToString());
                        break;
                    }
                }
                return new ResultDTO() { Object = vReturn };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ResultDTO() { Messages = new List<string>() { ex.Message }, ErrorCodes = new List<int>() { 404 }, Object = false };
            }
        }
    }
}
