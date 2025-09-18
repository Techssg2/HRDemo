using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Constants;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Infrastructure.Utilities;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class WorkflowBO : IWorkflowBO
    {
        private const int MaxJobLevel = 9;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IDashboardBO _dashboardBO;
        private readonly IIntegrationExternalServiceBO _exBO;
        private readonly IEmailNotification _emailNotification;
        private readonly IEdoc01BO _edoc01;
        private readonly IFacilityBO _facilityBO;
        private readonly ITradeContractBO _tradeContractBO;
        private readonly ISKUBO _skuBO;
        private readonly ITrackingHistoryBO _trackingHistoryBO;
        private Guid? _refDeparmentId = null;
        private bool _isActionStartWorkflow = false;
        public WorkflowBO(ILogger logger, IUnitOfWork uow, IIntegrationExternalServiceBO exBO, IEmailNotification emailNotification, IDashboardBO dashboardBO, IEdoc01BO edoc01, IFacilityBO facilityBO, ITradeContractBO tradeContractBO, ISKUBO skuBO, ITrackingHistoryBO trackingHistoryBO)
        {
            _logger = logger;
            _uow = uow;
            _exBO = exBO;
            _emailNotification = emailNotification;
            _dashboardBO = dashboardBO;
            _edoc01 = edoc01;
            _facilityBO = facilityBO;
            _tradeContractBO = tradeContractBO;
            _skuBO = skuBO;
            _trackingHistoryBO = trackingHistoryBO;
        }
        public async Task<ResultDTO> GetRunningWorkflow()
        {
            try
            {
                var wfInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync<WorkflowInstanceListViewModel>(x => !x.IsCompleted);
                return new ResultDTO() { Object = wfInstances };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task GetDepartmentCode(IList<WorkflowHistoryViewModel> histories)
        {
            foreach (var his in histories)
            {
                if (his.AssignedToDepartmentId.HasValue)
                {
                    var department = await _uow.GetRepository<Department>().FindByIdAsync(his.AssignedToDepartmentId.Value);
                    if (!(department is null)) his.AssignedToUserCode = string.IsNullOrEmpty(department.Code) ? "" : department.Code;
                }
                else if (his.AssignedToUserId.HasValue)
                {
                    var user = await _uow.GetRepository<User>().FindByIdAsync(his.AssignedToUserId.Value);
                    if (!(user is null)) his.AssignedToUserCode = string.IsNullOrEmpty(user.SAPCode) ? "" : user.SAPCode;
                }
            }
        }

        public async Task<ResultDTO> GetWorkflowStatusByItemId(Guid itemId, bool force = false)
        {
            var workflowStatus = new WorkflowStatusViewModel();
            try
            {
                var item = await GetWorkflowItem(itemId);
                if (item != null)
                {
                    workflowStatus.CurrentStatus = item.Entity.Status;
                    workflowStatus.WorkflowComment = item.Entity.WorkflowComment;
                    var workflowInstances = await _uow.GetRepository<WorkflowInstance>().FindByAsync(x => x.ItemId == itemId, "Created desc", x => x.Histories);
                    var wfInstance = workflowInstances.FirstOrDefault();
                    workflowStatus.WorkflowInstances = Mapper.Map<List<WorkflowInstanceViewModel>>(workflowInstances);
                    if (workflowStatus.WorkflowInstances.Count > 0)
                    {
                        foreach (var instance in workflowStatus.WorkflowInstances)
                        {
                            if (instance.Histories.Any()) await this.GetDepartmentCode(instance.Histories);
                            // danh dau round hien tai workflow
                            if (!instance.IsCompleted)
                                instance.Histories.ToList().Select(x => { x.CurrentRound = true; return x; }).ToList();
                            instance.Histories = instance.Histories.OrderBy(x => x.StepNumber).ToList();
                        }
                        workflowStatus.LastHistory = workflowStatus.WorkflowInstances.FirstOrDefault().Histories.OrderByDescending(x => x.Created).FirstOrDefault();
                        workflowStatus.WorkflowName = wfInstance.WorkflowName;
                    }
                    if (item.Entity.CreatedById == _uow.UserContext.CurrentUserId && !string.IsNullOrEmpty(item.Entity.Status) && item.Entity.Status.Equals("draft", StringComparison.InvariantCultureIgnoreCase))
                    {
                        workflowStatus.AllowToDelete = true;
                    }
                    var wfTemplates = await GetWorkflowTemplates(item);
                    if (wfTemplates.Count > 0 && (wfInstance == null || wfInstance.IsTerminated || wfInstance.IsCompleted))
                    {
                        if (item.Entity.CreatedById == _uow.UserContext.CurrentUserId || force)
                        {
                            workflowStatus.AllowToStartWorflow = true;
                            workflowStatus.WorkflowButtons = new List<WorkflowButton>();
                            var matchedNonMultipleWf = wfTemplates.Where(x => !x.AllowMultipleFlow).OrderBy(x => x.Order).FirstOrDefault();
                            if (matchedNonMultipleWf != null && workflowStatus.AllowToStartWorflow)
                            {
                                List<string> statusAllowStartRevoke = new List<string>() { "completed", "revoke - request to change" };
                                var name = matchedNonMultipleWf.StartWorkflowButton;
                                if (string.IsNullOrEmpty(name))
                                {
                                    if (statusAllowStartRevoke.Contains(item.Entity.Status.ToLower()))
                                    {
                                        name = "Revoke";
                                    }
                                    else
                                    {
                                        name = "Send Request";
                                    }
                                }
                                workflowStatus.WorkflowButtons.Add(new WorkflowButton()
                                {
                                    Name = name,
                                    Id = matchedNonMultipleWf.Id
                                });

                            }
                            var matchedMultipleWfs = wfTemplates.Where(x => x.AllowMultipleFlow).OrderBy(x => x.Order).ToList();
                            foreach (var matchedMultipleWf in matchedMultipleWfs)
                            {
                                workflowStatus.WorkflowButtons.Add(new WorkflowButton()
                                {
                                    Name = matchedMultipleWf.StartWorkflowButton,
                                    Id = matchedMultipleWf.Id
                                });
                            }

                        }
                        return new ResultDTO()
                        {
                            Object = workflowStatus
                        };
                    }
                    else if (wfInstance == null)
                    {
                        return new ResultDTO()
                        {
                            Object = workflowStatus
                        };
                    }

                    if (!wfInstance.IsCompleted && item.Entity.CreatedById == _uow.UserContext.CurrentUserId)
                    {
                        workflowStatus.AllowToCancel = true;

                        // Task #16233: Ẩn nút "Cancel" đối với Requestor khi BTA đã Book khách sạn hoặc Book vé máy bay
                        if (wfInstance != null && !string.IsNullOrEmpty(wfInstance.ItemReferenceNumber) && wfInstance.ItemReferenceNumber.StartsWith("BTA-"))
                        {
                            var btaDetails = await _uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => x.BusinessTripApplicationId.HasValue && x.BusinessTripApplicationId == wfInstance.ItemId);
                            if (btaDetails != null)
                            {
                                bool isNotBookingFlight = btaDetails.Any(x => !x.IsCommitBooking);
                                bool isStayHotel = btaDetails.Any(x => x.BusinessTripApplication.StayHotel);
                                var btaApplication = btaDetails.Where(y => y.BusinessTripApplicationId != null && y.BusinessTripApplicationId.HasValue).Select(x => x.BusinessTripApplicationId).FirstOrDefault();
                                var bookingRoomDetail = await _uow.GetRepository<RoomOrganization>().FindByAsync(x => btaApplication != null && btaApplication.HasValue ? btaApplication.Value == x.BusinessTripApplicationId : false);
                                if ((!isNotBookingFlight) || (isStayHotel && bookingRoomDetail != null && bookingRoomDetail.Any()))
                                    workflowStatus.AllowToCancel = false;
                            }

                            //Hiển thị nút cancel cho phiếu BTA với round Changing, Revoke
                            if (wfInstance.WorkflowName.ToLower().StartsWith("bta changing") || wfInstance.WorkflowName.ToLower().StartsWith("bta revoke"))
                            {
                                workflowStatus.AllowToCancel = true;
                            }
                            else
                            {
                                //Neu wftemplate dat ten khac thi check trang thai cua workflowtask
                                var currentWf = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == wfInstance.Id, "Created asc");
                                if (currentWf != null)
                                {
                                    var statusBookingFlight_change = ("Waiting for Change Business Trip").Replace(" ", "").Trim().ToLower();
                                    var statusBookingFlight_revoke = ("Waiting for Revoke").Replace(" ", "").Trim().ToLower();
                                    if (currentWf.Status.Replace(" ", "").Trim().ToLower().Equals(statusBookingFlight_change) || currentWf.Status.Replace(" ", "").Trim().ToLower().Equals(statusBookingFlight_revoke))
                                    {
                                        workflowStatus.AllowToCancel = true;
                                    }
                                }
                            }
                        }
                    }
                    var lastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync<WorkflowHistoryViewModel>(x =>
                    x.InstanceId == wfInstance.Id
                    && (x.AssignedToUserId == _uow.UserContext.CurrentUserId
                    || x.AssignedToDepartment.UserDepartmentMappings.Any(t => t.UserId == _uow.UserContext.CurrentUserId
                    && (!t.IsDeleted && t.Role == x.AssignedToDepartmentType || (t.Department.IsPerfomance && (Group.Member == t.Role || Group.Checker == t.Role))))
                    ), "created desc");
                    if (lastHistory == null || lastHistory.IsStepCompleted)
                    {
                        return new ResultDTO()
                        {
                            Object = workflowStatus
                        };
                    }
                    var currentStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == lastHistory.StepNumber);
                    workflowStatus.AllowToVote = true;
                    workflowStatus.IgnoreValidation = wfInstance.WorkflowData.IgnoreValidation;
                    workflowStatus.ApproveFieldText = currentStep.SuccessVote;
                    workflowStatus.RejectFieldText = currentStep.FailureVote;
                    workflowStatus.AllowRequestToChange = currentStep.AllowRequestToChange;
                    workflowStatus.RestrictedProperties = currentStep.RestrictedProperties;
                    workflowStatus.IsCustomEvent = currentStep.IsCustomEvent;
                    workflowStatus.CustomEventKey = currentStep.CustomEventKey;
                    workflowStatus.IsCustomRequestToChange = currentStep.IsCustomRequestToChange;
                    workflowStatus.IsAttachmentFile = currentStep.IsAttachmentFile;
                }
                return new ResultDTO()
                {
                    Object = workflowStatus
                };
            }
            catch (Exception ex)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() { ex.Message }
                };
            }
        }

        public async Task<ResultDTO> GetWorkflowTemplateById(Guid id)
        {
            var wfTemplate = await _uow.GetRepository<WorkflowTemplate>().FindByIdAsync(id);
            return new ResultDTO() { Object = Mapper.Map<WorkflowTemplateViewModel>(wfTemplate) };
        }

        public ResultDTO GetAllItemTypes()
        {
            var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
            var workflowTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x)).Select(x => x.Name).ToArray();
            return new ResultDTO() { Object = workflowTypes };
        }

        public async Task<ResultDTO> GetWorkflowTemplates(QueryArgs args)
        {
            var wfTemplates = await _uow.GetRepository<WorkflowTemplate>()
                .FindByAsync<WorkflowTemplateListViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);
            var count = await _uow.GetRepository<WorkflowTemplate>().CountAsync(args.Predicate, args.PredicateParameters);
            ResultDTO result = new ResultDTO
            {
                Object = new ArrayResultDTO { Data = wfTemplates, Count = count }
            };
            return result;

            //.GetAllAsync<WorkflowTemplateListViewModel>("WorkflowName desc");
            //return new ResultDTO() { Object = wfTemplates };
        }

        public async Task<ResultDTO> StartWorkflow(Guid workflowTemplateId, Guid itemId, string comment = "")
        {
            _isActionStartWorkflow = true;
            //var isInWorkflow = await _uow.GetRepository<WorkflowInstance>().AnyAsync(x => x.TemplateId == workflowTemplateId && !x.IsCompleted);
            //if (isInWorkflow)
            //{
            //    return new ResultDTO()
            //    {
            //        Messages = new List<string>() {
            //        "Workflow is in progress"
            //        }
            //    };
            //}
            //Detect object
            #region Log nhận
                var receiveLogs = new HRTrackingAPILog() { Action = ActionAPIConstants.STARTWORKFLOW, Payload = "", Response = "This is log! " };
                receiveLogs.ItemId = itemId;
            #endregion

            var type = string.Empty;
            var item = await GetWorkflowItem(itemId);
            if (item == null)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() {
                        "Item not found!"

                    }
                };
            }
            //Found object, start workflow here
            WorkflowTemplate wfTemplate = await _uow.GetRepository<WorkflowTemplate>().FindByIdAsync(workflowTemplateId);
            //Jira 517 - Fix to WF get current value for starting WF
            if (!wfTemplate.AllowMultipleFlow)
            {
                //rescan and get new WF
                var wfTemplates = await GetWorkflowTemplates(item);
                if (wfTemplates != null && wfTemplates.Any())
                {
                    wfTemplate = wfTemplates.OrderBy(x => x.Order).FirstOrDefault();
                }
            }
            if (wfTemplate == null)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() {
                            "Workflow Template not found !"
                            }
                };
            }

            //Clone workflow instance
            var wfInstance = new WorkflowInstance()
            {
                TemplateId = wfTemplate.Id,
                WorkflowData = wfTemplate.WorkflowData,
                ItemId = itemId,
                WorkflowName = wfTemplate.WorkflowName,
                DefaultCompletedStatus = wfTemplate.DefaultCompletedStatus,
                ItemReferenceNumber = item.Entity.ReferenceNumber
            };
            _uow.GetRepository<WorkflowInstance>().Add(wfInstance);
            item.Entity.WorkflowComment = comment;
            await _uow.CommitAsync();
            await ProcessNextStep(wfInstance, item, null, wfTemplate.WorkflowData?.Steps?.FirstOrDefault());
            // dem so luong workflow
            var vReturn = Mapper.Map<WorkflowInstanceListViewModel>(wfInstance);
            vReturn.RoundNum = await _uow.GetRepository<WorkflowInstance>().CountAsync(x => x.ItemId == itemId);
            await PushNotificationForCurrentStep("Edoc2", "APPROVE", itemId);
            //luu log
            receiveLogs.Response = JsonConvert.SerializeObject(vReturn);
            _uow.GetRepository<HRTrackingAPILog>().Add(receiveLogs);
            await _uow.CommitAsync();

            return new ResultDTO() { Object = vReturn };
        }

        private async Task PushNotificationForCurrentStep(string module, string type, Guid itemId)
        {
            switch (type)
            {
                case "APPROVE":
                    _logger.LogInformation($"case PushNotificationForCurrentStep.APPROVE");
                    var findCurrentTask = await _uow.GetRepository<WorkflowTask>().FindByAsync(x => x.ItemId == itemId
                    && ((x.AssignedToDepartmentId != null) || (x.AssignedToId != null))
                    && !x.IsCompleted, "created desc");
                    if (findCurrentTask.Any())
                    {
                        foreach (var task in findCurrentTask)
                        {
                            if (task.AssignedToId != null && task.AssignedToId.HasValue)
                            {
                                PushNotification(module, task.AssignedToId.Value, task.ItemId, task.ReferenceNumber, "ITEM_IS_PROCESSING").GetAwaiter();
                            }
                            else if (task.AssignedToDepartmentId != null && task.AssignedToDepartmentId.HasValue)
                            {
                                var findUserDepartmentMappings = await _uow.GetRepository<UserDepartmentMapping>()
                                    .FindByAsync(x => x.DepartmentId == task.AssignedToDepartmentId.Value
                                    && ((x.Role == task.AssignedToDepartmentGroup)), "created desc");
                                if (findUserDepartmentMappings.Any())
                                {
                                    foreach (var udm in findUserDepartmentMappings)
                                    {
                                        if (udm.UserId == null)
                                            continue;

                                        PushNotification(module, udm.UserId.Value, task.ItemId, task.ReferenceNumber, "ITEM_IS_PROCESSING").GetAwaiter();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "RTC":
                    _logger.LogInformation($"case be PushNotificationForCurrentStep.RTC");
                    var findCurrentRound = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == itemId, "created desc");
                    if (findCurrentRound != null)
                    {
                        var findHistories = await _uow.GetRepository<WorkflowHistory>(true).GetSingleAsync(x => x.InstanceId == findCurrentRound.Id
                        && (x.StepNumber == 1 || x.Outcome.Equals("Submitted")), "created asc");
                        _logger.LogInformation($"case af PushNotificationForCurrentStep.RTC [{findHistories.AssignedToUserId}]");
                        if (findHistories != null && findHistories.AssignedToUserId != null)
                        {
                            //_logger.LogInformation($"PushNotificationForCurrentStep => {JsonConvert.SerializeObject(findHistories)}");
                            PushNotification(module, findHistories.AssignedToUserId.Value, itemId, findCurrentRound.ItemReferenceNumber, "ITEM_IS_PROCESSING").GetAwaiter();
                        }
                        else
                        {
                            _logger.LogInformation($"PushNotificationForCurrentStep => findHistories is null or findHistories.AssignedToUserId");
                        }
                    }
                    break;
            }
        }

        public async Task PushNotification(string module, Guid userId, Guid itemId, string referenceNumber, string message = "")
        {
            try
            {
                #region Config client
                var client = new HttpClient();
                string domain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                _logger.LogInformation($"Current Domain: {domain}");
                client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
                client.BaseAddress = new Uri(domain);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Requested-With", "X");
                #endregion

                var result = new ResultDTO();
                var url = "/api/CreateNotificationForApproval";
                var args = new
                {
                    Module = module,
                    UserId = userId,
                    ReferenceNumber = referenceNumber,
                    Message = message,
                    Type = "APPROVE",
                    MessageKey = "ITEM_IS_PROCESSING",
                    ItemId = itemId
                };
                var payload = JsonConvert.SerializeObject(args);
                payload = ReplaceNull(payload);
                var content = Aeon.HR.BusinessObjects.Helpers.Utilities.StringContentObjectFromJson(payload);
                string fullPath = string.Format("{0}/{1}", "EdocIntegr", url);
                // fullPath = "/api/CreateNotificationForApproval";
                var response = await client.PostAsync(fullPath, content);
                if (response.IsSuccessStatusCode)
                    _logger.LogInformation($"Response: {response.Content.ReadAsStringAsync()}");
                else
                    _logger.LogInformation($"Response: {response.Content.ReadAsStringAsync()}");
            } catch (Exception e)
            {
                _logger.LogInformation($"Ex: {e.Message}");
            }
        }

        public HttpClient HttpConfig()
        {
            #region Config client
            var client = new HttpClient();
            string domain = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
            _logger.LogInformation($"Current Domain: {domain}");
            client.Timeout = TimeSpan.FromSeconds(AppSettingsHelper.SAP_TimeOut);
            client.BaseAddress = new Uri(domain);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Requested-With", "X");
            return client;
            #endregion
        }

        public void ClearNotification(Guid itemId)
        {
            _logger.LogInformation($"Start.ClearNotification:");
            try
            {
                var client = HttpConfig();
                var result = new ResultDTO();
                var url = "/api/ClearNotification";
                var args = new
                {
                    ItemId = itemId
                };
                var payload = JsonConvert.SerializeObject(args);
                payload = ReplaceNull(payload);
                var content = Aeon.HR.BusinessObjects.Helpers.Utilities.StringContentObjectFromJson(payload);
                string fullPath = string.Format("{0}/{1}", "EdocIntegr", url);
                // fullPath = "/api/CreateNotificationForApproval";
                // Gửi yêu cầu mà không đợi kết quả
                _ = client.PostAsync(fullPath, content)
                    .ContinueWith(responseTask =>
                    {
                        try
                        {
                            if (responseTask.Result.IsSuccessStatusCode)
                                _logger.LogInformation($"Response: {responseTask.Result.Content.ReadAsStringAsync().Result}");
                            else
                                _logger.LogInformation($"Response: {responseTask.Result.Content.ReadAsStringAsync().Result}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation($"Ex: {ex.Message}");
                        }
                    });
            } catch (Exception e)
            {
                _logger.LogInformation($"Ex: {e.Message}");
            }
            _logger.LogInformation($"Start.ClearNotification:");
        }

        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }

        private async Task<List<WorkflowTemplate>> GetWorkflowTemplates(WorkflowItem item)
        {
            List<WorkflowTemplate> matchedWfTemplates = new List<WorkflowTemplate>();
            var wfTemplates = await _uow.GetRepository<WorkflowTemplate>().FindByAsync(x => x.ItemType == item.Type && x.IsActivated, "order asc");
            var itemPros = await ExtractItemProperties(item.Entity);
            foreach (var wfTemp in wfTemplates)
            {
                try
                {
                    await CheckOverwriteWorkflowData(item, itemPros, wfTemp);
                    if (wfTemp.WorkflowData != null)
                    {
                        var isValid = IsValidCondition(itemPros, wfTemp.WorkflowData.StartWorkflowConditions);
                        if (isValid)
                        {
                            matchedWfTemplates.Add(wfTemp);
                        }
                    }
                }
                catch
                {

                }
            }

            return matchedWfTemplates;
        }

        private async Task CheckOverwriteWorkflowData(WorkflowItem item, Dictionary<string, string> itemPros, WorkflowTemplate wfTemp)
        {
            if (wfTemp != null && wfTemp.WorkflowData != null && wfTemp.WorkflowData.OverwriteRequestedDepartment)
            {
                var rqDepartment = await GetRequestDepartment(item.Entity.CreatedById.Value, itemPros, wfTemp.WorkflowData);
                itemPros["isassistant"] = Convert.ToString(await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.UserId == item.Entity.CreatedById && x.DepartmentId == rqDepartment.Id && x.Role == Group.Assistance));
                itemPros["isstore"] = Convert.ToString(rqDepartment.IsStore);
                itemPros["ishr"] = Convert.ToString(rqDepartment.IsHR);
                //itemPros["requestedjobgrade"] = Convert.ToString(rqDepartment.JobGrade.Caption);
                itemPros["requestedjobgrade"] = Convert.ToString(rqDepartment.JobGrade.Grade);
                itemPros["requesteddepartment"] = Convert.ToString(rqDepartment.Name);
                itemPros["requesteddepartmentcode"] = Convert.ToString(rqDepartment.Code);
            }
        }

        //public async Task<ResultDTO> CancelWorkflow(Guid itemId)
        //{
        //    var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == itemId, "modified desc");
        //    if (wfInstance == null)
        //    {
        //        return new ResultDTO()
        //        {
        //            Messages = new List<string>() {
        //                "Workflow is not available!"

        //            }
        //        };
        //    }
        //    if (wfInstance.IsCompleted)
        //    {
        //        return new ResultDTO()
        //        {
        //            Messages = new List<string>() {
        //                "Workflow is completed!"

        //            }
        //        };
        //    }
        //    var item = await GetWorkflowItem(wfInstance.ItemId);
        //    if (item == null)
        //    {
        //        return new ResultDTO()
        //        {
        //            Messages = new List<string>() {
        //                "Item not found!"

        //            }
        //        };
        //    }
        //    item.Entity.Status = string.Empty;
        //    wfInstance.IsCompleted = true;
        //    wfInstance.IsTerminated = true;
        //    item.Entity.Status = "Cancelled";
        //    var lastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id, "modified desc");
        //    if (lastHistory != null)
        //    {
        //        lastHistory.Comment = "";
        //        lastHistory.Outcome = "Cancelled";
        //    }

        //    //revert perm to original

        //    var perms = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == itemId);
        //    foreach (var perm in perms)
        //    {
        //        perm.Perm = Right.View;
        //    }
        //    await _uow.CommitAsync();
        //    return new ResultDTO()
        //    { Object = true };
        //}

        public async Task<ResultDTO> TerminateWorkflowById(Guid WorkflowId)
        {
            var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == WorkflowId);
            if (wfInstance == null)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() {
                        "Workflow is not available!"

                    }
                };
            }
            if (wfInstance.IsCompleted)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() {
                        "Workflow is completed!"

                    }
                };
            }
            var item = await GetWorkflowItem(wfInstance.ItemId);
            if (item == null)
            {
                return new ResultDTO()
                {
                    Messages = new List<string>() {
                        "Item not found!"

                    }
                };
            }
            item.Entity.Status = string.Empty;
            wfInstance.IsCompleted = true;
            wfInstance.IsTerminated = true;

            //revert perm to original
            await RemoveItemPerm(item.Entity.Id);
            _uow.GetRepository<Permission>().Add(new Permission()
            {
                Perm = Right.Full,
                UserId = item.Entity.CreatedById,
                ItemId = item.Entity.Id
            });
            return new ResultDTO()
            { Object = true };
        }

        public async Task<ResultDTO> UpdateWorkflowTemplate(WorkflowTemplateViewModel args)
        {

            /*var currentWorkflow = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == args.Id);
            if (currentWorkflow is null)
            {
                return new ResultDTO() { Object = "Workflow Template not exists!" };
            }
            currentWorkflow.WorkflowName = args.WorkflowName;
            currentWorkflow.ItemType = args.ItemType;
            currentWorkflow.Order = args.Order;
            currentWorkflow.IsActivated = args.IsActivated;
            currentWorkflow.AllowMultipleFlow = args.AllowMultipleFlow;
            currentWorkflow.StartWorkflowButton = args.StartWorkflowButton;
            currentWorkflow.DefaultCompletedStatus = args.DefaultCompletedStatus;
            currentWorkflow.Created = args.Created;
            currentWorkflow.Modified = args.Modified;
            *//*currentWorkflow.WorkflowDataStr = args.WorkflowData*//*;
            *//*currentWorkflow.WorkflowData = Mapper.Map<WorkflowData>(args.WorkflowData);*//*
            currentWorkflow._data = Mapper.Map<WorkflowData>(args.WorkflowData);
            currentWorkflow.HasTrackingLog = true;
            _uow.GetRepository<WorkflowTemplate>().Update(currentWorkflow);
            var result = await _uow.CommitAsync();

            try
            {
                if (!(currentWorkflow is null))
                {
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(currentWorkflow),
                        ItemId = currentWorkflow.Id,
                        ItemType = ItemTypeContants.Workflow,
                        Type = TrackingHistoryTypeContants.Update
                    });
                }
            }
            catch (Exception e)
            {

            }
            var resultss = await _uow.CommitAsync();
            return new ResultDTO() { Object = result };*/

            /*var currentWorkflow = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == args.Id);
            var currentWorkflows = Mapper.Map<WorkflowTemplateViewModel>(currentWorkflow);*/
            var wfTemplate = Mapper.Map<WorkflowTemplate>(args);
            wfTemplate.HasTrackingLog = true;
            _uow.GetRepository<WorkflowTemplate>().Update(wfTemplate);
            try
            {
                var currentWorkflow = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == args.Id);
                if (!(currentWorkflow is null))
                    await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                    {
                        DataStr = JsonConvert.SerializeObject(Mapper.Map<WorkflowTemplate>(currentWorkflow)),
                        ItemId = wfTemplate.Id,
                        ItemType = ItemTypeContants.Workflow,
                        Type = TrackingHistoryTypeContants.Update
                    });
            }
            catch (Exception e) {}

            int result = await _uow.CommitAsync();
            return new ResultDTO() { Object = result };
        }

        public async Task<ResultDTO> ChangeStatusWorkflow(WorkflowTemplateViewModel args)
        {
            if (args != null && args.Id != null)
            {
                var workflowTemplate = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == args.Id);
                try
                {
                    if (!(workflowTemplate is null))
                    {
                        workflowTemplate.HasTrackingLog = true;
                        await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                        {
                            DataStr = JsonConvert.SerializeObject(workflowTemplate),
                            ItemId = workflowTemplate.Id,
                            ItemType = ItemTypeContants.Workflow,
                            Type = TrackingHistoryTypeContants.Update
                        });
                    }
                }
                catch (Exception e) { }
                workflowTemplate.IsActivated = args.IsActivated;
                _uow.GetRepository<WorkflowTemplate>().Update(workflowTemplate);
                var result = await _uow.CommitAsync();
                return new ResultDTO() { Object = result };
            }
            return new ResultDTO() { Object = false };
        }

        public async Task<ResultDTO> CreateWorkflowTemplate(WorkflowTemplateViewModel args)
        {
            var wfTemplate = Mapper.Map<WorkflowTemplate>(args);
            wfTemplate.IsActivated = true;
            wfTemplate.HasTrackingLog = true;
            _uow.GetRepository<WorkflowTemplate>().Add(wfTemplate);
            await _uow.CommitAsync();
            try
            {
                await _trackingHistoryBO.SaveTrackingHistory(new TrackingHistoryArgs()
                {
                    DataStr = JsonConvert.SerializeObject(wfTemplate),
                    ItemId = wfTemplate.Id,
                    ItemType = ItemTypeContants.Workflow,
                    Type = TrackingHistoryTypeContants.Create
                });
            }
            catch (Exception e) { }
            return new ResultDTO() { Object = wfTemplate };
        }

        public async Task AddMoreProperties(WorkflowEntity wfItem, string type, Guid ItemId)
        {
            try
            {
                if (type.Equals(ItemTypeContants.BusinessTripOverBudget))
                {
                    var bobEntity = await _uow.GetRepository<BusinessTripOverBudget>().GetSingleAsync(x => x.Id == ItemId);
                    if (bobEntity != null && bobEntity.BusinessTripApplication != null && bobEntity.BusinessTripApplicationId != null && bobEntity.BusinessTripApplicationId.HasValue)
                    {
                        var wfInstanceBookingFlight = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == bobEntity.BusinessTripApplicationId.Value, "created desc");
                        if (wfInstanceBookingFlight != null)
                        {
                            double? approvedDay = BTAHelper.GetBTAApprovedDay(wfInstanceBookingFlight.ItemId, _uow);
                            if (approvedDay != null && approvedDay.HasValue)
                            {
                                var parseIntValue = int.Parse(approvedDay.Value.ToString());
                                if (parseIntValue >= 10)
                                {
                                    wfItem.LargerThanOrEqual10Day = true;
                                }
                                else
                                {
                                    wfItem.LessThan10Day = true;
                                }
                            }
                            /*string statusBookingFlight = ("Waiting for Booking Flight").Replace(" ", "").Trim().ToLower();
                            var wfTaskInprocessBookingFlight = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == wfInstanceBookingFlight.Id && x.ItemId == wfInstanceBookingFlight.ItemId
                            && !string.IsNullOrEmpty(x.Status) && x.Status.Replace(" ", "").Trim().ToLower().Equals(statusBookingFlight) && x.AssignedToId != null && x.AssignedToId.HasValue
                            && !x.IsCompleted, "created desc");
                            if (wfTaskInprocessBookingFlight != null)
                            {
                                var btaDetail = await _uow.GetRepository<BusinessTripApplicationDetail>(true).FindByAsync(x => x.BusinessTripApplicationId == bobEntity.BusinessTripApplicationId);
                                DateTimeOffset lastFromDateBTADetail = btaDetail.Where(y => y.FromDate != null && y.FromDate.HasValue).OrderBy(x => x.FromDate).Select(y => y.FromDate.Value).FirstOrDefault();
                                double day = (lastFromDateBTADetail - wfTaskInprocessBookingFlight.Created).TotalDays;
                                if (day >= 10)
                                {
                                    wfItem.LargerThanOrEqual10Day = true;
                                }
                                else
                                {
                                    wfItem.LessThan10Day = true;
                                }
                            }*/
                        }
                    }
                }
            }
            catch (Exception e) { }
        }
        public async Task<ResultDTO> Vote(VoteArgs args)
        {
            var trackingLogAPI = new HRTrackingAPILog() { Action = ActionAPIConstants.VOTE, Payload = JsonConvert.SerializeObject(args) };
            trackingLogAPI.ItemId = args.ItemId;
            try
            {
                try
                {
                    #region Log nhận
                    var receiveLogs = new HRTrackingAPILog() { ItemId = args.ItemId, Action = ActionAPIConstants.VOTE, Payload = JsonConvert.SerializeObject(args), Response = "This is log! " };
                    _uow.GetRepository<HRTrackingAPILog>().Add(receiveLogs);
                    await _uow.CommitAsync();
                    #endregion
                }
                catch (Exception e) { }
             
                var item = await GetWorkflowItem(args.ItemId);
                if (item == null)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                        "Item not found!"

                    }
                    };
                }
                var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == args.ItemId, "Created desc");
                if (wfInstance.IsCompleted)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                        "Workflow is completed!"

                    }
                    };
                }
                var lastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id, "Created desc");
                if (lastHistory == null || lastHistory.IsStepCompleted)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                            "Cannot find any running workflow !"
                        }
                    };
                }

                var ignorePerm = args.Vote == VoteType.Cancel && item.Entity.CreatedById == _uow.UserContext.CurrentUserId;
                var currentTask = await _uow.GetRepository<WorkflowTask>(ignorePerm).GetSingleAsync(x => x.ItemId == item.Entity.Id && x.WorkflowInstanceId == wfInstance.Id && !x.IsCompleted, "Created desc");

                //If user cancel incase of pending, the tasks was not generated
                if (currentTask == null && args.Vote != VoteType.Cancel)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                            "Cannot find any running workflow !"
                        }
                    };
                }
                if (currentTask != null)
                {
                    currentTask.IsCompleted = true;
                    currentTask.Vote = args.Vote;
                }
                var currentStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == lastHistory.StepNumber);
                lastHistory.VoteType = args.Vote;
                lastHistory.Modified = DateTime.Now;
                lastHistory.Comment = args.Comment;
                lastHistory.ApproverId = _uow.UserContext.CurrentUserId;
                lastHistory.Approver = _uow.UserContext.CurrentUserName;
                lastHistory.ApproverFullName = _uow.UserContext.CurrentUserFullName;
                lastHistory.IsStepCompleted = true;
                WorkflowStep nextStep = null;
                if (args.Vote == VoteType.Approve)
                {
                    lastHistory.Outcome = currentStep.OnSuccess;
                    if (wfInstance != null && !string.IsNullOrEmpty(wfInstance.ItemReferenceNumber) && wfInstance.ItemReferenceNumber.StartsWith("BOB-"))
                    {
                        await this.AddMoreProperties(item.Entity, ItemTypeContants.BusinessTripOverBudget, wfInstance.ItemId);
                    }
                    nextStep = wfInstance.WorkflowData.Steps.GetNextStep(currentStep, item.Entity, item.Entity.GetType());
                }
                else if (args.Vote == VoteType.Reject)
                {
                    lastHistory.Outcome = currentStep.OnFailure;
                }
                else if (args.Vote == VoteType.Cancel)
                {
                    if (string.IsNullOrEmpty(wfInstance.WorkflowData.OnCancelled))
                    {
                        lastHistory.Outcome = "Cancelled";
                    }
                    else
                    {
                        lastHistory.Outcome = wfInstance.WorkflowData.OnCancelled;
                    }
                }
                await UpdatePermission(item.Entity.Id, Right.View);
                await _uow.CommitAsync();
                if (args.Vote == VoteType.RequestToChange)
                {
                    List<string> statusAllowStartRevoke = new List<string>() { "completed", "revoke - request to change" };
                    item.Entity.Status = !string.IsNullOrEmpty(wfInstance.WorkflowData.onRequestToChange) ? wfInstance.WorkflowData.onRequestToChange : "Requested To Change";
                    var exacStatus = !string.IsNullOrEmpty(wfInstance.WorkflowData.onRequestToChange) ? wfInstance.WorkflowData.onRequestToChange : "Requested To Change";
                    wfInstance.IsTerminated = true;
                    wfInstance.IsCompleted = true;
                    //Send email notification
                    await SendEmailNotificationForCreator(EmailTemplateName.ForCreatorRequestToChange, item);
                    if (currentStep.ReturnToStepNumber == 0)
                    {
                        if (item.Entity.Status.Equals("revoke - request to change", StringComparison.OrdinalIgnoreCase))
                        {
                            await AssignPermissionToUser(item.Entity.Id, item.Entity.CreatedById.Value, Right.View);
                        }
                        else
                        {
                            await AssignPermissionToUser(item.Entity.Id, item.Entity.CreatedById.Value, Right.Full);
                        }
                        await _uow.CommitAsync();
                        //SaveLogWorkflow(item.Entity.Id, item.Entity.Status, "After Check RTC");

                        /*if (!item.Entity.Status.Equals(exacStatus))
                        {
                            item.Entity.Status = !string.IsNullOrEmpty(wfInstance.WorkflowData.onRequestToChange) ? wfInstance.WorkflowData.onRequestToChange : "Requested To Change";
                            await _uow.CommitAsync();
                        }*/

                        await PushNotificationForCurrentStep("Edoc2", "RTC", wfInstance.ItemId);
                    }
                    else
                    {
                        var newInstance = new WorkflowInstance()
                        {
                            ItemId = wfInstance.ItemId,
                            ItemReferenceNumber = wfInstance.ItemReferenceNumber,
                            TemplateId = wfInstance.TemplateId,
                            WorkflowData = wfInstance.WorkflowData,
                            WorkflowName = wfInstance.WorkflowName,
                            DefaultCompletedStatus = wfInstance.DefaultCompletedStatus
                        };
                        _uow.GetRepository<WorkflowInstance>().Add(newInstance);
                        var histories = wfInstance.Histories.Where(x => x.StepNumber < currentStep.ReturnToStepNumber).ToList();
                        foreach (var history in histories)
                        {
                            var wfHistory = new WorkflowHistory()
                            {
                                Approver = history.Approver,
                                ApproverFullName = history.ApproverFullName,
                                ApproverId = history.ApproverId,
                                AssignedToDepartmentId = history.AssignedToDepartmentId,
                                AssignedToDepartmentType = history.AssignedToDepartmentType,
                                AssignedToUserId = history.AssignedToUserId,
                                Comment = history.Comment,
                                DueDate = history.DueDate,
                                InstanceId = newInstance.Id,
                                IsStepCompleted = history.IsStepCompleted,
                                Outcome = history.Outcome,
                                StepNumber = history.StepNumber,
                                VoteType = history.VoteType
                            };
                            _uow.GetRepository<WorkflowHistory>().Add(wfHistory);
                        }
                        await _uow.CommitAsync();
                        nextStep = newInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == currentStep.ReturnToStepNumber);
                        lastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id && x.StepNumber < currentStep.ReturnToStepNumber, "Created desc");

                        //AEON-2565
                        if (newInstance.ItemReferenceNumber.StartsWith("BTA-"))
                        {
                            var tempCurrentStep = newInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == lastHistory.StepNumber);
                            nextStep = newInstance.WorkflowData.Steps.GetNextStep(tempCurrentStep, item.Entity, item.Entity.GetType());
                        }

                        await RequestToChangeAction(item, currentStep.StepName);
                        await ProcessNextStep(newInstance, item, lastHistory, nextStep);
                        await PushNotificationForCurrentStep("Edoc2", "RTC", wfInstance.ItemId);
                    }
                }
                else
                {
                    await ProcessNextStep(wfInstance, item, lastHistory, nextStep);

                    #region Save Department of Next Step For Send Daily Mail
                    bool isResignationItem = typeof(ResignationApplication).IsAssignableFrom(item.Entity.GetType());
                    if (isResignationItem && currentStep.StepNumber == 1 && currentStep.OnSuccess.Equals("Submitted", StringComparison.OrdinalIgnoreCase)) // submit
                    {
                        WorkflowStep nxStep = wfInstance.WorkflowData.Steps.GetNextStep(nextStep, item.Entity, item.Entity.GetType());
                        await ProcessUpdateNextDepartment(wfInstance, item, lastHistory, nxStep);
                    }
                    #endregion

                    #region Push Acting to SAP at step Acting Employee Confirmation
                    bool isActingItem = typeof(Acting).IsAssignableFrom(item.Entity.GetType());
                    if (isActingItem && currentStep.OnSuccess.Equals("Accepted Target", StringComparison.OrdinalIgnoreCase))
                    {
                        await _exBO.SubmitData(item.Entity, true);
                    }
                    if (!_isActionStartWorkflow)
                    {
                        await PushNotificationForCurrentStep("Edoc2", "APPROVE", wfInstance.ItemId);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                trackingLogAPI.Response = JsonConvert.SerializeObject(ex.Message);
                _uow.GetRepository<HRTrackingAPILog>().Add(trackingLogAPI);
                await _uow.CommitAsync();
                return new ResultDTO()
                {
                    Messages = new List<string>() { ex.Message }
                };
            }

            trackingLogAPI.Response = JsonConvert.SerializeObject(new { Object = true });
            _uow.GetRepository<HRTrackingAPILog>().Add(trackingLogAPI);
            await _uow.CommitAsync();
            return new ResultDTO()
            {
                Object = true
            };
        }

        /*private void SaveLogWorkflow(Guid ItemId, string status, string message)
        {
            var newItem = new ItemWorkflowLog()
            {
                ItemId = ItemId,
                Status = status,
                Message = message
            };
            _uow.GetRepository<ItemWorkflowLog>().Add(newItem);
        }*/

        private async Task ProcessNextStep(WorkflowInstance wfInstance, WorkflowItem wfItem, WorkflowHistory lastHistory, WorkflowStep nextStep)
        {
            try
            {
                var item = wfItem.Entity;
                if (nextStep == null)
                {
                    await CompleteWorkflow(wfItem, wfInstance, lastHistory);
                    this.ClearNotification(wfInstance.ItemId);
                }
                else
                {
                    var lastOutcome = lastHistory == null ? string.Empty : lastHistory.Outcome;
                    await DoCustomAction(wfItem, lastOutcome);
                    Guid? userId = null;
                    Guid? departmentId = null;
                    var itemPros = await ExtractItemProperties(item);
                    var rqDepartment = await GetRequestDepartment(item.CreatedById.Value, itemPros, wfInstance.WorkflowData, nextStep);
                    //Get next user/ dept
                    switch (nextStep.ParticipantType)
                    {
                        case ParticipantType.SpecificUser:
                            var user = await _uow.GetRepository<User>().FindByIdAsync(nextStep.UserId.Value);
                            if (!(user == null || !user.IsActivated))
                            {
                                userId = user.Id;
                                departmentId = nextStep.DepartmentId;
                            }
                            break;
                        case ParticipantType.CurrentDepartment:
                            Department dept = null;
                            if (lastHistory == null)
                            {
                                //Get current dept of user
                                dept = rqDepartment;
                            }
                            else
                            {
                                dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == lastHistory.AssignedToDepartmentId);
                            }
                            if (dept != null)
                            {
                                var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == dept.Id && nextStep.DepartmentType == x.Role);
                                if (hasParticipants)
                                {
                                    departmentId = dept.Id;
                                }
                                else
                                {
                                    _refDeparmentId = dept.Id;
                                }
                            }
                            break;
                        case ParticipantType.SpecificDepartment:
                            var specDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == nextStep.UserId.Value);
                            if (specDept != null)
                            {
                                departmentId = specDept.Id;
                            }
                            break;
                        case ParticipantType.UpperDepartment:
                            Department upperDept = await GetUpperDept(lastHistory, nextStep, rqDepartment);
                            if (upperDept != null)
                            {
                                departmentId = upperDept.Id;
                            }
                            break;
                        case ParticipantType.DepartmentLevel:
                            Department lvlDept = await GetLevelDept(lastHistory, nextStep, rqDepartment);
                            if (lvlDept != null)
                            {
                                departmentId = lvlDept.Id;
                            }
                            break;
                        case ParticipantType.ItemUserField:
                            Guid userGuid;
                            if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                            {
                                if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out userGuid))
                                {
                                    userId = userGuid;
                                }
                            }
                            break;
                        case ParticipantType.ItemDepartmentField:
                            Guid departmentGuid;
                            if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                            {

                                if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out departmentGuid))
                                {
                                    departmentId = departmentGuid;
                                }
                            }
                            break;
                        case ParticipantType.HRDepartment:
                            Department hrDepartment = await GetHrDept(lastHistory, nextStep, rqDepartment);
                            if (hrDepartment != null)
                            {
                                departmentId = hrDepartment.Id;
                            }
                            break;
                        case ParticipantType.HRManagerDepartment:
                            Department hrManagerDepartment = GetHrManagerDept(lastHistory, nextStep, rqDepartment);
                            if (hrManagerDepartment != null)
                            {
                                departmentId = hrManagerDepartment.Id;
                            }
                            break;
                        case ParticipantType.PerfomanceDepartment:
                            Department perfDepartment = await GetPerfDept(lastHistory, nextStep, rqDepartment);
                            if (perfDepartment != null)
                            {
                                departmentId = perfDepartment.Id;
                            }
                            break;
                        case ParticipantType.AdminDepartment:
                            Department adminDepartment = await GetAdminDept(lastHistory, nextStep, rqDepartment);
                            if (adminDepartment != null)
                            {
                                departmentId = adminDepartment.Id;
                            }
                            break;
                        case ParticipantType.CBDepartment:
                            Department cbDepartment = await GetCBDept(lastHistory, nextStep, rqDepartment);
                            if (cbDepartment != null)
                            {
                                departmentId = cbDepartment.Id;
                            }
                            break;
                        case ParticipantType.Appraiser1Department:
                            Department appraiser1Department = await GetAppraiser1Department(lastHistory, nextStep, rqDepartment);
                            if (appraiser1Department != null)
                            {
                                departmentId = appraiser1Department.Id;
                            }
                            break;
                        case ParticipantType.Appraiser2Department:
                            Department appraiser2Department = await GetAppraiser2Department(lastHistory, nextStep, rqDepartment);
                            if (appraiser2Department != null)
                            {
                                departmentId = appraiser2Department.Id;
                            }
                            break;
                        case ParticipantType.BTABudgetApprover:
                            rqDepartment = await this.GetBeforeStepBookingFlightBTA(item.Id);
                            upperDept = await GetBTAUpperDept(null, nextStep, rqDepartment);
                            if (upperDept != null)
                            {
                                departmentId = upperDept.Id;
                            }
                            break;
                    }

                    //Start next step
                    await UpdateItemStatus(nextStep, item, userId, departmentId);
                    //Create historical data
                    CreateHistoricalData(wfInstance, nextStep, userId, departmentId);
                    await AssignTaskAndPermission(wfInstance, wfItem, nextStep, item, userId, departmentId, rqDepartment);
                    await _uow.CommitAsync();

                    //Check if this step can be skipped
                    if (!nextStep.PreventAutoPopulate)
                    {
                        await AutoPopulateNextStep(wfInstance, wfItem, lastHistory, nextStep, item, userId, departmentId);
                    }

                    //Send email to admin checker when cancellation booking flight
                    if ((nextStep.CustomEventKey == "addChangingCancellationFee" || nextStep.CustomEventKey == "addCancellationFee"))
                    {
                        await SendEmaiAdminChecker(EmailTemplateName.BTASendEmailWhenNextStepAdminChecker, wfInstance, nextStep, departmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot complete next step, ex: {0}", ex.Message);
                throw ex;
            }
        }

        private async Task<Department> GetBeforeStepBookingFlightBTA(Guid ItemId)
        {
            Department returnDepartment = null;
            var overBudget = await _uow.GetRepository<BusinessTripOverBudget>(true).GetSingleAsync(x => x.Id == ItemId, "Created desc");
            if (overBudget != null && overBudget.BusinessTripApplicationId != null && overBudget.BusinessTripApplicationId.HasValue)
            {
                Guid BTAId = overBudget.BusinessTripApplicationId.Value;
                //Guid BTAId = ItemId;
                string stepBookingFlight = "Booking Flight"; // check vote success
                var wfInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.ItemId == BTAId, "created desc");
                if (wfInstance != null)
                {
                    int stepNum = wfInstance.WorkflowData.Steps.Where(x => x.SuccessVote.Replace(" ", "").ToLower().Equals(stepBookingFlight.Replace(" ", "").ToLower())).Select(i => i.StepNumber).FirstOrDefault();
                    var workflowHistories = await _uow.GetRepository<WorkflowHistory>(true).FindByAsync(x => x.InstanceId == wfInstance.Id, "created desc");
                    if (workflowHistories != null)
                    {
                        var beforeStepBTA = workflowHistories.Where(x => x.StepNumber < stepNum).OrderByDescending(y => y.StepNumber).FirstOrDefault();
                        if (beforeStepBTA != null)
                        {
                            returnDepartment = beforeStepBTA.AssignedToDepartment;
                        }
                    }
                }
            }
            return returnDepartment;
        }

        private async Task ProcessUpdateNextDepartment(WorkflowInstance wfInstance, WorkflowItem wfItem, WorkflowHistory lastHistory, WorkflowStep nextStep)
        {
            try
            {
                var item = wfItem.Entity;
                var itemPros = await ExtractItemProperties(item);
                var rqDepartment = await GetRequestDepartment(item.CreatedById.Value, itemPros, wfInstance.WorkflowData, nextStep);
                //Get next user/ dept
                Guid? nextDepartmentId = await GetNextDepartment(nextStep, rqDepartment, lastHistory, itemPros);
                if (nextDepartmentId.HasValue)
                {
                    Group departmentType = getNextDepartmentType(nextStep, nextDepartmentId);

                    // save table
                    EmailHistory processingEmail = new EmailHistory
                    {
                        ItemId = wfInstance.ItemId,
                        ReferenceNumber = wfInstance.ItemReferenceNumber,
                        DepartmentType = departmentType,
                        DepartmentId = nextDepartmentId,
                        ItemType = wfItem.Type
                    };
                    _uow.GetRepository<EmailHistory>().Add(processingEmail);
                }
                await _uow.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot complete next step, ex: {0}", ex.Message);
                throw ex;
            }
        }

        public Group getNextDepartmentType(WorkflowStep nextStep, Guid? departmentId)
        {
            Func<Group, bool> DoesHaveParticipants = (Group groupType) =>
            {
                bool checkStatus = _uow.GetRepository<UserDepartmentMapping>().Any(x => x.DepartmentId == departmentId.Value && (nextStep.ParticipantType == ParticipantType.HRDepartment && ((x.User.Role & UserRole.HR) == UserRole.HR || (x.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin)) && groupType == x.Role);
                return checkStatus;
            };

            var customDepartmentType = nextStep.DepartmentType;
            bool hasParticipants = DoesHaveParticipants(nextStep.DepartmentType);
            if (hasParticipants)
            {
                customDepartmentType = nextStep.DepartmentType;
            }
            else
            {
                customDepartmentType = DoesHaveParticipants(nextStep.NextDepartmentType) ? nextStep.NextDepartmentType : nextStep.DepartmentType;
            }
            return customDepartmentType;
        }
        public async Task<Guid?> GetNextDepartment(WorkflowStep nextStep, Department rqDepartment, WorkflowHistory lastHistory, Dictionary<string, string> itemPros)
        {
            Guid? departmentId = new Guid?();
            switch (nextStep.ParticipantType)
            {
                case ParticipantType.SpecificUser:
                    var user = await _uow.GetRepository<User>().FindByIdAsync(nextStep.UserId.Value);
                    if (!(user == null || !user.IsActivated))
                    {
                        departmentId = nextStep.DepartmentId;
                    }
                    break;
                case ParticipantType.CurrentDepartment:
                    Department dept = null;
                    if (lastHistory == null)
                    {
                        //Get current dept of user
                        dept = rqDepartment;
                    }
                    else
                    {
                        dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == lastHistory.AssignedToDepartmentId);
                    }
                    if (dept != null)
                    {
                        var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == dept.Id && nextStep.DepartmentType == x.Role);
                        if (hasParticipants)
                        {
                            departmentId = dept.Id;
                        }
                        else
                        {
                            _refDeparmentId = dept.Id;
                        }
                    }
                    break;
                case ParticipantType.SpecificDepartment:
                    var specDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == nextStep.UserId.Value);
                    if (specDept != null)
                    {
                        departmentId = specDept.Id;
                    }
                    break;
                case ParticipantType.UpperDepartment:
                    Department upperDept = await GetUpperDept(lastHistory, nextStep, rqDepartment);
                    if (upperDept != null)
                    {
                        departmentId = upperDept.Id;
                    }
                    break;
                case ParticipantType.DepartmentLevel:
                    Department lvlDept = await GetLevelDept(lastHistory, nextStep, rqDepartment);
                    if (lvlDept != null)
                    {
                        departmentId = lvlDept.Id;
                    }
                    break;
                /*case ParticipantType.ItemUserField:
                    Guid userGuid;
                    if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                    {
                        if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out userGuid))
                        {
                            userId = userGuid;
                        }
                    }
                    break;*/
                case ParticipantType.ItemDepartmentField:
                    Guid departmentGuid;
                    if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                    {

                        if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out departmentGuid))
                        {
                            departmentId = departmentGuid;
                        }
                    }
                    break;
                case ParticipantType.HRDepartment:
                    Department hrDepartment = await GetHrDept(lastHistory, nextStep, rqDepartment);
                    if (hrDepartment != null)
                    {
                        departmentId = hrDepartment.Id;
                    }
                    break;
                case ParticipantType.HRManagerDepartment:
                    Department hrManagerDepartment = GetHrManagerDept(lastHistory, nextStep, rqDepartment);
                    if (hrManagerDepartment != null)
                    {
                        departmentId = hrManagerDepartment.Id;
                    }
                    break;
                case ParticipantType.PerfomanceDepartment:
                    Department perfDepartment = await GetPerfDept(lastHistory, nextStep, rqDepartment);
                    if (perfDepartment != null)
                    {
                        departmentId = perfDepartment.Id;
                    }
                    break;
                case ParticipantType.AdminDepartment:
                    Department adminDepartment = await GetAdminDept(lastHistory, nextStep, rqDepartment);
                    if (adminDepartment != null)
                    {
                        departmentId = adminDepartment.Id;
                    }
                    break;
                case ParticipantType.CBDepartment:
                    Department cbDepartment = await GetCBDept(lastHistory, nextStep, rqDepartment);
                    if (cbDepartment != null)
                    {
                        departmentId = cbDepartment.Id;
                    }
                    break;
                case ParticipantType.Appraiser1Department:
                    Department appraiser1Department = await GetAppraiser1Department(lastHistory, nextStep, rqDepartment);
                    if (appraiser1Department != null)
                    {
                        departmentId = appraiser1Department.Id;
                    }
                    break;
                case ParticipantType.Appraiser2Department:
                    Department appraiser2Department = await GetAppraiser2Department(lastHistory, nextStep, rqDepartment);
                    if (appraiser2Department != null)
                    {
                        departmentId = appraiser2Department.Id;
                    }
                    break;
            }
            return departmentId;
        }

        private async Task UpdateItemStatus(WorkflowStep nextStep, WorkflowEntity item, Guid? userId, Guid? departmentId)
        {
            if (!(departmentId.HasValue || userId.HasValue))
            {
                item.Status = $"Pending";
                if (!nextStep.IgnoreIfNoParticipant)
                {
                    //Send notification email to administrator to assign approver;
                }
            }
            else
            {
                item.Status = $"Waiting for {nextStep.StepName}";
                if (departmentId.HasValue && !nextStep.IsStatusFollowStepName &&
                    (nextStep.StepName != "Appraiser 1" && nextStep.StepName != "Appraiser 2"))
                {
                    var nextDepartment = await _uow.GetRepository<Department>().FindByIdAsync(departmentId.Value);
                    item.Status = $"Waiting for {nextDepartment.PositionName} Approval";
                }
            }
        }

        private void CreateHistoricalData(WorkflowInstance wfInstance, WorkflowStep nextStep, Guid? userId, Guid? departmentId)
        {
            Func<Group, bool> DoesHaveParticipants = (Group groupType) =>
            {
                bool checkStatus = _uow.GetRepository<UserDepartmentMapping>().Any(x => x.DepartmentId == departmentId.Value && (nextStep.ParticipantType == ParticipantType.HRDepartment && ((x.User.Role & UserRole.HR) == UserRole.HR || (x.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin)) && groupType == x.Role);
                return checkStatus;
            };

            var customDepartmentType = nextStep.DepartmentType;
            bool hasParticipants = DoesHaveParticipants(nextStep.DepartmentType);
            if (hasParticipants)
            {
                customDepartmentType = nextStep.DepartmentType;
            }
            else
            {
                customDepartmentType = DoesHaveParticipants(nextStep.NextDepartmentType) ? nextStep.NextDepartmentType : nextStep.DepartmentType;
            }

            var historyItem = new WorkflowHistory()
            {
                DueDate = DateTime.Now.AddDays(nextStep.DueDateNumber),
                InstanceId = wfInstance.Id,
                AssignedToUserId = userId,
                AssignedToDepartmentId = departmentId.HasValue ? departmentId : _refDeparmentId,
                AssignedToDepartmentType = customDepartmentType,
                StepNumber = nextStep.StepNumber
            };
            _uow.GetRepository<WorkflowHistory>().Add(historyItem);
        }

        private async Task AssignTaskAndPermission(WorkflowInstance wfInstance, WorkflowItem wfItem, WorkflowStep nextStep, WorkflowEntity item, Guid? userId, Guid? departmentId, Department rqDepartment)
        {
            var permissions = new List<Permission>();
            //Delete all current permission
            await RemoveItemPerm(item.Id);
            var wfHistories = await _uow.GetRepository<WorkflowHistory>().FindByAsync(x => x.Instance.ItemId == wfItem.Entity.Id);
            AssignItemPermToPreviousStep(item, wfHistories, nextStep, permissions);
            if (userId.HasValue || departmentId.HasValue)
            {
                Func<Group, bool> DoesHaveParticipants = (Group groupType) =>
                {
                    bool checkStatus = _uow.GetRepository<UserDepartmentMapping>().Any(x => x.DepartmentId == departmentId.Value && (nextStep.ParticipantType == ParticipantType.HRDepartment && ((x.User.Role & UserRole.HR) == UserRole.HR || (x.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin)) && groupType == x.Role);
                    return checkStatus;
                };

                //Ignore task list and new permission for appover
                //Create new task list
                var hasParticipants = true;
                var customDepartmentType = nextStep.DepartmentType;

                if (departmentId.HasValue)
                {
                    hasParticipants = DoesHaveParticipants(nextStep.DepartmentType);
                    if (hasParticipants)
                    {
                        customDepartmentType = nextStep.DepartmentType;
                    }
                    else
                    {
                        customDepartmentType = DoesHaveParticipants(nextStep.NextDepartmentType) ? nextStep.NextDepartmentType : nextStep.DepartmentType;
                    }

                    var deprt = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == departmentId);
                    if (!hasParticipants && deprt.IsPerfomance)
                    {
                        if (nextStep.DepartmentType == Group.Checker)
                        {
                            nextStep.DepartmentType = Group.Member;
                        }
                        else
                            nextStep.DepartmentType = Group.Checker;

                    }
                }

                var newTask = new WorkflowTask()
                {
                    DueDate = DateTime.Now.AddDays(nextStep.DueDateNumber),
                    Title = item.ReferenceNumber,
                    ItemId = item.Id,
                    WorkflowInstanceId = wfInstance.Id,
                    Status = item.Status,
                    ItemType = wfItem.Type,
                    RequestorId = item.Id,
                    RequestorFullName = item.CreatedByFullName,
                    RequestorUserName = item.CreatedBy,
                    RequestedDepartmentId = rqDepartment?.Id,
                    RequestedDepartmentCode = rqDepartment?.Code,
                    RequestedDepartmentName = rqDepartment?.Name,
                    ReferenceNumber = item.ReferenceNumber,
                    AssignedToId = userId,
                    AssignedToDepartmentId = departmentId,
                    AssignedToDepartmentGroup = customDepartmentType,
                    IsTurnedOffSendNotification = nextStep.IsTurnedOffSendNotification
                };
                nextStep.DepartmentType = customDepartmentType;
                _uow.GetRepository<WorkflowTask>().Add(newTask);
                //Assign permission for Task
                AssignTaskPerm(newTask, nextStep, userId, departmentId, permissions);
                //Assign permission for Item
                AssignItemPermForNextStep(item, nextStep, userId, departmentId, permissions);

            }
            else if (!nextStep.IgnoreIfNoParticipant)
            {
                //Send email notification to notify that the workflow cannot find the participant;
            }
            _uow.GetRepository<Permission>().Add(permissions);
        }

        private async Task AutoPopulateNextStep(WorkflowInstance wfInstance, WorkflowItem wfItem, WorkflowHistory lastHistory, WorkflowStep nextStep, WorkflowEntity item, Guid? userId, Guid? departmentId)
        {
            if (userId.HasValue || departmentId.HasValue)
            {
                bool autoNext = await IsAutoNext(lastHistory, nextStep, item, userId, departmentId);
                if (autoNext)
                {
                    // await IgnoreStep(wfInstance, nextStep, false);
                    await Vote(new VoteArgs()
                    {
                        Vote = VoteType.Approve,
                        Comment = wfItem.Entity.WorkflowComment,
                        ItemId = item.Id
                    });
                }
            }
            else if (nextStep.IgnoreIfNoParticipant)
            {
                //await IgnoreStep(wfInstance, nextStep, true);
                var newNextStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == nextStep.StepNumber + 1);
                var newLastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id, "Created desc");
                await ProcessNextStep(wfInstance, wfItem, newLastHistory, newNextStep);
            }
        }

        //private async Task IgnoreStep(WorkflowInstance wfInstance, WorkflowStep nextStep, bool isCompleted)
        //{
        //    if (nextStep.HiddenIfIgnore)
        //    {
        //        var currentHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id && !x.IsStepCompleted, "created desc");
        //        currentHistory.IsIgnore = true;
        //        currentHistory.IsStepCompleted = isCompleted;
        //        await _uow.CommitAsync();
        //    }
        //}

        private async Task<bool> IsAutoNext(WorkflowHistory lastHistory, WorkflowStep nextStep, WorkflowEntity item, Guid? userId, Guid? departmentId)
        {
            var autoNext = false;
            if (nextStep.RestrictedProperties == null || nextStep.RestrictedProperties.Count == 0)
            {
                //Auto approve to next step if user is the same 
                if (lastHistory != null && lastHistory.VoteType == VoteType.Approve)
                {
                    autoNext = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(t => t.DepartmentId == departmentId && t.UserId == lastHistory.ApproverId && t.Role == nextStep.DepartmentType);
                    if (!autoNext)
                    {
                        autoNext = userId.HasValue && lastHistory.ApproverId == userId;
                    }
                }
                else if (lastHistory == null)
                {
                    if (userId.HasValue && _uow.UserContext.CurrentUserId == userId)
                    {
                        autoNext = true;
                    }
                    else
                    {
                        autoNext = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(t => t.DepartmentId == departmentId && t.UserId == _uow.UserContext.CurrentUserId && t.Role == nextStep.DepartmentType);
                    }
                }
            }
            return autoNext;
        }

        private async Task<Department> GetUpperDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            if (lastHistory != null && !nextStep.TraversingFromRoot)
            {
                if (lastHistory.AssignedToDepartmentId.HasValue || lastHistory.AssignedToUserId.HasValue)
                {
                    indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == lastHistory.AssignedToDepartmentId || x.UserDepartmentMappings.Any(u => u.UserId == lastHistory.AssignedToUserId && u.IsHeadCount));
                }
                if (indexDept == null)
                {
                    indexDept = rqDepartment;
                }
            }
            var allGrade = await _uow.GetRepository<JobGrade>().GetAllAsync();
            int maxGrade = allGrade.Max(x => x.Grade);
            if (indexDept != null)
            {
                bool skip = false;
                var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
                var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.JobGrade));
                var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.MaxJobGrade));
                var currentJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
                if (nextStep.ReverseJobGrade)
                {
                    var depts = new Dictionary<int, Guid>();
                    depts.Add(currentJobGrade.Grade, indexDept.Id);
                    while (!skip)
                    {
                        if (indexDept.ParentId.HasValue)
                        {
                            indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId);
                            if (indexDept != null)
                            {
                                var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
                                if (indexJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                {
                                    skip = true;
                                }
                                else
                                {
                                    if (!depts.ContainsKey(indexJobGrade.Grade))
                                    {
                                        depts.Add(indexJobGrade.Grade, indexDept.Id);
                                    }
                                }
                            }
                            else
                            {
                                skip = true;
                            }
                        }
                        else
                        {
                            skip = true;
                        }
                    }
                    var jobGrade = nextStepMaxJobGrade.Grade;
                    for (var i = jobGrade; i >= nextStepJobGrade.Grade; i--)
                    {
                        if (depts.ContainsKey(i))
                        {
                            //Check next department type
                            var deptId = depts[i];
                            var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == deptId && (nextStep.DepartmentType == x.Role || (x.Department.IsPerfomance && (Group.Member == x.Role || Group.Checker == x.Role))));
                            if (hasParticipants)
                            {

                                indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == deptId);
                                return indexDept;
                            }
                            else
                            {
                                _refDeparmentId = indexDept.Id;
                            }
                        }
                    }
                    return null;
                }
                else
                {
                    if (nextStep.IncludeCurrentNode && currentJobGrade.Grade >= nextStepJobGrade.Grade && currentJobGrade.Grade <= nextStepMaxJobGrade.Grade)
                    {
                        var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == indexDept.Id && (nextStep.DepartmentType == x.Role || (x.Department.IsPerfomance && (Group.Member == x.Role || Group.Checker == x.Role))));
                        if (hasParticipants)
                        {
                            return indexDept;
                        }
                        else
                        {
                            _refDeparmentId = indexDept.Id;
                        }
                    }
                    while (!skip)
                    {
                        if (indexDept.ParentId.HasValue)
                        {
                            indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId);
                            if (indexDept != null)
                            {
                                var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
                                if (indexJobGrade.Grade >= nextStepJobGrade.Grade)
                                {
                                    if (indexJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                    {
                                        return null;
                                    }
                                    //If next step is large than department type, get the next step
                                    if (indexJobGrade.Grade > nextStepJobGrade.Grade)
                                    {
                                        nextStep.DepartmentType = nextStep.NextDepartmentType;
                                    }
                                    //Check next department type
                                    var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == indexDept.Id && (nextStep.DepartmentType == x.Role || (x.Department.IsPerfomance && (Group.Member == x.Role || Group.Checker == x.Role))));
                                    if (hasParticipants)
                                    {
                                        return indexDept;
                                    }
                                    else
                                    {
                                        _refDeparmentId = indexDept.Id;
                                    }
                                }
                            }
                            else
                            {
                                if (indexDept.JobGrade.Grade == maxGrade)
                                {
                                    return indexDept;
                                }
                                return null;
                            }
                        }
                        else
                        {
                            if (indexDept.JobGrade.Grade == maxGrade)
                            {
                                return indexDept;
                            }
                        }
                        if (indexDept == null || !indexDept.ParentId.HasValue)
                        {
                            return null;
                        }
                    }
                }
            }
            return indexDept;
        }

        private async Task<Department> GetBTAUpperDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            if (lastHistory != null && !nextStep.TraversingFromRoot)
            {
                if (lastHistory.AssignedToDepartmentId.HasValue || lastHistory.AssignedToUserId.HasValue)
                {
                    indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == lastHistory.AssignedToDepartmentId || x.UserDepartmentMappings.Any(u => u.UserId == lastHistory.AssignedToUserId && u.IsHeadCount));
                }
                if (indexDept == null)
                {
                    indexDept = rqDepartment;
                }
            }
            var allGrade = await _uow.GetRepository<JobGrade>().GetAllAsync();
            int maxGrade = allGrade.Max(x => x.Grade);
            if (indexDept != null)
            {
                bool skip = false;
                bool isFindDept = false;
                while (!skip)
                {
                    if (indexDept.ParentId.HasValue)
                    {
                        indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId);
                        if (indexDept != null)
                        {
                            var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == indexDept.Id && nextStep.DepartmentType == x.Role);
                            if (hasParticipants)
                            {
                                isFindDept = true;
                                skip = true;
                            }
                        }
                        else
                        {
                            skip = true;
                        }
                    }
                    else
                    {
                        if (indexDept.JobGrade.Grade == maxGrade)
                        {
                            isFindDept = true;
                        }
                        skip = true;
                    }
                }
                if (!isFindDept)
                    return null;
                else
                    return indexDept;
            }
            return indexDept;
        }

        private async Task<Department> GetAdminDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
            var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
            var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.JobGrade));
            var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.MaxJobGrade));
            if (nextStepMaxJobGrade == null) { nextStepMaxJobGrade = nextStepJobGrade; }

            Department foundDept = await FindMatchedAdminDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade);
            //ReCheck to get HQ if store is not found
            if (foundDept == null)
            {
                //Force to get next department type level if navigate to HQ
                nextStep.DepartmentType = nextStep.NextDepartmentType;
                foundDept = await FindMatchedAdminDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade, true);
            }

            if (foundDept != null)
            {
                var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                if (hasParticipants)
                {
                    return foundDept;
                }
                else
                {
                    _refDeparmentId = foundDept.Id;
                    var skip = false;
                    while (!skip)
                    {
                        if (foundDept.ParentId.HasValue)
                        {
                            foundDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == foundDept.ParentId);
                            if (foundDept != null)
                            {
                                var foundJobGrade = jobGrades.FirstOrDefault(x => x.Id == foundDept.JobGradeId);
                                if (foundJobGrade.Grade >= nextStepJobGrade.Grade)
                                {
                                    if (foundJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                    {
                                        return null;
                                    }
                                    //If next step is large than department type, get the next step
                                    if (foundJobGrade.Grade > nextStepJobGrade.Grade)
                                    {
                                        nextStep.DepartmentType = nextStep.NextDepartmentType;
                                    }
                                    //Check next department type
                                    hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                                    if (hasParticipants)
                                    {
                                        return foundDept;
                                    }
                                }
                            }
                            else { return null; }
                        }
                        if (foundDept == null || !foundDept.ParentId.HasValue)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        private async Task<Department> GetPerfDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
            var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
            var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.JobGrade));
            var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.MaxJobGrade));
            if (nextStepMaxJobGrade == null) { nextStepMaxJobGrade = nextStepJobGrade; }

            Department foundDept = await FindMatchedPerfDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade);
            //ReCheck to get HQ if store is not found
            if (foundDept == null)
            {
                //Force to get next department type level if navigate to HQ
                nextStep.DepartmentType = nextStep.NextDepartmentType;
                foundDept = await FindMatchedPerfDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade, true);
            }

            if (foundDept != null)
            {
                var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                if (hasParticipants)
                {
                    return foundDept;
                }
                else
                {
                    _refDeparmentId = foundDept.Id;
                    var skip = false;
                    while (!skip)
                    {
                        if (foundDept.ParentId.HasValue)
                        {
                            foundDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == foundDept.ParentId);
                            if (foundDept != null)
                            {
                                var foundJobGrade = jobGrades.FirstOrDefault(x => x.Id == foundDept.JobGradeId);
                                if (foundJobGrade.Grade >= nextStepJobGrade.Grade)
                                {
                                    if (foundJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                    {
                                        return null;
                                    }
                                    //If next step is large than department type, get the next step
                                    if (foundJobGrade.Grade > nextStepJobGrade.Grade)
                                    {
                                        nextStep.DepartmentType = nextStep.NextDepartmentType;
                                    }
                                    //Check next department type
                                    hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                                    if (hasParticipants)
                                    {
                                        return foundDept;
                                    }
                                }
                            }
                            else { return null; }
                        }
                        if (foundDept == null || !foundDept.ParentId.HasValue)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        private async Task<Department> GetCBDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
            var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
            var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.JobGrade));
            var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.MaxJobGrade));
            if (nextStepMaxJobGrade == null) { nextStepMaxJobGrade = nextStepJobGrade; }

            Department foundDept = await FindMatchedCBDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade);
            //ReCheck to get HQ if store is not found
            if (foundDept == null)
            {
                //Force to get next department type level if navigate to HQ
                foundDept = await FindMatchedCBDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade, true);
            }

            if (foundDept != null)
            {
                var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                if (hasParticipants != null)
                {
                    if ((hasParticipants.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)
                    {
                        nextStep.DepartmentType = nextStep.NextDepartmentType;
                    }
                    return foundDept;
                }
                else
                {
                    _refDeparmentId = foundDept.Id;
                    var skip = false;
                    while (!skip)
                    {
                        if (foundDept.ParentId.HasValue)
                        {
                            foundDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == foundDept.ParentId);
                            if (foundDept != null)
                            {
                                var foundJobGrade = jobGrades.FirstOrDefault(x => x.Id == foundDept.JobGradeId);
                                if (foundJobGrade.Grade >= nextStepJobGrade.Grade)
                                {
                                    if (foundJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                    {
                                        return null;
                                    }
                                    ////If next step is large than department type, get the next step
                                    //if (foundJobGrade.Grade > nextStepJobGrade.Grade)
                                    //{
                                    //    nextStep.DepartmentType = nextStep.NextDepartmentType;
                                    //}
                                    //Check next department type
                                    hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                                    if (hasParticipants != null)
                                    {
                                        if ((hasParticipants.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)
                                        {
                                            nextStep.DepartmentType = nextStep.NextDepartmentType;
                                        }
                                        return foundDept;
                                    }
                                }
                            }
                            else { return null; }
                        }
                        if (foundDept == null || !foundDept.ParentId.HasValue)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        private async Task<Department> FindMatchedCBDept(WorkflowStep nextStep, Department indexDept, JobGrade nextStepJobGrade, JobGrade nextStepMaxJobGrade, bool forceHQ = false)
        {
            IEnumerable<Department> hrDepts = new List<Department>();
            if (forceHQ)
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsCB
                  && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                  && x.IsStore == false && x.UserDepartmentMappings.Any(udm => (udm.Role == Group.All || (udm.Role & nextStep.DepartmentType) == nextStep.DepartmentType || (udm.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)), string.Empty, x => x.JobGrade);
            }
            else
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsCB
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                 && (!nextStep.IsHRHQ || x.IsStore != nextStep.IsHRHQ) && x.UserDepartmentMappings.Any(udm => (udm.Role == Group.All || (udm.Role & nextStep.DepartmentType) == nextStep.DepartmentType || (udm.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)), string.Empty, x => x.JobGrade);
            }
            return await FindDept(indexDept, forceHQ, hrDepts);
        }

        private async Task<Department> FindMatchedHRDept(WorkflowStep nextStep, Department indexDept, JobGrade nextStepJobGrade, JobGrade nextStepMaxJobGrade, bool forceHQ = false)
        {
            IEnumerable<Department> hrDepts = new List<Department>();
            if (forceHQ)
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsHR
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                  && x.IsStore == false && x.UserDepartmentMappings.Any(udm => ((udm.User.Role & UserRole.HR) == UserRole.HR || (udm.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin) && (udm.Role == Group.All || (udm.Role & nextStep.DepartmentType) == nextStep.DepartmentType || (udm.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)), string.Empty, x => x.JobGrade);
            }
            else
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsHR
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                 && (nextStep.IsHRHQ ? !x.IsStore : x.IsStore) && x.UserDepartmentMappings.Any(udm => ((udm.User.Role & UserRole.HR) == UserRole.HR || (udm.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin) && (udm.Role == Group.All || (udm.Role & nextStep.DepartmentType) == nextStep.DepartmentType || (udm.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)), string.Empty, x => x.JobGrade);
            }
            return await FindDept(indexDept, (forceHQ || nextStep.IsHRHQ), hrDepts);
        }

        private async Task<Department> FindMatchedAdminDept(WorkflowStep nextStep, Department indexDept, JobGrade nextStepJobGrade, JobGrade nextStepMaxJobGrade, bool forceHQ = false)
        {
            IEnumerable<Department> hrDepts = new List<Department>();
            if (forceHQ)
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsAdmin
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                  && x.IsStore == false, string.Empty, x => x.JobGrade);
            }
            else
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsAdmin
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                 && (!nextStep.IsHRHQ || x.IsStore != nextStep.IsHRHQ), string.Empty, x => x.JobGrade);
            }
            return await FindDept(indexDept, forceHQ, hrDepts);
        }

        private async Task<Department> FindMatchedPerfDept(WorkflowStep nextStep, Department indexDept, JobGrade nextStepJobGrade, JobGrade nextStepMaxJobGrade, bool forceHQ = false)
        {
            IEnumerable<Department> hrDepts = new List<Department>();
            if (forceHQ)
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsPerfomance
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                  && x.IsStore == false, string.Empty, x => x.JobGrade);
            }
            else
            {
                hrDepts = await _uow.GetRepository<Department>().FindByAsync(x => x.IsPerfomance
                && x.JobGrade.Grade >= nextStepJobGrade.Grade && x.JobGrade.Grade <= nextStepMaxJobGrade.Grade
                 && (!nextStep.IsHRHQ || x.IsStore != nextStep.IsHRHQ), string.Empty, x => x.JobGrade);
            }
            return await FindDept(indexDept, forceHQ, hrDepts);
        }

        private async Task<Department> FindDept(Department indexDept, bool forceHQ, IEnumerable<Department> hrDepts)
        {
            var allParentDeptIds = new List<Guid>();
            allParentDeptIds.Add(indexDept.Id);
            var titleG6 = await _uow.GetRepository<JobGrade>()
                .GetSingleAsync(x => x.Title != null && x.Title.ToUpper().Equals("G6"));
            int g6Value = titleG6?.Grade ?? 6;
            while (indexDept != null && indexDept.ParentId.HasValue)
            {
                //indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId && (x.JobGrade.Grade <= 6 || forceHQ == true));
                indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId && (x.JobGrade.Grade <= g6Value || forceHQ == true));
                if (indexDept != null)
                {
                    //if (indexDept.JobGrade.Grade == 6 && !ListUtilities.notInDept.Contains(indexDept.Code))
                    if (indexDept.JobGrade.Grade == g6Value && !ListUtilities.notInDept.Contains(indexDept.Code))
                    {
                        continue;
                    }
                    allParentDeptIds.Add(indexDept.Id);
                }
            }

            Department foundDept = null;
            var lastIdx = 10;
            hrDepts = hrDepts.OrderBy(x => x.JobGrade.Grade).ToList();
            foreach (var hrDept in hrDepts)
            {
                var skip = false;
                var parentDept = hrDept;
                while (!skip)
                {
                    if (parentDept.ParentId.HasValue)
                    {
                        parentDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == parentDept.ParentId);
                        if (parentDept != null && allParentDeptIds.Contains(parentDept.Id))
                        {
                            if (allParentDeptIds.IndexOf(parentDept.Id) < lastIdx)
                            {
                                lastIdx = allParentDeptIds.IndexOf(parentDept.Id);
                                foundDept = hrDept;
                            }
                        }
                    }
                    if (parentDept == null || !parentDept.ParentId.HasValue)
                    {
                        skip = true;
                    }
                }
            }

            return foundDept;
        }

        private async Task<Department> GetHrDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
            var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
            var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.MaxJobGrade));
            var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == int.Parse(nextStep.JobGrade));
            if (nextStepMaxJobGrade == null) { nextStepMaxJobGrade = nextStepJobGrade; }
            Department foundDept = await FindMatchedHRDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade);
            //ReCheck to get HQ if store is not found
            if (foundDept == null)
            {
                //Force to get next department type level if navigate to HQ
                foundDept = await FindMatchedHRDept(nextStep, indexDept, nextStepJobGrade, nextStepMaxJobGrade, true);
            }
            if (foundDept != null)
            {
                var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == foundDept.Id && (nextStep.ParticipantType == ParticipantType.HRDepartment && ((x.User.Role & UserRole.HR) == UserRole.HR || (x.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin)) && nextStep.DepartmentType == x.Role);
                if (hasParticipants == null)
                {
                    hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == foundDept.Id && (nextStep.ParticipantType == ParticipantType.HRDepartment && ((x.User.Role & UserRole.HR) == UserRole.HR || (x.User.Role & UserRole.HRAdmin) == UserRole.HRAdmin)) && nextStep.NextDepartmentType == x.Role);
                }

                if (hasParticipants != null)
                {
                    if ((hasParticipants.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)
                    {
                        nextStep.DepartmentType = nextStep.NextDepartmentType;
                    }
                    return foundDept;
                }
                else
                {
                    _refDeparmentId = foundDept.Id;
                    var skip = false;
                    while (!skip)
                    {
                        if (foundDept.ParentId.HasValue)
                        {
                            foundDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == foundDept.ParentId);
                            if (foundDept != null)
                            {
                                var foundJobGrade = jobGrades.FirstOrDefault(x => x.Id == foundDept.JobGradeId);
                                if (foundJobGrade.Grade >= nextStepJobGrade.Grade)
                                {
                                    if (foundJobGrade.Grade > nextStepMaxJobGrade.Grade)
                                    {
                                        return null;
                                    }
                                    ////If next step is large than department type, get the next step
                                    //if (foundJobGrade.Grade > nextStepJobGrade.Grade)
                                    //{
                                    //    nextStep.DepartmentType = nextStep.NextDepartmentType;
                                    //}
                                    //Check next department type
                                    hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == foundDept.Id && nextStep.DepartmentType == x.Role);
                                    if (hasParticipants != null)
                                    {
                                        if ((hasParticipants.Role & nextStep.NextDepartmentType) == nextStep.NextDepartmentType)
                                        {
                                            nextStep.DepartmentType = nextStep.NextDepartmentType;
                                        }
                                        return foundDept;
                                    }
                                }
                            }
                            else { return null; }
                        }
                        if (foundDept == null || !foundDept.ParentId.HasValue)
                        {
                            return null;
                        }
                    }
                }
            }
            return null;
        }
        private async Task<Department> GetAppraiser1Department(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            //CR - 420
            Department returnValue = null;
            var instanceId = lastHistory.InstanceId;
            try
            {
                if (!(rqDepartment is null))
                {
                    var workFlowInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == instanceId);
                    if (!(workFlowInstance is null))
                    {
                        var actingId = workFlowInstance.ItemId;
                        var actingItem = await _uow.GetRepository<Acting>().GetSingleAsync(x => x.Id == actingId);

                        var appraiser1DepartmentId = actingItem.FirstAppraiserDepartmentId;

                        var hasUser = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == appraiser1DepartmentId && x.Role == nextStep.DepartmentType);
                        if (!(hasUser is null))
                        {
                            returnValue = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == appraiser1DepartmentId);
                        }
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        private async Task<Department> GetAppraiser2Department(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            //CR - 420
            Department returnValue = null;
            var instanceId = lastHistory.InstanceId;
            try
            {
                if (!(rqDepartment is null))
                {
                    var workFlowInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == instanceId);
                    if (!(workFlowInstance is null))
                    {
                        var actingId = workFlowInstance.ItemId;
                        var actingItem = await _uow.GetRepository<Acting>().GetSingleAsync(x => x.Id == actingId);

                        var appraiser2DepartmentId = actingItem.SecondAppraiserDepartmentId;
                        var hasUser = await _uow.GetRepository<UserDepartmentMapping>().GetSingleAsync(x => x.DepartmentId == appraiser2DepartmentId && x.Role == nextStep.DepartmentType);
                        if (!(hasUser is null))
                        {
                            returnValue = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == appraiser2DepartmentId);
                        }
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        private Department GetHrManagerDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department returnValue = null;
            try
            {
                if (!(rqDepartment is null))
                {
                    string startJobGrade = nextStep.JobGrade.Replace("G", string.Empty);
                    string maxJobGrade = nextStep.MaxJobGrade.Replace("G", string.Empty);
                    returnValue = rqDepartment.GetHRManagerDepartment(_uow, startJobGrade.GetAsInt(), maxJobGrade.GetAsInt());
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        private async Task<Department> GetLevelDept(WorkflowHistory lastHistory, WorkflowStep nextStep, Department rqDepartment)
        {
            Department indexDept = rqDepartment;
            if (lastHistory != null && !nextStep.TraversingFromRoot)
            {
                if (lastHistory.AssignedToDepartmentId.HasValue || lastHistory.AssignedToUserId.HasValue)
                {
                    indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == lastHistory.AssignedToDepartmentId || x.UserDepartmentMappings.Any(u => u.UserId == lastHistory.AssignedToUserId && u.IsHeadCount));
                }
                if (indexDept == null)
                {
                    indexDept = rqDepartment;
                }
            }
            if (indexDept != null)
            {
                bool skip = false;
                var jobGrades = await _uow.GetRepository<JobGrade>().GetAllAsync();
                var currentGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId).Grade;
                var nextGrade = currentGrade + nextStep.Level;
                var nextMaxGrade = currentGrade + nextStep.MaxLevel;
                /*var maxJobLevel = MaxJobLevel;
                var maxJL = ConfigurationManager.AppSettings["MaxJobLevel"];
                if (!string.IsNullOrEmpty(maxJL))
                {
                    int.TryParse(maxJL, out maxJobLevel);
                }*/
                
                var allGrade = await _uow.GetRepository<JobGrade>().GetAllAsync();
                int maxJobLevel = allGrade.Max(x => x.Grade);
                nextMaxGrade = nextMaxGrade < maxJobLevel ? nextMaxGrade : maxJobLevel;
                var nextStepJobGrade = jobGrades.FirstOrDefault(x => x.Grade == nextGrade);
                var nextStepMaxJobGrade = jobGrades.FirstOrDefault(x => x.Grade == nextMaxGrade);
                int step = 0;
                while (!skip)
                {
                    if (step != 0)
                    {
                        indexDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == indexDept.ParentId);
                    }

                    var indexJobGrade = jobGrades.FirstOrDefault(x => x.Id == indexDept.JobGradeId);
                    if (indexDept != null && indexJobGrade.Grade >= nextStepJobGrade.Grade && indexJobGrade.Grade <= nextStepMaxJobGrade.Grade)
                    {
                        if (indexJobGrade.Grade > nextStepMaxJobGrade.Grade)
                        {
                            return null;
                        }
                        //If next step is large than department type, get the next step
                        if (indexJobGrade.Grade > nextStepJobGrade.Grade)
                        {
                            nextStep.DepartmentType = nextStep.NextDepartmentType;
                        }
                        //Check next department type
                        var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == indexDept.Id && nextStep.DepartmentType == x.Role);
                        if (hasParticipants)
                        {
                            return indexDept;
                        }
                        else
                        {
                            _refDeparmentId = indexDept.Id;
                        }
                    }
                    if (indexDept == null || !indexDept.ParentId.HasValue)
                    {
                        return null;
                    }
                    step++;
                }
            }
            return indexDept;
        }

        private async Task<bool> DoesStatusUpdated(WorkflowItem wfItem, string status)
        {
            bool returnValue = false;
            try
            {
                WorkflowItem newWFItem = await GetWorkflowItem(wfItem.Entity.Id);
                if (newWFItem != null && newWFItem.Entity != null)
                {
                    string newStatus = newWFItem.Entity.Status;
                    returnValue = (status == newStatus);
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        //private async Task<bool> CheckAndUpdateStatusUntilSuccess(WorkflowItem wfItem, string status, int counter)
        //{
        //    bool returnValue = false; 
        //    try
        //    {
        //        if (wfItem != null && wfItem.Entity != null && counter < 50)
        //        {
        //            System.Threading.Thread.Sleep(100);
        //            bool checkedStatus = await DoesStatusUpdated(wfItem, status);
        //            if (checkedStatus)
        //            {
        //                returnValue = true;
        //            }
        //            else
        //            {
        //                wfItem.Entity.Status = status;
        //                await _uow.CommitAsync();
        //                returnValue = await CheckAndUpdateStatusUntilSuccess(wfItem, status, counter+1);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        returnValue = false;
        //    }
        //    return returnValue;
        //}

        private async Task CompleteWorkflow(WorkflowItem wfItem, WorkflowInstance wfInstance, WorkflowHistory lastHistory)
        {
            try
            {
                var item = wfItem.Entity;
                //Complete workflow
                //Update Item Status
                string status = "";
                if (lastHistory.VoteType == VoteType.Approve)
                {
                    status = string.IsNullOrEmpty(wfInstance.DefaultCompletedStatus) ? lastHistory == null ? "Completed" : lastHistory.Outcome : wfInstance.DefaultCompletedStatus;
                }
                else
                {
                    status = lastHistory == null ? "Completed" : lastHistory.Outcome;
                }

                item.Status = status;
                await UpdatePermission(item.Id, Right.View);
                wfInstance.IsCompleted = true;
                if (lastHistory.VoteType == VoteType.Approve)
                {
                    item.SignedBy = lastHistory == null ? item.CreatedBy : lastHistory.Approver;
                    item.SignedDate = DateTime.Now;
                    //Update wf status
                    string bkStatus = wfItem.Entity.Status;
                    await _uow.CommitAsync();
                    //await CheckAndUpdateStatusUntilSuccess(wfItem, bkStatus, 0);
                    //Send email notification
                    await SendEmailNotificationForCreator(EmailTemplateName.ForCreatorApproved, wfItem); // Check UAT pending
                    //Push Data
                    await PushData(item);
                    await CompleteAction(wfItem);

                }
                else if (lastHistory.VoteType == VoteType.Reject)
                {
                    //Update wf status
                    string bkStatus = wfItem.Entity.Status;
                    await _uow.CommitAsync();
                    /*SaveLogWorkflow(item.Id, item.Status, "After Check Reject");
                    if (!item.Status.Equals("Rejected"))
                    {
                        item.Status = "Rejected";
                        await _uow.CommitAsync();
                    }*/

                    //await CheckAndUpdateStatusUntilSuccess(wfItem, bkStatus, 0);
                    //Send failure to requestor
                    await SendEmailNotificationForCreator(EmailTemplateName.ForCreatorRejected, wfItem);
                    await CancelAction(wfItem);
                }
                else if (lastHistory.VoteType == VoteType.Cancel)
                {
                    //Update wf status
                    string bkStatus = wfItem.Entity.Status;
                    await _uow.CommitAsync();
                    //await CheckAndUpdateStatusUntilSuccess(wfItem, bkStatus, 0);
                    await CancelAction(wfItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot complete workflow {0}", ex.Message);
            }
        }

        private async Task RemoveItemPerm(Guid id)
        {
            var perms = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == id);
            _uow.GetRepository<Permission>().Delete(perms);
        }

        private async Task UpdatePermission(Guid id, Right right)
        {
            try
            {
                var perms = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == id);
                foreach (var perm in perms)
                {
                    perm.Perm = right;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Cannot update permission, ex: {0}", ex.Message);
            }
        }

        private async Task AssignPermissionToUser(Guid id, Guid userId, Right right)
        {
            var perms = await _uow.GetRepository<Permission>().FindByAsync(x => x.ItemId == id && x.UserId == userId);
            foreach (var perm in perms)
            {
                perm.Perm = right;
            }
        }

        private void AssignTaskPerm(WorkflowTask task, WorkflowStep nextStep, Guid? userId, Guid? departmentId, List<Permission> permissions)
        {
            permissions.Add(new Permission()
            {
                Perm = Right.View,
                UserId = userId,
                DepartmentId = departmentId,
                DepartmentType = nextStep.DepartmentType,
                ItemId = task.Id
            });
        }

        private void AssignItemPermToPreviousStep(WorkflowEntity item, IEnumerable<WorkflowHistory> wfHistories, WorkflowStep nextStep, List<Permission> permissions)
        {
            //Add perm for requestor
            permissions.Add(new Permission()
            {
                Perm = nextStep.RequestorPerm,
                UserId = item.CreatedById,
                ItemId = item.Id
            });
            if (wfHistories != null)
            {
                //Add perm for previous approvers
                foreach (var wfHistory in wfHistories)
                {
                    permissions.Add(new Permission()
                    {
                        Perm = Right.View,
                        UserId = wfHistory.ApproverId,
                        DepartmentId = wfHistory.AssignedToDepartmentId,
                        DepartmentType = wfHistory.AssignedToDepartmentType,
                        ItemId = item.Id
                    });
                }
            }
        }

        private void AssignItemPermForNextStep(WorkflowEntity item, WorkflowStep nextStep, Guid? userId, Guid? departmentId, List<Permission> permissions)
        {
            //Add perm for current approver
            permissions.Add(new Permission()
            {
                Perm = nextStep.ApproverPerm,
                UserId = userId,
                DepartmentId = departmentId,
                DepartmentType = nextStep.DepartmentType,
                ItemId = item.Id
            });
        }

        private async Task<WorkflowItem> GetWorkflowItem(Guid ItemId)
        {
            var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
            var workflowModelTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
            foreach (var workflowModelType in workflowModelTypes)
            {
                var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", workflowModelType);
                var item = (await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", ItemId));
                if (item != null)
                {
                    return new WorkflowItem() { Entity = item as WorkflowEntity, Type = workflowModelType.Name };
                }
            }
            return null;
        }

        private async Task<Dictionary<string, string>> ExtractItemProperties(WorkflowEntity item)
        {
            var result = new Dictionary<string, string>();
            //Add department values
            var rqDepartment = await GetRequestDepartment(item.CreatedById.Value);
            if (rqDepartment != null)
            {
                result["isassistant"] = Convert.ToString(await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.UserId == item.CreatedById && x.DepartmentId == rqDepartment.Id && x.Role == Group.Assistance));
                result["isstore"] = Convert.ToString(rqDepartment.IsStore);
                result["ishr"] = Convert.ToString(rqDepartment.IsHR);
                result["iscb"] = Convert.ToString(rqDepartment.IsCB);
                result["isPerf"] = Convert.ToString(rqDepartment.IsPerfomance);
                //result["requestedjobgrade"] = Convert.ToString(rqDepartment.JobGrade.Caption);
                result["requestedjobgrade"] = Convert.ToString(rqDepartment.JobGrade.Grade);
                result["requesteddepartment"] = Convert.ToString(rqDepartment.Name);
                result["requesteddepartmentcode"] = Convert.ToString(rqDepartment.Code);
                if (rqDepartment.BusinessModel != null && !string.IsNullOrEmpty(rqDepartment.BusinessModel.Code))
                {
                    result["businessmodelcode"] = Convert.ToString(rqDepartment.BusinessModel.Code);
                }
            }

            // AEONCR102-5: Approval OT normal for G4+
            var overtimeApplication = await _uow.GetRepository<OvertimeApplication>().FindByIdAsync(item.Id);
            if (overtimeApplication != null)
            {
                var overtimeApplicationDetail = await _uow.GetRepository<OvertimeApplicationDetail>().FindByAsync(x => x.OvertimeApplicationId == overtimeApplication.Id);
                if (overtimeApplicationDetail.Any())
                {
                    var OTNormal = false;
                    // case 1
                    bool isExistHoliday = overtimeApplicationDetail.Where(x => x.Date != null && x.Date.Date.ToLocalTime().IsPublicHoliday(_uow)).Any();

                    // case 2
                    bool isNotExistShiftCodePRD = false;
                    foreach (var dtl in overtimeApplicationDetail)
                    {
                        string userSAPCode = null;
                        if (overtimeApplication.Type == OverTimeType.EmployeeSeftService)
                        {
                            userSAPCode = overtimeApplication.UserSAPCode;
                        }
                        else if (overtimeApplication.Type == OverTimeType.ManagerApplyForEmployee)
                        {
                            userSAPCode = dtl.SAPCode;
                        }
                        if (!string.IsNullOrEmpty(userSAPCode))
                        {
                            var currentUser = await _uow.GetRepository<User>().GetSingleAsync(x => userSAPCode == x.SAPCode && x.IsActivated && !x.IsDeleted);
                            if (currentUser != null)
                            {
                                TargetDateDetail targetDetail = currentUser.GetActualTarget1_ByDate(_uow, dtl.Date.Date, true);
                                if (targetDetail != null && !string.IsNullOrEmpty(targetDetail.date) && !string.IsNullOrEmpty(targetDetail.value))
                                {
                                    var shiftCode = await _uow.GetRepository<ShiftCode>(true).GetSingleAsync(x => !x.IsDeleted && x.Code.ToLower() == targetDetail.value.ToLower());
                                    if (shiftCode != null)
                                    {
                                        if (!shiftCode.Code.Equals("PRD"))
                                        {
                                            isNotExistShiftCodePRD = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!isExistHoliday)
                    {
                        OTNormal = isNotExistShiftCodePRD ? true : false;
                    }

                    if (OTNormal)
                    {
                        result["otnormal"] = Convert.ToString(OTNormal);
                    }
                }
            }

            var fields = item.GetType().GetProperties();
            //PropertyInfo deptDivision = null;
            foreach (var field in fields)
            {
                //if (field.Name == "DeptDivision") {
                //    deptDivision = field;
                //}
                //if (field.Name == "DeptCode")
                //{
                //    result[field.Name.ToLower()] = Convert.ToString(deptDivision.GetValue(item).GetPropValue("Code"));
                //}
                //else if (field.Name == "DeptName")
                //{
                //    result[field.Name.ToLower()] = Convert.ToString(deptDivision.GetValue(item).GetPropValue("Name"));
                //}
                //else {

                //}
                // CR 9.6 G5.5
                if (Const.FieldNameGrades.Contains(field.Name))
                {
                    result[field.Name.ToLower()] = Convert.ToString(field.GetValue(item)).Replace("G","");
                } else
                {
                    result[field.Name.ToLower()] = Convert.ToString(field.GetValue(item));
                }
            }
            return result;
        }

        private async Task<Department> GetRequestDepartment(Guid userId, Dictionary<string, string> itemPros = null, WorkflowData wfData = null, WorkflowStep nextStep = null)
        {
            Department rqDepartment = null;
            if (wfData != null && wfData.OverwriteRequestedDepartment)
            {
                var field = wfData.RequestedDepartmentField;
                if (nextStep != null && nextStep.OverwriteRequestedDepartment)
                {
                    field = nextStep.RequestedDepartmentField;
                }
                if (itemPros.ContainsKey(field?.Trim().ToLowerInvariant()))
                {
                    if (Guid.TryParse(itemPros[field?.Trim().ToLowerInvariant()], out Guid departmentGuid))
                    {
                        rqDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == departmentGuid, "", x => x.JobGrade);
                    }
                    else
                    {
                        rqDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Code == field, "", x => x.JobGrade);
                    }
                }
            }
            if (rqDepartment == null)
            {
                rqDepartment = await _uow.GetRepository<Department>().GetSingleAsync(x => x.UserDepartmentMappings.Any(t => t.UserId == userId && t.IsHeadCount), "", x => x.JobGrade);
            }
            return rqDepartment;
        }

        private bool IsValidCondition(Dictionary<string, string> itemProperties, IList<WorkflowCondition> conditions)
        {
            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    var field = condition.FieldName.ToLowerInvariant();
                    if (itemProperties.ContainsKey(field))
                    {
                        var extractedValue = itemProperties[field]?.Trim().ToLowerInvariant();
                        var isValid = IsValid(condition, extractedValue);
                        if (!isValid)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsValid(WorkflowCondition condition, string extractedValue)
        {
            foreach (var conV in condition.FieldValues)
            {
                if (conV?.ToLowerInvariant().Trim() == extractedValue)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task SendEmailNotificationForCreator(EmailTemplateName type, WorkflowItem item)
        {
            try
            {
                var user = await _uow.GetRepository<User>().FindByIdAsync(item.Entity.CreatedById.Value);
                if (user != null)
                {
                    var mergeFields = new Dictionary<string, string>();
                    mergeFields["CreatorName"] = user.FullName;
                    string linkType, bizName, bizNameVN;
                    Utilities.GetLinkType(item.Type, out linkType, out bizName);
                    Utilities.GetLinkTypeVN(item.Type, out linkType, out bizNameVN);
                    mergeFields["BusinessName"] = bizName;
                    mergeFields["BusinessNameVN"] = bizNameVN;
                    mergeFields["ReferenceNumber"] = item.Entity.ReferenceNumber;
                    mergeFields["Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/{linkType}/item/{item.Entity.ReferenceNumber}?id={item.Entity.Id}\">{item.Entity.ReferenceNumber}</a>";
                    var recipients = new List<string>() { user.Email };
                    await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task PushData(WorkflowEntity item)
        {
            try
            {
                if (item.Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _exBO.SubmitData(item, true);
                }
                else
                {
                    //CR318 for Leave Application
                    if ((item.ReferenceNumber.StartsWith("LEA-", StringComparison.InvariantCultureIgnoreCase) || item.ReferenceNumber.StartsWith("SHI-", StringComparison.InvariantCultureIgnoreCase))

                        && item.Status.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await _exBO.SubmitData(item, true);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("PushData: " + ex.Message + ex.StackTrace, ex);
            }
        }

        private async Task DoCustomAction(WorkflowItem wfItem, string lastOutcome)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly();
                var actions = ass.GetTypes().Where(x => typeof(ICustomAction).IsAssignableFrom(x));
                var action = actions.FirstOrDefault(x => x.GetCustomAttribute<ActionAttribute>() != null && x.GetCustomAttribute<ActionAttribute>().Type.Name == wfItem.Type);
                if (action != null)
                {
                    var act = (ICustomAction)Activator.CreateInstance(action);
                    await act.Execute(_uow, wfItem.Entity.Id, lastOutcome, _dashboardBO, this);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private async Task CompleteAction(WorkflowItem wfItem)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly();
                var actions = ass.GetTypes().Where(x => typeof(ICompleteAction).IsAssignableFrom(x));
                var action = actions.FirstOrDefault(x => x.GetCustomAttribute<ActionAttribute>() != null && x.GetCustomAttribute<ActionAttribute>().Type.Name == wfItem.Type);
                if (action != null)
                {
                    ICompleteAction act = (ICompleteAction)Activator.CreateInstance(action);
                    await act.Execute(_uow, wfItem.Entity.Id, _dashboardBO, this, _logger);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private async Task RequestToChangeAction(WorkflowItem wfItem, string stepName)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly();
                var actions = ass.GetTypes().Where(x => typeof(IRequestToChangeAction).IsAssignableFrom(x));
                var action = actions.FirstOrDefault(x => x.GetCustomAttribute<ActionAttribute>() != null && x.GetCustomAttribute<ActionAttribute>().Type.Name == wfItem.Type);
                if (action != null)
                {
                    IRequestToChangeAction act = (IRequestToChangeAction)Activator.CreateInstance(action);
                    await act.Execute(_uow, wfItem.Entity.Id, _dashboardBO, this, _logger, stepName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private async Task CancelAction(WorkflowItem wfItem)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly();
                var actions = ass.GetTypes().Where(x => typeof(ICancelAction).IsAssignableFrom(x));
                var action = actions.FirstOrDefault(x => x.GetCustomAttribute<ActionAttribute>() != null && x.GetCustomAttribute<ActionAttribute>().Type.Name == wfItem.Type);
                if (action != null)
                {
                    ICancelAction act = (ICancelAction)Activator.CreateInstance(action);
                    await act.Execute(_uow, wfItem.Entity.Id, _dashboardBO, this, _logger);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        // Tam Khóa
        //public async Task<ResultDTO> GetTasks(QueryArgs args, bool isSuperAdmin = false)
        //{
        //    try
        //    {
        //        args.Predicate += " AND (( itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") AND dueDate >= \"2020-02-25 20:52:06.8996976 +07:00\" ) OR !(itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") ))";
        //        var edoc2_tasks = _uow.GetRepository<WorkflowTask>(isSuperAdmin).FindBy<WorkflowTaskViewModel>(args.Order, 1, 1000, args.Predicate, args.PredicateParameters).ToList();

        //        if (edoc2_tasks.Count > 0)
        //        {
        //            foreach (WorkflowTaskViewModel item in edoc2_tasks)
        //            {
        //                var departmentItem = _uow.GetRepository<Department>(true).FindBy(x => x.Id == item.RequestedDepartmentId).FirstOrDefault();
        //                if (null != departmentItem)
        //                {
        //                    if (null != departmentItem.Region)
        //                    {
        //                        item.RegionId = departmentItem.Region.Id;
        //                        item.RegionName = departmentItem.Region.RegionName;
        //                    }
        //                    else
        //                    {
        //                        item.RegionName = "";
        //                    }
        //                }
        //                item.Module = ModuleIntegrationsConstants.EDOC2;
        //            }
        //        }

        //        #region Get Ignore Acting Items
        //        List<string> ignoreReferenceNumberActing = this.IgnoreReferenceNumberActing();
        //        if (ignoreReferenceNumberActing.Any())
        //        {
        //            edoc2_tasks = edoc2_tasks.Where(x => ignoreReferenceNumberActing.Any() && !ignoreReferenceNumberActing.Contains(x.ReferenceNumber)).ToList();
        //        }
        //        #endregion

        //        var edoc1_tasks = await _edoc01.GetTasks(new Edoc1Arg
        //        {
        //            LoginName = _uow.UserContext.CurrentUserName,
        //            OrderBy = "Created asc",
        //            Skip = 0,
        //            Top = 10000
        //        });

        //        //ncao2: add more tasks from new modules
        //        var moduleTasks = DashboardHelper.GetMyTasks(_uow.UserContext.CurrentUserId, args);

        //        //add more tasks from facility modules
        //        var facilityTasks = new List<WorkflowTaskViewModel>();
        //        try
        //        {
        //            facilityTasks = await _facilityBO.GetTasks(args, isSuperAdmin);
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.LogError(e.Message, e);
        //        }

        //        // Trade Contract
        //        var tradeContractTask = await _tradeContractBO.GetTasks(new TradeContractArgs
        //        {
        //            LoginName = _uow.UserContext.CurrentUserName,
        //            /*LoginName = "edoc03",*/
        //            Skip = 0,
        //            Top = 20,
        //            DocumentSetTypes = new string[] { }
        //        });

        //        // SKU
        //        var skuTasks = await _skuBO.GetTasks(new SKUArgs
        //        {
        //            UserName = _uow.UserContext.CurrentUserName
        //        });

        //        #region get current task
        //        var new_edoc1_tasks = await _edoc01.GetTasksV2(new Edoc1ArgV2
        //        {
        //            LoginName = _uow.UserContext.CurrentUserName,
        //            Page = 1,
        //            Limit = 1000
        //        });
        //        #endregion

        //        var count = edoc2_tasks.Count() + edoc1_tasks.Count() + new_edoc1_tasks.Count() + moduleTasks.Count + facilityTasks.Count + tradeContractTask.Count + skuTasks.Count;
        //        return new ResultDTO()
        //        {
        //            Object = new { Items = (edoc2_tasks.Concat(edoc1_tasks).Concat(moduleTasks).Concat(facilityTasks).Concat(tradeContractTask).Concat(skuTasks).Concat(new_edoc1_tasks)), Count = count }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //        return new ResultDTO()
        //        {
        //            Messages = new List<string>() { ex.Message },
        //            ErrorCodes = new List<int>() { -1 }
        //        };
        //    }
        //}

        // Hàm tối ưu
        public async Task<ResultDTO> GetTasks(QueryArgs args, bool isSuperAdmin = false)
        {
            string APIKey = "AIzaSyD-EXEMPLEKEA1234567890ABCDEFGHIJK";
            try
            {
                args.Predicate += " AND (( itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") AND dueDate >= \"2020-02-25 20:52:06.8996976 +07:00\" ) OR !(itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") ))";
                var edoc2_tasks = _uow.GetRepository<WorkflowTask>(isSuperAdmin).FindBy<WorkflowTaskViewModel>(args.Order, 1, 1000, args.Predicate, args.PredicateParameters).ToList();
                if (edoc2_tasks.Count > 0)
                {
                    foreach (WorkflowTaskViewModel item in edoc2_tasks)
                    {
                        var departmentItem = _uow.GetRepository<Department>(true).FindBy(x => x.Id == item.RequestedDepartmentId).FirstOrDefault();
                        if (null != departmentItem)
                        {
                            if (null != departmentItem.Region)
                            {
                                item.RegionId = departmentItem.Region.Id;
                                item.RegionName = departmentItem.Region.RegionName;
                            }
                            else
                            {
                                item.RegionName = "";
                            }
                        }
                        item.Module = ModuleIntegrationsConstants.EDOC2;
                    }
                }

                #region Get Ignore Acting Items
                List<string> ignoreReferenceNumberActing = this.IgnoreReferenceNumberActing();
                if (ignoreReferenceNumberActing.Any())
                {
                    edoc2_tasks = edoc2_tasks.Where(x => ignoreReferenceNumberActing.Any() && !ignoreReferenceNumberActing.Contains(x.ReferenceNumber)).ToList();
                }
                #endregion

                var taskResults = new Dictionary<string, List<WorkflowTaskViewModel>>();

                var tasks = new List<Task>
                {
                    #region Edoc1 không còn sử dụng
                    //(_edoc01 != null ? _edoc01.GetTasks(new Edoc1Arg
                    //{
                    //    LoginName = _uow.UserContext.CurrentUserName,
                    //    OrderBy = "Created asc",
                    //    Skip = 0,
                    //    Top = 10000
                    //}) : Task.FromResult(new List<WorkflowTaskViewModel>())).ContinueWith(t =>
                    //    taskResults["Edoc1"] = t.IsFaulted ? new List<WorkflowTaskViewModel>() : t.Result),
                    #endregion
                    #region Facility
                    (_facilityBO != null ? _facilityBO.GetTasks(args, isSuperAdmin) : Task.FromResult(new List<WorkflowTaskViewModel>())).ContinueWith(t =>
                        taskResults["Facility"] = t.IsFaulted ? new List<WorkflowTaskViewModel>() : t.Result),
                    #endregion
                    #region Trade 
                    (_tradeContractBO != null ? _tradeContractBO.GetTasks(new TradeContractArgs
                    {
                        LoginName = _uow.UserContext.CurrentUserName,
                        Skip = 0,
                        Top = 20,
                        DocumentSetTypes = new string[] { }
                    }) : Task.FromResult(new List<WorkflowTaskViewModel>())).ContinueWith(t =>
                        taskResults["TradeContract"] = t.IsFaulted ? new List<WorkflowTaskViewModel>() : t.Result),
                    #endregion
                    #region SKU
                    (_skuBO != null ? _skuBO.GetTasks(new SKUArgs
                    {
                        UserName = _uow.UserContext.CurrentUserName
                    }) : Task.FromResult(new List<WorkflowTaskViewModel>())).ContinueWith(t =>
                        taskResults["SKU"] = t.IsFaulted ? new List<WorkflowTaskViewModel>() : t.Result),
                    #endregion
                    #region get current task
                    (_edoc01 != null ? _edoc01.GetTasksV2(new Edoc1ArgV2
                    {
                        LoginName = _uow.UserContext.CurrentUserName,
                        Page = 1,
                        Limit = 1000
                    }) : Task.FromResult(new List<WorkflowTaskViewModel>())).ContinueWith(t =>
                        taskResults["Edoc1V2"] = t.IsFaulted ? new List<WorkflowTaskViewModel>() : t.Result)
                    #endregion
                };

                await Task.WhenAll(tasks);

                //ncao2: add more tasks from new modules
                var moduleTasks = DashboardHelper.GetMyTasks(_uow.UserContext.CurrentUserId, args);

                var allTasks = new List<WorkflowTaskViewModel>(edoc2_tasks);
                allTasks.AddRange(taskResults.TryGetValue("Edoc1", out var edoc1Result) ? edoc1Result : new List<WorkflowTaskViewModel>());
                allTasks.AddRange(moduleTasks);
                allTasks.AddRange(taskResults.TryGetValue("Facility", out var facilityResult) ? facilityResult : new List<WorkflowTaskViewModel>());
                allTasks.AddRange(taskResults.TryGetValue("TradeContract", out var tradeContractResult) ? tradeContractResult : new List<WorkflowTaskViewModel>());
                allTasks.AddRange(taskResults.TryGetValue("SKU", out var skuResult) ? skuResult : new List<WorkflowTaskViewModel>());
                allTasks.AddRange(taskResults.TryGetValue("Edoc1V2", out var edoc1V2Result) ? edoc1V2Result : new List<WorkflowTaskViewModel>());

                var count = allTasks.Count;
                return new ResultDTO
                {
                    Object = new { Items = allTasks, Count = count }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ResultDTO
                {
                    Messages = new List<string> { ex.Message },
                    ErrorCodes = new List<int> { -1 }
                };
            }
        }

        public async Task<ResultDTO> GetTodoList(TodoListArgs args)
        {
            _logger.LogInformation("Time Start: " + DateTimeOffset.Now.ToString("HH:mm"));
            try
            {
                var queryArgs = new QueryArgs()
                {
                    Order = "Status desc, Created",
                    Predicate = "isCompleted != @0",
                    PredicateParameters = new object[] { true },
                    Limit = 10,
                    Page = 1
                };

                if (string.IsNullOrEmpty(args.LoginName))
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() { "Login name is required!" },
                        ErrorCodes = new List<int>() { -1 }
                    };
                }
                var findUser = await _uow.GetRepository<User>(true).GetSingleAsync(x => x.LoginName.Equals(args.LoginName));
                if (findUser == null)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() { "User is not exists!" },
                        ErrorCodes = new List<int>() { -1 }
                    };
                }
                _uow.UserContext.CurrentUserId = findUser.Id;
                _uow.UserContext.CurrentUserName = findUser.LoginName;
                _uow.UserContext.CurrentUserFullName = findUser.FullName;

                queryArgs.Predicate += " AND (( itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") AND dueDate >= \"2020-02-25 20:52:06.8996976 +07:00\" ) OR !(itemType IN (\"ShiftExchangeApplication\",\"LeaveApplication\",\"OvertimeApplication\",\"MissingTimeClock\") ))";
                var edoc2_tasks = await _uow.GetRepository<WorkflowTask>(false).FindByAsync<WorkflowTaskViewModel>("Created desc", 1, 1000, queryArgs.Predicate, queryArgs.PredicateParameters);
                var getAllDepartments = await _uow.GetRepository<Department>().GetAllAsync();
                _logger.LogInformation("Time getAllDepartments S: " + DateTimeOffset.Now.ToString("HH:mm:ss"));
                string baseUrl = ConfigurationManager.AppSettings["siteUrl"] != null ? ConfigurationManager.AppSettings["siteUrl"].ToString() : "";
                string subSiteUrl = ConfigurationManager.AppSettings["subSiteUrl"] != null ? ConfigurationManager.AppSettings["subSiteUrl"].ToString() : "/hrv2/#!/";
                string subLiquorSiteUrl = ConfigurationManager.AppSettings["subLiquorUrl"] != null ? ConfigurationManager.AppSettings["subLiquorUrl"].ToString() : "/LiquorV2/";
                string subSKUSiteUrl = ConfigurationManager.AppSettings["subSKUUrl"] != null ? ConfigurationManager.AppSettings["subSKUUrl"].ToString() : "/SKUV2/";
                string subFinSiteUrl = ConfigurationManager.AppSettings["subFinUrl"] != null ? ConfigurationManager.AppSettings["subFinUrl"].ToString() : "/Finv2/";
                if (edoc2_tasks.Any())
                {
                    foreach (WorkflowTaskViewModel item in edoc2_tasks)
                    {
                        var findRequestedDepartment = getAllDepartments.Where(x => x.Id == item.RequestedDepartmentId).FirstOrDefault();
                        if (findRequestedDepartment != null && findRequestedDepartment.Region != null)
                        {
                            item.RegionId = findRequestedDepartment.Region.Id;
                            item.RegionName = findRequestedDepartment.Region.RegionName;
                        }
                        else
                        {
                            item.RegionName = "";
                        }
                        if (!string.IsNullOrEmpty(baseUrl))
                        {
                            switch (item.ItemType)
                            {
                                case "RequestToHire":
                                    item.Link = baseUrl + subSiteUrl + $"/home/requestToHire/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "PromoteAndTransfer":
                                    item.Link = baseUrl + subSiteUrl + $"/home/promoteAndTransfer/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "Acting":
                                    item.Link = baseUrl + subSiteUrl + $"/home/action/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "LeaveApplication":
                                    item.Link = baseUrl + subSiteUrl + $"/home/leaves-management/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "MissingTimeClock":
                                    item.Link = baseUrl + subSiteUrl + $"/home/missingTimelock/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "OvertimeApplication":
                                    item.Link = baseUrl + subSiteUrl + $"/home/overtimeApplication/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "ShiftExchangeApplication":
                                    item.Link = baseUrl + subSiteUrl + $"/home/shift-exchange/item/${item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "ResignationApplication":
                                    item.Link = baseUrl + subSiteUrl + $"/home/resignationApplication/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "BusinessTripApplication":
                                    item.Link = baseUrl + subSiteUrl + $"/home/business-trip-application/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "TargetPlan":
                                    item.Link = baseUrl + subSiteUrl + $"/home/target-plan/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                                case "BusinessTripOverBudget":
                                    item.Link = baseUrl + subSiteUrl + $"/home/over-budget/item/{item.ReferenceNumber}?id={item.ItemId}";
                                    break;
                            }
                        }
                        
                        item.Module = ModuleIntegrationsConstants.EDOC2;
                    }
                }
                _logger.LogInformation("Time getAllDepartments: E" + DateTimeOffset.Now.ToString("HH:mm:ss"));

                #region Get Ignore Acting Items
                List<string> ignoreReferenceNumberActing = this.IgnoreReferenceNumberActing();
                if (ignoreReferenceNumberActing.Any())
                {
                    edoc2_tasks = edoc2_tasks.Where(x => ignoreReferenceNumberActing.Any() && !ignoreReferenceNumberActing.Contains(x.ReferenceNumber)).ToList();
                }
                #endregion

                /*var edoc1_tasks = await _edoc01.GetTasks(new Edoc1Arg
                {
                    LoginName = args.LoginName,
                    OrderBy = "Created asc",
                    Skip = 0,
                    Top = 10000
                });*/
                _logger.LogInformation("Time edoc1_tasks: " + DateTimeOffset.Now.ToString("HH:mm:ss"));

                var moduleTasks = DashboardHelper.GetMyTasks(findUser.Id, queryArgs);
                _logger.LogInformation("ModuleTasks 1: " + JsonConvert.SerializeObject(moduleTasks));
                moduleTasks.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Link)) return;
                    if (x.ReferenceNumber.StartsWith("ATR-"))
                    {
                        x.Link = baseUrl + subSiteUrl + $"home/academy/edit-request/{x.ItemId}";
                    }
                });

                var facilityTasks = new List<WorkflowTaskViewModel>();
                try
                {
                    facilityTasks = await _facilityBO.GetTasks(queryArgs, true);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
                _logger.LogInformation("Time _tradeContractBO: " + DateTimeOffset.Now.ToString("HH:mm:ss"));

                // Trade Contract
                var tradeContractTask = await _tradeContractBO.GetTasks(new TradeContractArgs
                {
                    LoginName = args.LoginName,
                    Skip = 0,
                    Top = 20,
                    DocumentSetTypes = new string[] { }
                });

                _logger.LogInformation("Time skuTasks: " + DateTimeOffset.Now.ToString("HH:mm:ss"));
                var skuTasks = await _skuBO.GetTasks(new SKUArgs
                {
                    UserName = args.LoginName
                });
                skuTasks = skuTasks.Select(x => new WorkflowTaskViewModel
                {
                    ReferenceNumber = x.ReferenceNumber,
                    IsCompleted = x.IsCompleted,
                    ItemId = x.ItemId,
                    ItemType = x.ItemType,
                    Link = x.Link,
                    Module = x.Module,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    RequestorFullName = x.RequestorFullName,
                    RequestorId = x.RequestorId,
                    RequestorUserName = x.RequestorUserName,
                    Status = x.Status,
                    Title = x.Title,
                    Vote = x.Vote,
                    WorkflowInstanceId = x.WorkflowInstanceId,
                    Created = x.Created,
                    DueDate = x.DueDate,
                    RequestedDepartmentId = x.RequestedDepartmentId,
                    RequestedDepartmentName = x.RequestedDepartmentName,
                    RequestedDepartmentCode = x.RequestedDepartmentCode
                })
                .ToList();

                _logger.LogInformation("Time new_edoc1_tasks: " + DateTimeOffset.Now.ToString("HH:mm:ss"));
                #region get current task
                var queryNewTaskEdoc1Args = new Edoc1ArgV2
                {
                    LoginName = args.LoginName,
                    Page = 1,
                    Limit = 10000
                };

                if (args.FromDueDate != null)
                    queryNewTaskEdoc1Args.FromDueDate = args.FromDueDate;
                if (args.ToDueDate != null)
                    queryNewTaskEdoc1Args.ToDueDate = args.ToDueDate;

                var new_edoc1_tasks = await _edoc01.GetTasksV2(queryNewTaskEdoc1Args);
                new_edoc1_tasks = new_edoc1_tasks.Select(x => new WorkflowTaskViewModel
                {
                    ReferenceNumber = x.ReferenceNumber,
                    IsCompleted = x.IsCompleted,
                    ItemId = x.ItemId,
                    ItemType = x.ItemType,
                    Link = x.Module.Equals("Edoc1") 
                        ? (!string.IsNullOrEmpty(x.Link) ? (x.Link.Replace("/fin/", subFinSiteUrl)) : "") // :
                          // x.Module.Equals("LiquorLicense") ? (!string.IsNullOrEmpty(x.Link) ? (baseUrl + x.Link.Replace("/Liquor/", subLiquorSiteUrl)) : "") 
                          : (x.Link.Contains("http") ? x.Link : (baseUrl + x.Link)),
                    Module = x.Module,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    RequestorFullName = x.RequestorFullName,
                    RequestorId = x.RequestorId,
                    RequestorUserName = x.RequestorUserName,
                    Status = x.Status,
                    Title = x.Title,
                    Vote = x.Vote,
                    WorkflowInstanceId = x.WorkflowInstanceId,
                    Created = x.Created,
                    DueDate = x.DueDate,
                    RequestedDepartmentId = x.RequestedDepartmentId,
                    RequestedDepartmentName = x.RequestedDepartmentName,
                    RequestedDepartmentCode = x.RequestedDepartmentCode

                })
                .ToList();
                #endregion

                // var new_edoc1_tasks = await _edoc01.GetTasksV2(queryNewTaskEdoc1Args);
                _logger.LogInformation("Time End Filter: " + DateTimeOffset.Now.ToString("HH:mm:ss"));
                var allData = (edoc2_tasks.Concat(moduleTasks).Concat(facilityTasks).Concat(tradeContractTask).Concat(skuTasks).Concat(new_edoc1_tasks));

                if (args.FromDueDate != null)
                {
                    allData = allData.Where(x => x.DueDate != null && x.DueDate.Value.Date >= args.FromDueDate.Value.Date);
                }

                if (args.ToDueDate != null)
                {
                    allData = allData.Where(x => x.DueDate != null && x.DueDate.Value.Date <= args.ToDueDate.Value.Date);
                }

                if (args.FromCreatedDate != null)
                {
                    allData = allData.Where(x => x.Created != null && x.Created.Value.Date >= args.FromCreatedDate.Value.Date);
                }

                if (args.ToCreatedDate != null)
                {
                    allData = allData.Where(x => x.Created != null && x.Created.Value.Date <= args.ToCreatedDate.Value.Date);
                }

                if (!string.IsNullOrEmpty(args.Keyword))
                {
                    allData = allData.Where(x => !string.IsNullOrEmpty(x.ReferenceNumber) && x.ReferenceNumber.ToLower().Contains(args.Keyword.ToLower()));
                }

                if (!string.IsNullOrEmpty(args.Module))
                {
                    // Them cai module
                    if (args.Module.Contains(","))
                    {
                        var modules = args.Module.ToLower().Replace(" ", "").Split(',');
                        allData = allData.Where(x => modules.Contains(x.Module.ToLower()));
                    }
                    else
                    {
                        allData = allData.Where(x => !string.IsNullOrEmpty(x.Module) && x.Module.ToLower().Equals(args.Module.ToLower()));
                    }
                }
                var count = allData.Count();
                allData = allData.Skip((args.Page - 1) * args.Limit).Take(args.Limit);
                _logger.LogInformation("Time Start: " + DateTimeOffset.Now.ToString("HH:mm:ss"));
                return new ResultDTO()
                {
                    Object = new { CountAll = count, Count = allData.Count(), Items = allData.OrderBy(x => x.Created).ToList() }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new ResultDTO()
                {
                    Messages = new List<string>() { ex.Message },
                    ErrorCodes = new List<int>() { -1 }
                };
            }
        }

        private class WorkflowItem
        {
            public WorkflowEntity Entity { get; set; }
            public string Type { get; set; }
        }

        public void SendEmailNotificationForApprover(EmailTemplateName type, EdocTaskViewModel task)
        {
            try
            {
                _logger.LogInformation($"Send email approver to {task.User.UserEmail}");
                var mergeFields = new Dictionary<string, string>();
                mergeFields["ApproverName"] = task.User.UserFullName;
                var bizTypesEdoc1 = task.Edoc1Tasks.Select(x => x.ItemType).Distinct();
                var bizTypesEdoc2 = task.Edoc2Tasks.Select(x => x.ItemType).Distinct();
                Utilities.UpdateMergeField(mergeFields, bizTypesEdoc1, true);
                Utilities.UpdateMergeField(mergeFields, bizTypesEdoc2, false);
                var recipients = new List<string>() { task.User.UserEmail };
                var ccRecipients = new List<string>();
                if (task.CCUser != null)
                {
                    ccRecipients.Add(task.CCUser.UserEmail);
                }
                _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients, null, ccRecipients).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        public async Task SendEmaiAdminChecker(EmailTemplateName type, WorkflowInstance wfInstance, WorkflowStep nextStep, Guid? adminCheckerDeptId)
        {
            try
            {
                if (adminCheckerDeptId != null)
                {
                    _logger.LogInformation($"BTA - Send email approver to Admin Checker - " + wfInstance.ItemReferenceNumber);
                    var mergeFields = new Dictionary<string, string>();
                    mergeFields["BusinessTripApplicationNumber"] = wfInstance.ItemReferenceNumber;
                    var recipients = GetListEmailAddressAminChecker(adminCheckerDeptId, nextStep);
                    await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients);
                }
                else
                {
                    _logger.LogInformation($"BTA - SendEmaiAdminChecker: Admin Checker Department Id is null.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SendEmaiAdminChecker: " + ex.Message, ex);
            }
        }
        public List<string> GetListEmailAddressAminChecker(Guid? adminCheckerDeptId, WorkflowStep nextStep)
        {
            var result = new List<string>();
            try
            {
                var userDepartmentMapping = _uow.GetRepository<UserDepartmentMapping>().FindBy<UserDepartmentMappingViewModel>(x => x.DepartmentId == adminCheckerDeptId && x.Role == nextStep.DepartmentType);
                if (userDepartmentMapping.Any())
                {
                    foreach (var currentUser in userDepartmentMapping)
                    {
                        result.Add(currentUser.UserEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;
        }

        public async Task<ResultDTO> SyncWorkflowById(Guid id)
        {
            var result = new ResultDTO();
            try
            {
                var workflowInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == id);
                if (workflowInstance != null)
                {
                    var workflowTemplate = await _uow.GetRepository<WorkflowTemplate>().GetSingleAsync(x => x.Id == workflowInstance.TemplateId);
                    if (workflowTemplate != null)
                    {
                        workflowInstance.WorkflowDataStr = workflowTemplate.WorkflowDataStr;
                        _uow.GetRepository<WorkflowInstance>().Update(workflowInstance);
                        await _uow.CommitAsync();
                        result.Object = Mapper.Map<WorkflowInstanceViewModel>(workflowInstance);
                    }
                }
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int> { -1 };
                result.Messages = new List<string> { e.Message };
            }
            return result;
        }

        public async Task<ResultDTO> UpdateAssignToByItemId(UpdateAssignToWorkflowArgs args)
        {
            var result = new ResultDTO();
            try
            {
                bool error = true;
                if (string.IsNullOrEmpty(args.Type))
                {
                    error = false;
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Type is not null!" };
                }
                if (args.Type.Equals(ItemTypeContants.User) && !args.AssignToId.HasValue)
                {
                    error = false;
                    result.ErrorCodes = new List<int>() { -1 };
                    result.Messages = new List<string>() { "Assign To is not null!" };
                }
                else
                {
                    if (!args.AssignToId.HasValue && !args.AssignToGroup.HasValue)
                    {
                        error = false;
                        result.ErrorCodes = new List<int>() { -1 };
                        result.Messages = new List<string>() { "Assign To and Assign To Group is not null!" };
                    }
                }
                if (!error) goto Finish;

                var workflowInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == args.InstanceId);
                if (!(workflowInstance is null) && !workflowInstance.IsCompleted)
                {
                    // task
                    var currentTask = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == workflowInstance.Id, "Created desc");
                    if (!(currentTask is null) && !currentTask.IsCompleted)
                    {
                        var permissions = await _uow.GetRepository<Permission>().GetSingleAsync(x => x.ItemId == currentTask.Id, "Created desc");
                        if (!(permissions is null))
                        {
                            if (args.Type.Equals(ItemTypeContants.Department))
                            {
                                if (args.AssignToId.HasValue)
                                    permissions.DepartmentId = args.AssignToId;
                                if (args.AssignToGroup.HasValue)
                                    permissions.DepartmentType = (Group)args.AssignToGroup;
                            }
                            else
                            {
                                permissions.UserId = args.AssignToId;
                            }
                        }

                        int? stepNumber = null;
                        var currentHistories = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == workflowInstance.Id, "Created desc");
                        if (!(currentHistories is null) && !currentHistories.IsStepCompleted)
                        {
                            if (args.Type.Equals(ItemTypeContants.Department))
                            {
                                if (args.AssignToId.HasValue)
                                    currentHistories.AssignedToDepartmentId = args.AssignToId;
                                if (args.AssignToGroup.HasValue)
                                    currentHistories.AssignedToDepartmentType = (Group)args.AssignToGroup;
                            }
                            else
                            {
                                currentHistories.AssignedToUserId = args.AssignToId;
                            }
                            stepNumber = currentHistories.StepNumber;
                        }

                        if (args.Type.Equals(ItemTypeContants.Department))
                        {
                            if (args.AssignToId.HasValue)
                                currentTask.AssignedToDepartmentId = args.AssignToId;
                            if (args.AssignToGroup.HasValue)
                                currentTask.AssignedToDepartmentGroup = (Group) args.AssignToGroup;

                            var status = await this.GetStatusItem(workflowInstance, stepNumber.Value, args.AssignToId.Value, false);
                            if (!string.IsNullOrEmpty(status))
                            {
                                currentTask.Status = status;
                                // update item
                                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                                var itemTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                                foreach (var itemType in itemTypes)
                                {
                                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", itemType);
                                    var item = (IWorkflowEntity)(await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", workflowInstance.ItemId));
                                    if (item != null && !string.IsNullOrEmpty(status))
                                    {
                                        item.Status = status;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentTask.AssignedToId = args.AssignToId;
                        }

                        // Update permission for Item
                        if (currentTask.AssignedToDepartmentId.HasValue)
                        {
                            var permissionCurrentItem = await _uow.GetRepository<Permission>().GetSingleAsync(x => x.ItemId == currentTask.ItemId && x.DepartmentId == args.AssignFromId && x.DepartmentType == ((Group) args.AssignFromGroup) , "created desc");
                            if (!(permissionCurrentItem is null))
                            {
                                permissionCurrentItem.DepartmentId = currentTask.AssignedToDepartmentId;
                                permissionCurrentItem.DepartmentType = currentTask.AssignedToDepartmentGroup;
                            }
                        } else if (currentTask.AssignedToId.HasValue)
                        {
                            var permissionCurrentItem = await _uow.GetRepository<Permission>().GetSingleAsync(x => x.ItemId == currentTask.ItemId && !x.DepartmentId.HasValue && x.DepartmentType == 0, "created desc");
                            if (!(permissionCurrentItem is null))
                            {
                                permissionCurrentItem.UserId = currentTask.AssignedToId;
                            }
                        }

                        await _uow.CommitAsync();
                        await PushNotificationForCurrentStep("Edoc2", "APPROVE", workflowInstance.ItemId);
                        result.Object = Mapper.Map<WorkflowTaskViewModel>(currentTask);
                    }
                }
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int>() { -1 };
                result.Messages = new List<string>() { e.Message };
            }
        Finish:
            return result;
        }

        public async Task<ResultDTO> ChangeStepWorkflow(ChangeStepWorkflowArgs args)
        {
            var result = new ResultDTO();
            try
            {
                var workflowInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.Id == args.InstanceId);
                if (!(workflowInstance is null))
                {
                    var workflowHistories = await _uow.GetRepository<WorkflowHistory>().FindByAsync(x => x.InstanceId == workflowInstance.Id && x.StepNumber >= args.NewStepNumber, "Created Asc");
                    if (!(workflowHistories is null))
                    {
                        var countLineDeleteHistories = 0;
                        var currentApproverDepartmentId = Guid.Empty;
                        foreach (var itemHistory in workflowHistories)
                        {
                            if (itemHistory.StepNumber == args.NewStepNumber)
                            {
                                if (itemHistory.AssignedToDepartmentId.HasValue)
                                {
                                    currentApproverDepartmentId = itemHistory.AssignedToDepartmentId.Value;
                                }
                                itemHistory.ApproverId = null;
                                itemHistory.Approver = null;
                                itemHistory.ApproverFullName = null;
                                itemHistory.Outcome = null;
                                itemHistory.Comment = null;
                                itemHistory.VoteType = 0;
                                itemHistory.IsStepCompleted = false;
                                _uow.GetRepository<WorkflowHistory>().Update(itemHistory);
                            }
                            else
                            {
                                _uow.GetRepository<WorkflowHistory>().Delete(itemHistory);
                                countLineDeleteHistories++;
                            }
                        }

                        string status = null;
                        if (currentApproverDepartmentId != Guid.Empty)
                        {

                            status = await this.GetStatusItem(workflowInstance, args.NewStepNumber, currentApproverDepartmentId, true);
                            if (!string.IsNullOrEmpty(status))
                            {
                                // update item
                                var ass = Assembly.GetAssembly(typeof(WorkflowInstance));
                                var itemTypes = ass.GetTypes().Where(x => typeof(IWorkflowEntity).IsAssignableFrom(x));
                                foreach (var itemType in itemTypes)
                                {
                                    var repository = DynamicInvoker.InvokeGeneric(_uow, "GetRepository", itemType);
                                    var item = (IWorkflowEntity)(await DynamicInvoker.InvokeAsync(repository, "FindByIdAsync", workflowInstance.ItemId));
                                    if (item != null)
                                    {
                                        item.Status = status;
                                        break;
                                    }
                                }
                            }
                        }

                        if (countLineDeleteHistories > 0)
                        {
                            var deleteWorkflowTask = 0;
                            var workflowTask = await _uow.GetRepository<WorkflowTask>().FindByAsync(x => x.WorkflowInstanceId == workflowInstance.Id && x.ReferenceNumber == args.ItemReferenceNumber, "created desc");
                            if (!(workflowTask is null))
                            {
                                foreach (var itemTask in workflowTask)
                                {
                                    if (deleteWorkflowTask < countLineDeleteHistories)
                                    {
                                        _uow.GetRepository<WorkflowTask>().Delete(itemTask);
                                    }
                                    else
                                    {
                                        itemTask.Vote = 0;
                                        itemTask.IsCompleted = false;
                                        if (!string.IsNullOrEmpty(status))
                                        {
                                            itemTask.Status = status;
                                        }
                                        _uow.GetRepository<WorkflowTask>().Update(itemTask);
                                        break;
                                    }
                                    deleteWorkflowTask++;
                                }
                                if (deleteWorkflowTask > 0)
                                    await _uow.CommitAsync();

                                result.Object = deleteWorkflowTask;
                                // so line da delete thanh cong
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.ErrorCodes = new List<int>() { -1 };
                result.Messages = new List<string>() { e.Message };
            }
            return result;
        }

        private async Task<string> GetStatusItem(WorkflowInstance workflowInstance, int stepName, Guid DepartmentId, bool isUpdateStepName)
        {
            var status = "";
            // update workflow task
            var workflowData = workflowInstance.WorkflowData is null ? null : workflowInstance.WorkflowData;
            if (!(workflowData is null) && workflowData.Steps.Any())
            {
                // truong hop 2 step number trung nhau khong update
                var workflowStep = workflowData.Steps.Where(x => x.StepNumber == stepName).ToList();
                if (workflowStep.Count() == 1)
                {
                    if (workflowStep[0].IsStatusFollowStepName)
                    {
                        if (isUpdateStepName)
                        {
                            status = "Waiting For " + (!string.IsNullOrEmpty(workflowStep[0].StepName) ? workflowStep[0].StepName : "");
                        }
                    }
                    else
                    {
                        var department = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == DepartmentId);
                        if (!(department is null))
                        {
                            status = "Waiting For " + department.PositionName + " Approval";
                        }
                    }
                }
            }
            return status;
        }

        public async Task<ResultDTO> VoteAdminBTA(VoteArgs args)
        {
            try
            {
                var item = await GetWorkflowItem(args.ItemId);
                if (item == null)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                        "Item not found!"

                    }
                    };
                }
                var wfInstance = await _uow.GetRepository<WorkflowInstance>().GetSingleAsync(x => x.ItemId == args.ItemId, "Created desc");
                if (wfInstance.IsCompleted)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                        "Workflow is completed!"

                    }
                    };
                }
                var lastHistory = await _uow.GetRepository<WorkflowHistory>().GetSingleAsync(x => x.InstanceId == wfInstance.Id, "Created desc");
                if (lastHistory == null || lastHistory.IsStepCompleted)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                            "Cannot find any running workflow !"
                        }
                    };
                }

                var ignorePerm = true;
                //ignore perm to get the latest task incase user perform cancel action                                      
                var currentTask = await _uow.GetRepository<WorkflowTask>(ignorePerm).GetSingleAsync(x => x.ItemId == item.Entity.Id && x.WorkflowInstanceId == wfInstance.Id && !x.IsCompleted, "Created desc");

                //If user cancel incase of pending, the tasks was not generated
                if (currentTask == null && args.Vote != VoteType.Cancel)
                {
                    return new ResultDTO()
                    {
                        Messages = new List<string>() {
                            "Cannot find any running workflow !"
                        }
                    };
                }
                if (currentTask != null)
                {
                    currentTask.IsCompleted = true;
                    currentTask.Vote = args.Vote;
                }
                var currentStep = wfInstance.WorkflowData.Steps.FirstOrDefault(x => x.StepNumber == lastHistory.StepNumber);
                lastHistory.VoteType = args.Vote;
                lastHistory.Modified = DateTime.Now;
                lastHistory.Comment = args.Comment;
                lastHistory.ApproverId = item.Entity.CreatedById;
                lastHistory.Approver = item.Entity.CreatedBy;
                lastHistory.ApproverFullName = item.Entity.CreatedByFullName;
                lastHistory.IsStepCompleted = true;
                WorkflowStep nextStep = null;
                if (args.Vote == VoteType.Approve)
                {
                    lastHistory.Outcome = currentStep.OnSuccess;
                    nextStep = wfInstance.WorkflowData.Steps.GetNextStep(currentStep, item.Entity, item.Entity.GetType());
                }
                await UpdatePermission(item.Entity.Id, Right.View);
                await _uow.CommitAsync();
                await ProcessNextStep(wfInstance, item, lastHistory, nextStep);
                await PushNotificationForCurrentStep("Edoc2", "APPROVE", wfInstance.ItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new ResultDTO()
                { Messages = new List<string>() { ex.Message } };
            }
            return new ResultDTO();
        }

        // Ignore cac ticket chua den thoi han phe duyet
        public List<string> IgnoreReferenceNumberActing()
        {
            #region Get Ignore Acting Items
            List<string> ignoreStatus = new List<string>() { "Completed", "Draft", "Cancelled", "Rejected", "Requested To Change" };

            var processingActingItem = _uow.GetRepository<Acting>(true).FindBy(x => !ignoreStatus.Contains(x.Status)).ToList();
            Func<DateTimeOffset?, bool> doesAllowShowTodoList = (DateTimeOffset? periodEnd) =>
            {
                bool blReturnValue = false;
                try
                {
                    if (periodEnd.HasValue && (DateTime.Now.ToUniversalTime() < periodEnd.Value.AddDays(-15).ToUniversalTime() || DateTime.Now.ToUniversalTime() > periodEnd.Value.AddDays(1).ToUniversalTime()))
                    {
                        blReturnValue = true;
                    }
                }
                catch
                {
                    blReturnValue = false;
                }
                return blReturnValue;
            };
            List<string> referenceNumberActingValid = new List<string>();
            foreach (var act in processingActingItem)
            {
                if (act.Period4To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                {
                    if (doesAllowShowTodoList(act.Period4To))
                        referenceNumberActingValid.Add(act.ReferenceNumber);
                }
                else
                {
                    if (act.Period3To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                    {
                        if (doesAllowShowTodoList(act.Period3To))
                            referenceNumberActingValid.Add(act.ReferenceNumber);
                    }
                    else
                    {
                        if (act.Period2To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                        {
                            if (doesAllowShowTodoList(act.Period2To))
                                referenceNumberActingValid.Add(act.ReferenceNumber);
                        }
                        else
                        {
                            if (act.Period1To.HasValue && (act.Status == "Waiting for Appraiser 1" || act.Status == "Waiting for Appraiser 2"))
                            {
                                if (doesAllowShowTodoList(act.Period1To))
                                    referenceNumberActingValid.Add(act.ReferenceNumber);
                            }
                        }
                    }
                }
            }
            #endregion
            return referenceNumberActingValid;
        }

        public async Task<ResultDTO> ReFindAssignToCurrentStep(Guid InstanceId)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var workflowInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.Id == InstanceId, "created desc");
                if (workflowInstance is null)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Cannot find any round!" };
                    goto Finish;
                }
                if (workflowInstance.IsCompleted)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Current round is already completed!" };
                    goto Finish;
                }

                var lastHistory = await _uow.GetRepository<WorkflowHistory>(true).GetSingleAsync(x => x.InstanceId == workflowInstance.Id, "created desc");
                if (lastHistory.IsStepCompleted)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Workflow histories is already completed!" };
                    goto Finish;
                }

                var lastWorkflowTask = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == workflowInstance.Id, "created desc");
                var count_task = await _uow.GetRepository<WorkflowTask>().CountAsync(x => x.WorkflowInstanceId == workflowInstance.Id);
                var count_his = await _uow.GetRepository<WorkflowHistory>().CountAsync(x => x.InstanceId == workflowInstance.Id);
                if (lastWorkflowTask is null || (lastWorkflowTask != null && count_task == count_his && lastWorkflowTask.IsCompleted && lastHistory.IsStepCompleted))
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Cannot status task is not pending!" };
                    goto Finish;
                }
                
                
                var PrevlastHistory = await _uow.GetRepository<WorkflowHistory>(true).GetSingleAsync(x => x.InstanceId == workflowInstance.Id && x.StepNumber == lastHistory.StepNumber - 1, "created desc"); ;
                
                var nextStep = workflowInstance.WorkflowData.Steps.Where(x => x.StepNumber == lastHistory.StepNumber).FirstOrDefault();
                if (nextStep is null)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Cannot find current step!" };
                    goto Finish;
                }

                var wfItem = await GetWorkflowItem(workflowInstance.ItemId);
                var item = wfItem.Entity;
                var itemPros = await ExtractItemProperties(item);
                var rqDepartment = await GetRequestDepartment(item.CreatedById.Value, itemPros, workflowInstance.WorkflowData, nextStep);
                Guid? departmentId = null;
                Guid? userId = null;

                switch (nextStep.ParticipantType)
                {
                    case ParticipantType.SpecificUser:
                        var user = await _uow.GetRepository<User>().FindByIdAsync(nextStep.UserId.Value);
                        if (!(user == null || !user.IsActivated))
                        {
                            userId = user.Id;
                            departmentId = nextStep.DepartmentId;
                        }
                        break;
                    case ParticipantType.CurrentDepartment:
                        Department dept = null;
                        if (lastHistory == null)
                        {
                            //Get current dept of user
                            dept = rqDepartment;
                        }
                        else
                        {
                            dept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == PrevlastHistory.AssignedToDepartmentId);
                        }
                        if (dept != null)
                        {
                            var hasParticipants = await _uow.GetRepository<UserDepartmentMapping>().AnyAsync(x => x.DepartmentId == dept.Id && nextStep.DepartmentType == x.Role);
                            if (hasParticipants)
                            {
                                departmentId = dept.Id;
                            }
                            else
                            {
                                _refDeparmentId = dept.Id;
                            }
                        }
                        break;
                    case ParticipantType.SpecificDepartment:
                        var specDept = await _uow.GetRepository<Department>().GetSingleAsync(x => x.Id == nextStep.UserId.Value);
                        if (specDept != null)
                        {
                            departmentId = specDept.Id;
                        }
                        break;
                    case ParticipantType.UpperDepartment:
                        Department upperDept = await GetUpperDept(PrevlastHistory, nextStep, rqDepartment);
                        if (upperDept != null)
                        {
                            departmentId = upperDept.Id;
                        }
                        break;
                    case ParticipantType.DepartmentLevel:
                        Department lvlDept = await GetLevelDept(PrevlastHistory, nextStep, rqDepartment);
                        if (lvlDept != null)
                        {
                            departmentId = lvlDept.Id;
                        }
                        break;
                    case ParticipantType.ItemUserField:
                        Guid userGuid;
                        if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                        {
                            if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out userGuid))
                            {
                                userId = userGuid;
                            }
                        }
                        break;
                    case ParticipantType.ItemDepartmentField:
                        Guid departmentGuid;
                        if (itemPros.ContainsKey(nextStep.DataField?.Trim().ToLowerInvariant()))
                        {

                            if (Guid.TryParse(itemPros[nextStep.DataField?.Trim().ToLowerInvariant()], out departmentGuid))
                            {
                                departmentId = departmentGuid;
                            }
                        }
                        break;
                    case ParticipantType.HRDepartment:
                        Department hrDepartment = await GetHrDept(PrevlastHistory, nextStep, rqDepartment);
                        if (hrDepartment != null)
                        {
                            departmentId = hrDepartment.Id;
                        }
                        break;
                    case ParticipantType.HRManagerDepartment:
                        Department hrManagerDepartment = GetHrManagerDept(PrevlastHistory, nextStep, rqDepartment);
                        if (hrManagerDepartment != null)
                        {
                            departmentId = hrManagerDepartment.Id;
                        }
                        break;
                    case ParticipantType.PerfomanceDepartment:
                        Department perfDepartment = await GetPerfDept(PrevlastHistory, nextStep, rqDepartment);
                        if (perfDepartment != null)
                        {
                            departmentId = perfDepartment.Id;
                        }
                        break;
                    case ParticipantType.AdminDepartment:
                        Department adminDepartment = await GetAdminDept(PrevlastHistory, nextStep, rqDepartment);
                        if (adminDepartment != null)
                        {
                            departmentId = adminDepartment.Id;
                        }
                        break;
                    case ParticipantType.CBDepartment:
                        Department cbDepartment = await GetCBDept(PrevlastHistory, nextStep, rqDepartment);
                        if (cbDepartment != null)
                        {
                            departmentId = cbDepartment.Id;
                        }
                        break;
                    case ParticipantType.Appraiser1Department:
                        Department appraiser1Department = await GetAppraiser1Department(PrevlastHistory, nextStep, rqDepartment);
                        if (appraiser1Department != null)
                        {
                            departmentId = appraiser1Department.Id;
                        }
                        break;
                    case ParticipantType.Appraiser2Department:
                        Department appraiser2Department = await GetAppraiser2Department(PrevlastHistory, nextStep, rqDepartment);
                        if (appraiser2Department != null)
                        {
                            departmentId = appraiser2Department.Id;
                        }
                        break;
                    case ParticipantType.BTABudgetApprover:
                        rqDepartment = await this.GetBeforeStepBookingFlightBTA(item.Id);
                        upperDept = await GetUpperDept(null, nextStep, rqDepartment);
                        if (upperDept != null)
                        {
                            departmentId = upperDept.Id;
                        }
                        break;
                }

                if (departmentId == null || !departmentId.HasValue || departmentId == Guid.Empty)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "Cannot find department!" };
                    goto Finish;
                }

                await UpdateItemStatus(nextStep, item, userId, departmentId);

                #region Update task
                if(!lastHistory.AssignedToDepartmentId.HasValue)
                {
                    lastHistory.AssignedToDepartmentId = departmentId;
                    lastHistory.AssignedToDepartmentType = nextStep.DepartmentType;
                }

                await AssignTaskAndPermission(workflowInstance, wfItem, nextStep, item, userId, departmentId, rqDepartment);
                #endregion

                await _uow.CommitAsync();

                var finalWorkflowTask = await _uow.GetRepository<WorkflowTask>().GetSingleAsync<WorkflowTaskViewModel>(x => x.WorkflowInstanceId == workflowInstance.Id, "created desc");

                resultDto.Object = finalWorkflowTask;
            Finish:
                return resultDto;
            }
            catch (Exception ex)
            {
                return new ResultDTO() { Messages = new List<string>() { ex.Message } };
            }
        }

        public async Task<ResultDTO> CheckReFindAssign(Guid InstanceId)
        {
            var resultDto = new ResultDTO() { };
            try
            {
                var workflowInstance = await _uow.GetRepository<WorkflowInstance>(true).GetSingleAsync(x => x.Id == InstanceId, "created desc");
                
                var count_task = await _uow.GetRepository<WorkflowTask>().CountAsync(x => x.WorkflowInstanceId == workflowInstance.Id);
                var count_his = await _uow.GetRepository<WorkflowHistory>().CountAsync(x => x.InstanceId == workflowInstance.Id);

                if(count_his != count_task)
                {
                    resultDto.ErrorCodes = new List<int>() { -1 };
                    resultDto.Messages = new List<string>() { "status task is pending!" };
                    goto Finish;
                }

                var lastTask = await _uow.GetRepository<WorkflowTask>().GetSingleAsync(x => x.WorkflowInstanceId == workflowInstance.Id, "created desc");
                resultDto.Object = lastTask;

            Finish:
                return resultDto;
            }
            catch (Exception ex)
            {
                return new ResultDTO() { Messages = new List<string>() { ex.Message } };
            }
        }
    }
}
