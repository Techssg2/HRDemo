using JobSendMailApproverNotificationsV2.src.Args;
using JobSendMailApproverNotificationsV2.src.ConfigAPI;
using JobSendMailApproverNotificationsV2.src.DTOs;
using JobSendMailApproverNotificationsV2.src.Entity;
using JobSendMailApproverNotificationsV2.src.Enums;
using JobSendMailApproverNotificationsV2.src.Helpers;
using JobSendMailApproverNotificationsV2.src.Model;
using JobSendMailApproverNotificationsV2.src.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2.src
{
    public class JobSendMailApproverNotifications
    {
        private readonly WorkflowTaskService _workflowTaskService;
        private readonly UserService _userService;
        private readonly ITUserService _itUserService;
        private readonly DepartmentService _departmentService;
        private readonly UserDepartmentMappingService _userDepartmentMappingService;
        private readonly ActingService _actingService;

        private readonly SAPSettingsSection _edoc1Setting;
        private readonly SAPSettingsSection _edoc1SettingV2;
        private readonly TradeContractSettingsSection _tradeContractSetting;

        private string RootUrlEdoc1 = ConfigurationManager.AppSettings["RootUrlEdoc1"];
        private string SubDomainLiquor = ConfigurationManager.AppSettings["SubDomainLiquor"];
        private string API_DEFAULT_SAF = ConfigurationManager.AppSettings["ApiDefaultSAF"];
        private string API_DEFAULT_DS = ConfigurationManager.AppSettings["ApiDefaultDS"];

        private readonly SmtpSection _section;

        // Constructor
        public JobSendMailApproverNotifications()
        {
            _workflowTaskService = new WorkflowTaskService();
            _departmentService = new DepartmentService();
            _userDepartmentMappingService = new UserDepartmentMappingService();
            _userService = new UserService();
            _itUserService = new ITUserService();
            _actingService = new ActingService();

            _tradeContractSetting = (TradeContractSettingsSection)ConfigurationManager.GetSection("aeonTradeContractSettings");
            _edoc1Setting = (SAPSettingsSection)ConfigurationManager.GetSection("aeonPhase1Settings");
            _edoc1SettingV2 = (SAPSettingsSection)ConfigurationManager.GetSection("aeonPhase1SettingsV2");

            _section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }

        public async Task Run()
        {
            await StartJobSendMailApproverNotifications();
        }

        public async Task StartJobSendMailApproverNotifications()
        {
            try
            {
                Utilities.WriteLogError("SendNotifications Approver Notification Job started");

                var edoc2Tasks = new List<EdocTaskModel>();
                var groupDept = new List<NotificationUserModel>();

                List<WorkflowTaskEntity> tasks = _workflowTaskService.GetTask();

                var deptTasks = tasks.Where(x => x.AssignedToDepartmentId.HasValue);
                foreach (var deptTask in deptTasks)
                {
                    if (!groupDept.Any(x => x.DepartmentId == deptTask.AssignedToDepartmentId && (int)x.DepartmentGroup == deptTask.AssignedToDepartmentGroup))
                    {
                        var users = _userService.GetUsers(deptTask.AssignedToDepartmentId.Value, deptTask.AssignedToDepartmentGroup);

                        foreach (var user in users)
                        {
                            groupDept.Add(new NotificationUserModel
                            {
                                UserFullName = user.FullName,
                                UserEmail = user.Email,
                                UserId = user.Id,
                                DepartmentId = deptTask.AssignedToDepartmentId.Value,
                                DepartmentGroup = (Group)deptTask.AssignedToDepartmentGroup
                            });
                        }
                    }
                }

                var assistanceNodes = _departmentService.GetAssistanceNodes();
                var userMappingDepartments = _userDepartmentMappingService.GetUserMappingsForAssistanceNodes(assistanceNodes);

                var allUserIds = groupDept.Select(x => x.UserId).Distinct().ToList();
                var uIds = tasks.Where(x => x.AssignedToId.HasValue).Select(x => x.AssignedToId.Value).ToList();

                foreach (var uId in uIds)
                {
                    if (!allUserIds.Contains(uId))
                    {
                        allUserIds.Add(uId);
                    }
                }

                foreach (var userId in allUserIds)
                {
                    var notiUser = groupDept.FirstOrDefault(x => x.UserId == userId);
                    if (notiUser != null)
                    {
                        var userTasks = tasks.Where(x => x.AssignedToId == userId || (x.AssignedToDepartmentId == notiUser.DepartmentId &&
                       x.AssignedToDepartmentGroup == (int)notiUser.DepartmentGroup)).Select(x => new WorkflowTaskModel
                       {
                           Title = x.Title,
                           ItemId = x.ItemId,
                           ItemType = x.ItemType,
                           ReferenceNumber = x.ReferenceNumber,
                           DueDate = x.DueDate,
                           Status = x.Status,
                           Vote = (VoteType)x.Vote,
                           RequestedDepartmentId = x.RequestedDepartmentId,
                           RequestedDepartmentCode = x.RequestedDepartmentCode,
                           RequestedDepartmentName = x.RequestedDepartmentName,
                           RequestorId = x.RequestorId,
                           RequestorUserName = x.RequestorUserName,
                           RequestorFullName = x.RequestorFullName,
                           IsCompleted = x.IsCompleted,
                           WorkflowInstanceId = x.WorkflowInstanceId,
                           Created = x.Created,
                       }).ToList();

                        edoc2Tasks.Add(new EdocTaskModel
                        {
                            User = notiUser,
                            Edoc2Tasks = userTasks
                        });
                    }
                }

                ////qle24: add more tasks from new modules
                var moduleTasks = JobDashboardHelper.GetJobTasks();
                foreach (var moduleTask in moduleTasks)
                {
                    edoc2Tasks.Add(new EdocTaskModel
                    {
                        User = new NotificationUserModel
                        {
                            UserId = moduleTask.User.UserId,
                            UserFullName = moduleTask.User.UserFullName,
                            UserEmail = moduleTask.User.UserEmail,
                            DepartmentId = moduleTask.User.DepartmentId,
                        },
                        Edoc2Tasks = moduleTask.Edoc2Tasks
                    });
                }

                try
                {
                    #region
                    // ignore acting dang phe duyet chua den ky
                    List<string> ignoreReferenceNumberActing = _actingService.GetIgnoreReferenceNumbers();
                    if (ignoreReferenceNumberActing.Any())
                    {
                        edoc2Tasks = edoc2Tasks.Where(x => x.Edoc2Tasks.Any() && x.Edoc2Tasks.Where(y => !ignoreReferenceNumberActing.Contains(y.ReferenceNumber)).Any()).ToList();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError("Error filtering ignored tasks: " + ex.Message);
                }

                var edocTasks = await GetTasks(edoc2Tasks);
                if (edocTasks.Any())
                {
                    foreach (var task in edocTasks)
                    {
                        task.CCUser = GetAssistanceUser(task.User, assistanceNodes, userMappingDepartments);
                        Utilities.WriteLogError("\n");
                        Utilities.WriteLogError("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        await SendEmailNotificationForApprover(EmailTemplateName.ForApprover, task);
                        Utilities.WriteLogError("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error in StartJobSendMail: " + ex.Message);
            }
        }

        #region Func
        private async Task<List<EdocTaskModel>> GetTasks(List<EdocTaskModel> edoc2Tasks)
        {
            var result = new List<EdocTaskModel>();
            var lockObject = new object();

            try
            {
                var allUsers = _userService.GetActiveUsers();
                Utilities.WriteLogError($"GetTasks: Found {allUsers.Count} active users");

                var userNotInAll = edoc2Tasks.Where(x => !allUsers.Any(y => x.User.UserId == y.ID));
                if (userNotInAll.Any())
                {
                    result.AddRange(userNotInAll);
                }

                var maxRequest = 10;
                var semaphore = new SemaphoreSlim(maxRequest, maxRequest);
                var tasks = allUsers.Select(async user =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await ProcessSingleUser(user, edoc2Tasks);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var results = await Task.WhenAll(tasks);

                lock (lockObject)
                {
                    result.AddRange(results.Where(r => r != null));
                }
            }
            catch (Exception e)
            {
                Utilities.WriteLogError("GetTasks.Exception :" + e.Message);
            }

            return result;
        }
        private async Task<EdocTaskModel> ProcessSingleUser(UserEntity user, List<EdocTaskModel> edoc2Tasks)
        {
            try
            {
                var emailUser = _itUserService.GetUserEmailEdoc1(user);
                Utilities.WriteLogError("SendNotifications.GetTasks.GetUserEmailEdoc1 :" + user.SAPCode);

                var argV2 = new Edoc1ArgV2
                {
                    LoginName = user.LoginName,
                    Page = 1,
                    Limit = 1000
                };

                var allTasksV2 = await GetTasksV2(argV2);
                var tasksV2Approval = allTasksV2?.Where(task => task.Status != "Requested To Change")
                                                ?.Where(x => !string.IsNullOrEmpty(x.Module) && x.Module.Equals("Edoc1"))
                                                ?.ToList();

                var edoc2Task = edoc2Tasks.FirstOrDefault(x => x.User.UserId == user.Id);

                if (tasksV2Approval?.Any() == true || edoc2Task != null)
                {
                    var edocTask = new EdocTaskModel
                    {
                        User = edoc2Task?.User ?? new NotificationUserModel { UserFullName = user.FullName, UserEmail = emailUser, UserId = user.Id },
                        Edoc1Tasks = tasksV2Approval?.ToList(),
                        Edoc2Tasks = edoc2Task?.Edoc2Tasks ?? new List<WorkflowTaskModel>()
                    };

                    if (!string.IsNullOrEmpty(emailUser))
                    {
                        edocTask.User.UserEmail = emailUser;
                    }

                    return edocTask;
                }
            }
            catch (Exception e)
            {
                Utilities.WriteLogError($"ProcessSingleUser.Exception for user {user.SAPCode}: {e.Message}");
            }

            return null;
        }
        public async Task<List<WorkflowTaskModel>> GetTasks(Edoc1Arg arg)
        {
            var result = new List<WorkflowTaskModel>();
            var client = ConfigAPI();
            HttpResponseMessage response = null;
            try
            {
                var url = "GetTasks";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    Edoc1ResultModel<Edoc1TaskModel> res = JsonConvert.DeserializeObject<Edoc1ResultModel<Edoc1TaskModel>>(httpResponseResult);

                    if (res != null && res.Items.Any())
                    {
                        res.Items.ForEach(x =>
                        {
                            result.Add(new WorkflowTaskModel
                            {
                                Created = ConvertDateFromEdoc1Data(x.CreatedDate),
                                DueDate = ConvertDateFromEdoc1Data(x.DueDate),
                                RequestedDepartmentName = x.RequestedDepartmentName,
                                ReferenceNumber = x.ReferenceNumber,
                                Status = x.Status,
                                RequestorUserName = x.Requestor,
                                RequestorFullName = x.Requestor,
                                Title = x.ReferenceNumber,
                                Link = x.Link,
                                ItemType = GetItemTypeByLink(x.Link),
                                Module = "Edoc1"
                            });
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        public async Task<List<WorkflowTaskModel>> GetTasksV2(Edoc1ArgV2 arg)
        {
            var result = new List<WorkflowTaskModel>();
            var client = ConfigAPIV2();
            HttpResponseMessage response = null;
            try
            {
                var url = "/it/api/Partner/GetTasksByLoginName";
                var payload = JsonConvert.SerializeObject(arg);
                payload = ReplaceNull(payload);
                var content = Utilities.StringContentObjectFromJson(payload);
                response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    ResultDTO res = JsonConvert.DeserializeObject<ResultDTO>(httpResponseResult);
                    if (res != null && res.Object != null)
                    {
                        var deserializedTasks = JsonConvert.DeserializeObject<List<Edoc1TaskModelV2>>(res.Object.ToString());

                        var detailTask = deserializedTasks.Select(x => new Edoc1TaskModelV2
                        {
                            Id = x.Id,
                            Title = x.Title,
                            ItemId = x.ItemId,
                            ItemType = x.ItemType,
                            ReferenceNumber = x.ReferenceNumber,
                            DueDate = x.DueDate,
                            Status = x.Status,
                            Vote = x.Vote,
                            RequestedDepartmentId = x.RequestedDepartmentId,
                            RequestedDepartmentCode = x.RequestedDepartmentCode,
                            RequestedDepartmentName = x.RequestedDepartmentName,
                            RequestorId = x.RequestorId,
                            RequestorUserName = x.RequestorUserName,
                            RequestorFullName = x.RequestorFullName,
                            IsCompleted = x.IsCompleted,
                            WorkflowInstanceId = x.WorkflowInstanceId,
                            Created = x.Created,
                            Modified = x.Modified,
                            Link = x.Link,
                            RegionId = x.RegionId,
                            RegionName = x.RegionName,
                            Module = x.Module,
                            CreatedById = x.CreatedById,
                            CreatedBy = x.CreatedBy,
                            CreatedByFullName = x.CreatedByFullName,
                            IsParallelApprove = x.IsParallelApprove,
                            ParallelStep = x.ParallelStep,
                            IsSignOff = x.IsSignOff,
                            IsMultibudget = x.IsMultibudget,
                            IsManual = x.IsManual,
                            IsConfidentialContract = x.IsConfidentialContract,
                            DocumentSetPurpose = x.DocumentSetPurpose
                        }).ToList();

                        if (detailTask != null && detailTask.Any())
                        {
                            foreach (var task in detailTask)
                            {
                                var crTask = new WorkflowTaskModel
                                {
                                    Created = task.Created,
                                    DueDate = task.DueDate,
                                    RequestedDepartmentName = task.RequestedDepartmentName,
                                    ReferenceNumber = task.ReferenceNumber,
                                    Status = task.Status,
                                    RequestorUserName = task.RequestorUserName,
                                    RequestorFullName = task.RequestorFullName,
                                    ItemId = task.ItemId,
                                    Module = "Edoc1"
                                };
                                this.GetTitleLinkEdoc1(task, crTask);
                                if (!string.IsNullOrEmpty(crTask.Link) && !crTask.Module.Equals("TradeContract"))
                                    crTask.ItemType = GetItemTypeByLink(crTask.Link);

                                if (task.IsConfidentialContract != null && task.IsConfidentialContract == true)
                                {
                                    crTask.ReferenceNumber = crTask.ReferenceNumber + "-CONF";
                                }

                                result.Add(crTask);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        private string GetItemTypeByLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                if (link.Contains("form-contract") || link.Contains("form-contract-custom") || link.Contains("form-contract-multiF2"))
                {
                    return "Contract";
                }
                else if (link.Contains("form-non-expense-contract"))
                {
                    return "NonExpenseContract";
                }
                else if (link.Contains("form-payment"))
                {
                    return "Payment";
                }
                else if (link.Contains("form-advance"))
                {
                    return "Advance";
                }
                else if (link.Contains("form-reimbursement-payment"))
                {
                    return "ReimbursementPayment";
                }
                else if (link.Contains("form-reimbursement") || link.Contains("form-reimursement"))
                {
                    return "Reimbursement";
                }
                else if (link.Contains("form-purchase-multiItem") || link.Contains("form-purchase"))
                {
                    return "Purchase";
                }
                else if (link.Contains("form-transfer-cash"))
                {
                    return "TransferCash";
                }
                else if (link.Contains("form-refund-card"))
                {
                    return "RefundCard";
                }
                else if (link.Contains("form-credit-note"))
                {
                    return "CreditNote";
                }
                else if (link.Contains("/Tasks/TaskDetail"))
                {
                    return "Liquor/Task";
                }
                else if (link.Contains("/Project/Project"))
                {
                    return "Liquor/Project";
                }
                else if (link.Contains("/Document/Document"))
                {
                    return "Liquor/Document";
                }
                else if (link.Contains("/RetailLicense/RetailLicense"))
                {
                    return "Liquor/RetailLicense";
                }
            }
            return "";
        }
        void GetTitleLinkEdoc1(Edoc1TaskModelV2 taskEdocIT, WorkflowTaskModel taskEdocEdoc1)
        {
            if (taskEdocIT != null)
            {
                if (!string.IsNullOrEmpty(taskEdocIT.ReferenceNumber))
                {
                    if (taskEdocIT.ReferenceNumber.StartsWith("F2-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("F3-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("F4-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("CN-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RP-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("CA-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RE-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("PR-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("TF-") ||
                        taskEdocIT.ReferenceNumber.StartsWith("RF-"))
                    {
                        taskEdocEdoc1.Link = RootUrlEdoc1;
                        if (taskEdocIT.ReferenceNumber.StartsWith("F2-"))
                        {
                            taskEdocEdoc1.Title = "Purchasing";
                            if (taskEdocIT.IsMultibudget)
                            {
                                taskEdocEdoc1.Link += "/form-purchase-multiItem/";
                            }
                            else
                            {
                                taskEdocEdoc1.Link += "/form-purchase/";
                            }
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("F3-"))
                        {
                            taskEdocEdoc1.Title = "Contract";
                            if (taskEdocIT.IsManual)
                            {
                                taskEdocEdoc1.Link += "/form-contract-custom/";
                            }
                            else
                            {
                                taskEdocEdoc1.Link = RootUrlEdoc1 + "/form-contract-multiF2/";
                            }
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("F4-"))
                        {
                            taskEdocEdoc1.Link += "/form-non-expense-contract/";
                            taskEdocEdoc1.Title = "Non-Expense Contract";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("CN-"))
                        {
                            taskEdocEdoc1.Link += "/form-credit-note/";
                            taskEdocEdoc1.Title = "Credit Note";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("RP-"))
                        {
                            taskEdocEdoc1.Link += "/form-reimbursement-payment/";
                            taskEdocEdoc1.Title = "Reimbursement Payment";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("PR-"))
                        {
                            taskEdocEdoc1.Link += "/form-payment/";
                            taskEdocEdoc1.Title = "Payment";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("CA-"))
                        {
                            taskEdocEdoc1.Link += "/form-advance/";
                            taskEdocEdoc1.Title = "Advance";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("RE-"))
                        {
                            taskEdocEdoc1.Link += "/form-reimbursement/";
                            taskEdocEdoc1.Title = "Reimbursement";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("TF-"))
                        {
                            taskEdocEdoc1.Link += "/form-transfer-cash/";
                            taskEdocEdoc1.Title = "Transfer Cash";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("RF-"))
                        {
                            taskEdocEdoc1.Link += "/form-refund-card/";
                            taskEdocEdoc1.Title = "Refund Card";
                        }

                        taskEdocEdoc1.Link += taskEdocIT.ItemId.ToString().ToLower().Replace("-", "");
                    }
                    else if (taskEdocIT.ReferenceNumber.StartsWith("DOC-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("PJ-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("T-") ||
                            taskEdocIT.ReferenceNumber.StartsWith("RL-"))
                    {
                        taskEdocEdoc1.Link = SubDomainLiquor;
                        if (taskEdocIT.ReferenceNumber.StartsWith("DOC-"))
                        {
                            taskEdocEdoc1.Link += "/Document/Document?id=";
                            taskEdocEdoc1.Title = "Document";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("PJ-"))
                        {
                            taskEdocEdoc1.Link += "/Project/Project?id=";
                            taskEdocEdoc1.Title = "Project";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("T-"))
                        {
                            taskEdocEdoc1.Link += "/Tasks/TaskDetail?id=";
                            taskEdocEdoc1.Title = "Tasks";
                        }
                        else if (taskEdocIT.ReferenceNumber.StartsWith("RL-"))
                        {
                            taskEdocEdoc1.Link += "/RetailLicense/RetailLicense?id=";
                            taskEdocEdoc1.Title = "RetailLicense";
                        }
                        taskEdocEdoc1.Link += taskEdocIT.ItemId.ToString();
                        taskEdocEdoc1.Module = "LicenseLiquor";
                    }
                    else if (taskEdocIT.ReferenceNumber.StartsWith("SA-") || taskEdocIT.ReferenceNumber.StartsWith("SR-"))
                    {
                        taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocEdoc1.ReferenceNumber);
                        taskEdocEdoc1.Module = "TradeContract";
                        if (taskEdocIT.ReferenceNumber.StartsWith("SA-"))
                        {
                            taskEdocEdoc1.Title = "Supplier Application Form";
                            taskEdocEdoc1.ItemType = "Supplier Application Form";
                            taskEdocEdoc1.Module = "TradeContract";
                        }
                        else
                        {
                            taskEdocEdoc1.Title = "Supplier Audit Form";
                            taskEdocEdoc1.ItemType = "Supplier Audit Form";
                            taskEdocEdoc1.Module = "TradeContract";
                        }
                    }
                    else
                    {
                        taskEdocEdoc1.Title = "Document Set";
                        taskEdocEdoc1.ItemType = "Document Set";
                        taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_DS + taskEdocIT.ItemId);
                        taskEdocEdoc1.Module = "TradeContract";
                        if (taskEdocIT.DocumentSetPurpose != null)
                        {
                            if (taskEdocIT.DocumentSetPurpose == 1)
                            {
                                taskEdocEdoc1.ItemType = "Freeze Contract Code";
                                taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocIT.ReferenceNumber);
                            }
                            else if (taskEdocIT.DocumentSetPurpose == 2)
                            {
                                taskEdocEdoc1.ItemType = "Extend Trading Term";
                                taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_SAF + taskEdocIT.ReferenceNumber);
                            }
                            else if (taskEdocIT.DocumentSetPurpose == 0)
                            {
                                taskEdocEdoc1.Link = (_tradeContractSetting.BaseUrl + API_DEFAULT_DS + taskEdocIT.ItemId);
                            }
                        }
                    }
                }
            }
        }
        private DateTime ConvertDateFromEdoc1Data(string date)
        {
            //"/Date(1530440534143-0000)/";
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    var splitStringDate = date.Split('(');
                    var keys = splitStringDate[1].Split('-');
                    DateTime datetime = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(keys[0]));
                    return datetime.ToLocalTime();
                }
                catch (Exception ex)
                {
                    return DateTime.MinValue;
                }
            }
            return DateTime.MinValue;

        }
        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }
        private NotificationUserModel GetAssistanceUser(NotificationUserModel user, List<DepartmentEntity> assistanceNodes, List<UserDepartmentMappingEntity> userMappingDepartments)
        {
            NotificationUserModel r_user = null;
            try
            {
                if (user.DepartmentId != Guid.Empty && assistanceNodes != null && assistanceNodes.Any())
                {
                    var assistanceNode = assistanceNodes.FirstOrDefault(x => x.ParentId == user.DepartmentId);

                    if (assistanceNode != null)
                    {
                        var currentDepartmentMapping = userMappingDepartments.FirstOrDefault(x =>
                            x.DepartmentId == assistanceNode.ID &&
                            x.Role == (int)Group.Assistance &&
                            x.IsHeadCount);

                        if (currentDepartmentMapping != null)
                        {
                            r_user = new NotificationUserModel
                            {
                                UserId = currentDepartmentMapping.UserId,
                                UserEmail = currentDepartmentMapping.UserEmail,
                                DepartmentId = currentDepartmentMapping.DepartmentId,
                                UserFullName = currentDepartmentMapping.UserFullName
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("GetAssistanceUser: " + ex);
            }
            return r_user;
        }
        private async Task SendEmailNotificationForApprover(EmailTemplateName type, EdocTaskModel task)
        {
            try
            {
                Utilities.WriteLogError($"Send email approver to {task.User.UserEmail}");

                var mergeFields = new Dictionary<string, string>();
                mergeFields["ApproverName"] = task.User.UserFullName;

                var bizTypesEdoc1 = task.Edoc1Tasks.Select(x => x.ItemType).Distinct();
                var bizTypesEdoc2 = task.Edoc2Tasks.Select(x => x.ItemType).Distinct();

                UpdateMergeField(mergeFields, bizTypesEdoc1, true);
                UpdateMergeField(mergeFields, bizTypesEdoc2, false);

                var recipients = new List<string>() { task.User.UserEmail };
                var ccRecipients = new List<string>();

                if (task.CCUser != null && !string.IsNullOrEmpty(task.CCUser.UserEmail))
                {
                    ccRecipients.Add(task.CCUser.UserEmail);
                }

                await SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients, null, ccRecipients);
                Utilities.WriteLogError($"Email sent successfully to {task.User.UserEmail}");
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError($"Send email approver to {task.User.UserEmail} failed");
                Utilities.WriteLogError("Error Send Mail: " + ex.Message);
                Utilities.WriteLogError("Stack trace: " + ex.StackTrace);
            }
        }
        private void UpdateMergeField(Dictionary<string, string> mergeFields, IEnumerable<string> bizTypes, bool edoc1)
        {
            try
            {
                if (bizTypes != null && bizTypes.Any())
                {
                    var bizContent = "<ul>";
                    var bizContentVN = "<ul>";

                    foreach (var bizType in bizTypes)
                    {
                        string linkType, bizName, bizNameVN;
                        GetLinkType(bizType, out linkType, out bizName);
                        GetLinkTypeVN(bizType, out linkType, out bizNameVN);

                        if (!string.IsNullOrEmpty(bizName))
                        {
                            bizContent += $"<li>{bizName}</li>";
                        }

                        if (!string.IsNullOrEmpty(bizNameVN))
                        {
                            bizContentVN += $"<li>{bizNameVN}</li>";
                        }
                    }

                    bizContent += "</ul>";
                    bizContentVN += "</ul>";

                    if (!edoc1)
                    {
                        mergeFields["Edoc2BusinessName"] = bizContent;
                        mergeFields["Edoc2BusinessNameVN"] = bizContentVN;
                        mergeFields["Edoc2Link"] = $"<a href=\"{ConfigurationManager.AppSettings["siteUrl"]}/_layouts/15/AeonHR/Default.aspx#!/home/todo\">Link</a>";
                    }
                    else
                    {
                        mergeFields["Edoc1BusinessName"] = bizContent;
                        mergeFields["Edoc1BusinessNameVN"] = bizContentVN;
                        mergeFields["Edoc1Link"] = $"<a href=\"{ConfigurationManager.AppSettings["edoc1Url"]}/default.aspx#/todo\">Link</a>";
                    }
                }
                else
                {
                    if (edoc1)
                    {
                        mergeFields["Edoc1BusinessName"] = "------------------------------------------";
                        mergeFields["Edoc1BusinessNameVN"] = "------------------------------------------";
                        mergeFields["Edoc1Link"] = "";
                    }
                    else
                    {
                        mergeFields["Edoc2BusinessName"] = "------------------------------------------";
                        mergeFields["Edoc2BusinessNameVN"] = "------------------------------------------";
                        mergeFields["Edoc2Link"] = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("UpdateMergeField: " + ex);
            }
        }
        public static void GetLinkType(string type, out string linkType, out string bizName)
        {
            linkType = string.Empty;
            bizName = string.Empty;
            switch (type)
            {
                case "ShiftExchangeApplication":
                    linkType = "shift-exchange";
                    bizName = "Shift Exchange";
                    break;
                case "Acting":
                    linkType = "action";
                    bizName = "Acting";
                    break;
                case "ResignationApplication":
                    linkType = "resignationApplication";
                    bizName = "Resignation";
                    break;
                case "RequestToHire":
                    linkType = "requestToHire";
                    bizName = "Request To Hire";
                    break;
                case "PromoteAndTransfer":
                    linkType = "promoteAndTransfer";
                    bizName = "Promotion and Transfer";
                    break;
                case "OvertimeApplication":
                    linkType = "overtimeApplication";
                    bizName = "Overtime";
                    break;
                case "MissingTimeClock":
                    linkType = "missingTimelock";
                    bizName = "Missing Time Lock";
                    break;
                case "LeaveApplication":
                    linkType = "leaves-management";
                    bizName = "Leave";
                    break;
                case "BusinessTripApplication":
                    linkType = "businessTripApplication";
                    bizName = "Business Trip Application";
                    break;
                case "TargetPlan":
                    linkType = "targetPlan";
                    bizName = "Target Plan";
                    break;
                // edoc 1
                case "Contract":
                    linkType = "Contract";
                    bizName = "Contract";
                    break;
                case "NonExpenseContract":
                    linkType = "Contract";
                    bizName = "Non-Expense Contract";
                    break;
                case "Payment":
                    linkType = "Payment";
                    bizName = "Payment";
                    break;
                case "Advance":
                    linkType = "Advance";
                    bizName = "Advance";
                    break;
                case "Reimbursement":
                    linkType = "Reimbursement";
                    bizName = "Business Trip Reimbursement";
                    break;
                case "ReimbursementPayment":
                    linkType = "ReimbursementPayment";
                    bizName = "Reimbursement Payment ";
                    break;
                case "Purchase":
                    linkType = "Purchase";
                    bizName = "Purchase ";
                    break;
                case "CreditNote":
                    linkType = "CreditNote";
                    bizName = "Credit Note";
                    break;
            }
        }
        public static void GetLinkTypeVN(string type, out string linkType, out string bizNameVN)
        {
            linkType = string.Empty;
            bizNameVN = string.Empty;
            switch (type)
            {
                case "ShiftExchangeApplication":
                    linkType = "shift-exchange";
                    bizNameVN = "Đơn đăng ký Chuyển ca";
                    break;
                case "Acting":
                    linkType = "action";
                    bizNameVN = "Đề xuất Tạm quyền";
                    break;
                case "ResignationApplication":
                    linkType = "resignationApplication";
                    bizNameVN = "Đơn đăng ký Nghỉ việc";
                    break;
                case "RequestToHire":
                    linkType = "requestToHire";
                    bizNameVN = "Đề xuất Tuyển dụng";
                    break;
                case "PromoteAndTransfer":
                    linkType = "promoteAndTransfer";
                    bizNameVN = "Đề xuất Thăng chức và thuyên chuyển";
                    break;
                case "OvertimeApplication":
                    linkType = "overtimeApplication";
                    bizNameVN = "Đơn đăng ký tăng ca";
                    break;
                case "MissingTimeClock":
                    linkType = "missingTimelock";
                    bizNameVN = "Phiếu bổ sung dữ liệu quyét thẻ";
                    break;
                case "LeaveApplication":
                    linkType = "leaves-management";
                    bizNameVN = "Đơn đăng ký Nghỉ phép";
                    break;
                case "BusinessTripApplication":
                    linkType = "businessTripApplication";
                    bizNameVN = "Đơn đăng kí công tác";
                    break;
                // edoc 1
                case "Contract":
                    linkType = "Contract";
                    bizNameVN = "Hợp đồng";
                    break;
                case "NonExpenseContract":
                    linkType = "NonExpenseContract";
                    bizNameVN = "Hợp đồng không tính phí";
                    break;
                case "Payment":
                    linkType = "Payment";
                    bizNameVN = "Thanh toán";
                    break;
                case "Advance":
                    linkType = "Advance";
                    bizNameVN = "Tạm ứng";
                    break;
                case "Reimbursement":
                    linkType = "Reimbursement";
                    bizNameVN = "Hoàn phí công tác";
                    break;
                case "ReimbursementPayment":
                    linkType = "ReimbursementPayment";
                    bizNameVN = "Thanh toán hoàn phí";
                    break;
                case "Purchase":
                    linkType = "Purchase";
                    bizNameVN = "Đơn hàng ";
                    break;
                case "CreditNote":
                    linkType = "CreditNote";
                    bizNameVN = "Tín dụng";
                    break;
            }
        }
        #endregion

        #region SendMail
        async Task<bool> SendEmail(EmailTemplateName template, EmailTemplateName layoutName, Dictionary<string, string> mergedFields, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
        {
            if (recipients != null && recipients.Any())
            {
                Utilities.WriteLogError("Sending email to {0}\", string.Join(\";\", recipients)");
            }
            var result = false;
            try
            {
                var mailTemplate = await GetEmailTemplate(template.ToString());
                if (mailTemplate is null)
                {
                    Utilities.WriteLogError("GetEmailTemplate() - Get email template from API is null");
                    return false;
                }
                var layout = new EmailTemplateModel() { Body = mailTemplate.Body, Subject = mailTemplate.Subject, TemplatCode = mailTemplate.Code, Name = mailTemplate.Subject };
                var emailTemplate = new EmailTemplateModel() { Body = mailTemplate.Body, Subject = mailTemplate.Subject, TemplatCode = mailTemplate.Code, Name = mailTemplate.Code };
                if (emailTemplate != null && (!(emailTemplate.Subject is null)) && (!(emailTemplate.Body is null)) && layout != null)
                {
                    AddDefaultMergedFields(mergedFields);
                    ProcessEmailContent(emailTemplate, layout, mergedFields);
                    var email = ProcessEmailNotificationArgs(emailTemplate, recipients, attachments, ccRecipients);
                    if (email != null)
                    {
                        await SendBySmtpClient(email, emailTemplate);
                    }
                    else
                    {
                        Utilities.WriteLogError("Error when send email");
                    }
                }
                else
                {
                    Utilities.WriteLogError(string.Format("Email Template '{0}' not found.", template.ToString()));
                }

                result = true;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error: SendEmail(): " + ex.Message);
            }

            return result;
        }
        public async Task<ResponseEmailTemplateModel> GetEmailTemplate(string name)
        {
            var responseEmailTemplate = new ResponseEmailTemplateModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var baseUrl = ConfigurationManager.AppSettings["mailAPI"];
                    Utilities.WriteLogError("GetEmailTemplate.baseUrl" + baseUrl);
                    var requestUri = new Uri(new Uri(baseUrl), "GetEmailTemplateByCode");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var model = new EmailTemplateModel { Code = name };
                    string payload = JsonConvert.SerializeObject(model);
                    payload = payload.Replace("null", "\"\"");
                    var content = Utilities.StringContentObjectFromJson(payload);
                    var response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        responseEmailTemplate = JsonConvert.DeserializeObject<ResponseEmailTemplateModel>(httpResponseResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error: GetEmailTemplate(): " + ex.Message);
            }
            return responseEmailTemplate;
        }
        private void AddDefaultMergedFields(Dictionary<string, string> mergedFields)
        {
            if (mergedFields == null)
            {
                mergedFields = new Dictionary<string, string>();
            }
            mergedFields["BaseUrl"] = "Aeon";
        }
        private void ProcessEmailContent(EmailTemplateModel template, EmailTemplateModel layout, Dictionary<string, string> mergedFields)
        {
            try
            {
                if (mergedFields != null)
                {
                    // Add main content to layout
                    if (layout != null && !string.IsNullOrEmpty(layout.Body) && layout.Body.Contains("[MainContent]"))
                    {
                        template.Body = layout.Body.Replace("[MainContent]", template.Body);
                    }
                    //Process content
                    foreach (var key in mergedFields.Keys)
                    {
                        if (!(mergedFields[key] is null))
                        {
                            template.Subject = template.Subject.Replace(string.Format("[{0}]", key), mergedFields[key]);
                            template.Body = template.Body.Replace(string.Format("[{0}]", key), mergedFields[key]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error in ProcessEmailContent(): " + ex.Message);
            }
        }
        private EmailNotificationArgs ProcessEmailNotificationArgs(EmailTemplateModel emailTemplate, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
        {
            try
            {
                EmailNotificationArgs email = new EmailNotificationArgs();
                email.Subject = emailTemplate.Subject;
                email.Body = emailTemplate.Body;
                email.Recipients = recipients;
                email.Attachments = attachments;
                email.Smtp = _section.Network.Host;
                email.Port = _section.Network.Port;
                email.EnableSSL = _section.Network.EnableSsl;
                email.Sender = _section.From;
                email.Username = _section.Network.UserName;
                email.Password = _section.Network.Password;
                if (ccRecipients != null)
                {
                    email.CcRecipients = ccRecipients;
                }
                return email;
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error in ProcessEmailNotificationArgs(): " + ex.Message);
            }
            return null;
        }
        private async Task SendBySmtpClient(EmailNotificationArgs args, EmailTemplateModel type)
        {
            try
            {
                var smtpClient = new SmtpClient(args.Smtp, args.Port)
                {
                    EnableSsl = args.EnableSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = !args.RequiredAuthentication,
                    Credentials = new NetworkCredential(args.Username, args.Password)
                };

                var mail = new MailMessage();

                // Receipients
                if (args.Recipients != null)
                {
                    foreach (var recipient in args.Recipients)
                    {
                        if (!string.IsNullOrEmpty(recipient))
                        {
                            mail.To.Add(recipient);
                        }
                    }
                }
                if (args.CcRecipients != null)
                {
                    foreach (var cc in args.CcRecipients)
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            mail.CC.Add(cc);
                        }
                    }
                }

                if (mail.To.Count > 0)
                {
                    string fromEmailAddress = string.IsNullOrEmpty(args.Sender) ? "e-document.notification@aeon.com.vn" : args.Sender;
                    mail.From = new MailAddress(fromEmailAddress, "AEON Notification");
                    mail.Subject = args.Subject;
                    mail.Body = args.Body;
                    mail.IsBodyHtml = true;
                    List<L_AttachmentFile> attachmentFile = new List<L_AttachmentFile>();

                    try
                    {
                        if (args.Attachments != null)
                        {
                            foreach (var fileName in args.Attachments.Keys)
                            {
                                var stream = new MemoryStream(args.Attachments[fileName]);
                                attachmentFile.Add(new L_AttachmentFile() { FileName = fileName, Base64 = Convert.ToBase64String(stream.ToArray()) });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utilities.WriteLogError("Error.Attachment: " + e.Message);
                    }

                    var mailwait = new MailWaitListModel();
                    mailwait.TemplateCode = type.TemplatCode;
                    mailwait.Subject = type.Subject;
                    mailwait.Body = mail.Body;
                    mailwait.MailTo = new List<string>() { mail.To.ToString() };
                    mailwait.MailCC = new List<string>() { };
                    mailwait.MailBCC = new List<string>() { };
                    mailwait.Module = "Edoc2";
                    mailwait.Attachments = attachmentFile;
                    mailwait.SendCount = 0;
                    mailwait.SendDate = DateTimeOffset.Now.AddSeconds(30);
                    await SendMailToWaitList(mailwait);
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("Error.SendBySmtpClient: " + ex.Message);
                Utilities.WriteLogError("Error.SendBySmtpClient: " + ex.StackTrace);
            }
        }
        public async Task<ResponseCreateEmailWaitlistModel> SendMailToWaitList(MailWaitListModel email)
        {
            var responseCreateMail = new ResponseCreateEmailWaitlistModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var baseUrl = ConfigurationManager.AppSettings["mailAPI"];
                    var requestUri = new Uri(new Uri(baseUrl), "CreateEmailWaitlist");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonContent = JsonConvert.SerializeObject(email);
                    Utilities.WriteLogError("SendMailToWaitList.jsonContent: " + jsonContent);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        responseCreateMail = JsonConvert.DeserializeObject<ResponseCreateEmailWaitlistModel>(httpResponseResult);
                        Utilities.WriteLogError("CreateMailTemplate.Status: " + responseCreateMail.Status);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("responseCreateMail: " + ex.Message);
                Utilities.WriteLogError("responseCreateMail: " + ex.StackTrace);
            }
            return responseCreateMail;
        }
        #endregion

        #region Config API
        protected HttpClient ConfigAPI()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(SAP_TimeOut);
            client.BaseAddress = new Uri(_edoc1Setting.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_edoc1Setting.Header.AcceptType));
            client.DefaultRequestHeaders.Add("X-Requested-With", _edoc1Setting.Header.XRequestWith);
            client.DefaultRequestHeaders.Add("Token", _edoc1Setting.Header.Token);
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_edoc1Setting.Authentication.UserName}:{_edoc1Setting.Authentication.Password}")); //("Username:Password")  
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            return client;
        }
        protected HttpClient ConfigAPIV2()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(4000);
            client.BaseAddress = new Uri(_edoc1SettingV2.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _edoc1SettingV2.Header.Token);
            return client;
        }
        public static double SAP_TimeOut
        {
            get
            {
                double returnValue = 180;
                try
                {
                    string strValue = ConfigurationManager.AppSettings["SAP_TimeOut"];
                    if (string.IsNullOrEmpty(strValue) || !double.TryParse(strValue, out returnValue))
                    {
                        returnValue = 180;
                    }
                }
                catch
                {
                    returnValue = 180;
                }
                return returnValue;
            }
        }
        #endregion
    }
}