using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class ApproverNotificationJob
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmailNotification _emailNotification;
        private readonly IEdoc01BO _edoc01;
        private readonly IWorkflowBO _workflowBO;

        public ApproverNotificationJob(ILogger logger, IUnitOfWork uow, IEmailNotification emailNotification, IEdoc01BO edoc01, IWorkflowBO workflowBO)
        {
            _logger = logger;
            _uow = uow;
            _emailNotification = emailNotification;
            _edoc01 = edoc01;
            _workflowBO = workflowBO;
        }

        public async Task SendNotifications()
        {
            _logger.LogInformation("SendNotifications Approver Notification Job:");
            var edoc2Tasks = new List<EdocTask>();
            var groupDept = new List<NotificationUser>();
            var tasks = _uow.GetRepository<WorkflowTask>(true).FindBy(x => !x.IsCompleted && !x.IsTurnedOffSendNotification).ToList();
            var deptTasks = tasks.Where(x => x.AssignedToDepartmentId.HasValue);
            foreach (var deptTask in deptTasks)
            {
                if (!groupDept.Any(x => x.DepartmentId == deptTask.AssignedToDepartmentId && x.DepartmentGroup == deptTask.AssignedToDepartmentGroup))
                {
                    var users = _uow.GetRepository<User>().FindBy(x => !string.IsNullOrEmpty(x.Email) && x.UserDepartmentMappings.Any(t => t.Role == deptTask.AssignedToDepartmentGroup && t.DepartmentId == deptTask.AssignedToDepartmentId && !t.IsDeleted));
                    foreach (var user in users)
                    {
                        groupDept.Add(new NotificationUser { UserFullName = user.FullName, UserEmail = user.Email, UserId = user.Id, DepartmentId = deptTask.AssignedToDepartmentId.Value, DepartmentGroup = deptTask.AssignedToDepartmentGroup });
                    }
                }
            }
            var assistanceNodes = _uow.GetRepository<Department>().FindBy<DepartmentViewModel>(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.Role == Group.Assistance && !y.IsDeleted)).ToList();
            var userMappingDepartments = _uow.GetRepository<UserDepartmentMapping>().FindBy<UserDepartmentMappingViewModel>(x => assistanceNodes.Any(y => y.Id == x.DepartmentId && !x.IsDeleted));
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
                    var userTasks = tasks.Where(x => x.AssignedToId == userId || (x.AssignedToDepartmentId == notiUser.DepartmentId && x.AssignedToDepartmentGroup == notiUser.DepartmentGroup)).Select(x => Mapper.Map<WorkflowTaskViewModel>(x)).ToList();
                    //SendEmailNotificationForApprover(EmailTemplateName.ForApprover, userTasks, notiUser);
                    edoc2Tasks.Add(new EdocTask
                    {
                        User = notiUser,
                        Edoc2Tasks = userTasks
                    });
                }
            }

            //qle24: add more tasks from new modules
            var moduleTasks = DashboardHelper.GetJobTasks();
            foreach (var moduleTask in moduleTasks)
            {
                edoc2Tasks.Add(new EdocTask
                {
                    User = new NotificationUser
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
                List<string> ignoreReferenceNumberActing = _workflowBO.IgnoreReferenceNumberActing();
                if (ignoreReferenceNumberActing.Any())
                {
                    edoc2Tasks = edoc2Tasks.Where(x => x.Edoc2Tasks.Any() && x.Edoc2Tasks.Where(y => !ignoreReferenceNumberActing.Contains(y.ReferenceNumber)).Any()).ToList();
                }
                #endregion
            }
            catch (Exception e)
            {
                _logger.LogInformation("SendNotifications.ignoreReferenceNumberActing.Exception :" + e.Message);
            }
            var edocTasks = await GetTasks(edoc2Tasks);
            if (edocTasks.Any())
            {
                foreach (var task in edocTasks)
                {
                    task.CCUser = GetAssistanceUser(task.User, assistanceNodes, userMappingDepartments);
                    await SendEmailNotificationForApprover(EmailTemplateName.ForApprover, task);
                }
            }
        }
        private NotificationUser GetAssistanceUser(NotificationUser user, IEnumerable<DepartmentViewModel> allAssistanceNodes, IEnumerable<UserDepartmentMappingViewModel> userDepartmentMappings)
        {
            NotificationUser cUser = null;
            var assistanceNode = allAssistanceNodes.Where(x => x.ParentId == user.DepartmentId).Select(y => Mapper.Map<DepartmentViewModel>(y)).FirstOrDefault();
            if (assistanceNode != null)
            {
                var currentDepartmentMapping = userDepartmentMappings.FirstOrDefault(x => x.DepartmentId == assistanceNode.Id && x.IsHeadCount && !x.IsDeleted);
                cUser = new NotificationUser
                {
                    UserId = currentDepartmentMapping.UserId,
                    UserEmail = currentDepartmentMapping.UserEmail,
                    DepartmentId = currentDepartmentMapping.DepartmentId.Value,
                    UserFullName = currentDepartmentMapping.UserFullName
                };
            }
            return cUser;
        }
        private async Task<List<EdocTask>> GetTasks(List<EdocTask> edoc2Tasks)
        {
            var result = new List<EdocTask>();
            try
            {
                var allUsers = await _uow.GetRepository<User>().FindByAsync<UserListViewModel>(x => x.Type == LoginType.ActiveDirectory && !x.IsDeleted && x.IsActivated);
                //var allUsers = _uow.GetRepository<User>().FindBy<UserListViewModel>(x => x.Type == LoginType.ActiveDirectory);
                var userNotInAll = edoc2Tasks.Where(x => !allUsers.Any(y => x.User.UserId == y.Id));
                if (userNotInAll.Any())
                {
                    result.AddRange(userNotInAll);
                }
                foreach (var user in allUsers)
                {
                    try
                    {
                        var emailUser = checkUserEmailEdoc1(user as UserListViewModel); // luôn có giá trị trừ khi user.Email = Null && EmailEdoc1 = Null
                        _logger.LogInformation("SendNotifications.GetTasks.UserEdoc1 :" + user.SAPCode);
                        var arg = new Edoc1Arg
                        {
                            LoginName = user.LoginName,
                            OrderBy = "Created",
                            Skip = 0,
                            Top = 1000
                        };
                        var argV2 = new Edoc1ArgV2
                        {
                            LoginName = user.LoginName,
                            Page = 1,
                            Limit = 1000
                        };
                        var tasks = await _edoc01.GetTasks(arg);
                        _logger.LogInformation("SendNotifications.GetTasks.UserEdoc1.GetAPIEdoc1Success :" + user.SAPCode);

                        var allTasksV2 = await _edoc01.GetTasksV2(argV2);
                        var tasksV2Approval = allTasksV2?.Where(task => task.Status != "Requested To Change").ToList();

                        var ignoreModule = new List<string>() { "TradeContract", "LiquorLicense"};

                        // ignore another module 
                        tasksV2Approval = tasksV2Approval?.Where(x => !string.IsNullOrEmpty(x.Module) && x.Module.Equals("Edoc1")).ToList();

                        _logger.LogInformation("SendNotifications.GetTasksV2.UserEdoc1.GetAPIEdoc1Success :" + user.SAPCode);
                        //var tasks = new List<WorkflowTaskViewModel>();
                        var edoc2Task = edoc2Tasks.FirstOrDefault(x => x.User.UserId == user.Id);
                        if (tasks.Any() || tasksV2Approval.Any() || edoc2Task != null)
                        {
                            var edocTask = new EdocTask
                            {
                                User = edoc2Task?.User ?? new NotificationUser { UserFullName = user.FullName, UserEmail = emailUser, UserId = user.Id },
                                Edoc1Tasks = tasks.Concat(tasksV2Approval).ToList(),
                                Edoc2Tasks = edoc2Task?.Edoc2Tasks ?? new List<WorkflowTaskViewModel>()
                            };
                            if (!string.IsNullOrEmpty(emailUser))
                            {
                                edocTask.User.UserEmail = emailUser;
                            }
                            result.Add(edocTask);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("SendNotifications.GetTasks.Exception :" + e.Message);
                    }
                }
            } catch (Exception e)
            {
                _logger.LogInformation("GetTasks.Exception :" + e.Message);
            }
            return result;
        }

        private class NotificationUser
        {
            public Guid UserId { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public Guid DepartmentId { get; set; }
            public Group DepartmentGroup { get; set; }
        }

        private class EdocTask
        {
            public EdocTask()
            {
                Edoc1Tasks = new List<WorkflowTaskViewModel>();
                Edoc2Tasks = new List<WorkflowTaskViewModel>();
            }
            public NotificationUser User { get; set; }
            public NotificationUser CCUser { get; set; }
            public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; } // Edoc2
            public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
        }

        private void SendEmailNotificationForApprover(EmailTemplateName type, List<WorkflowTask> userTasks, NotificationUser user)
        {
            try
            {
                //_logger.LogInformation($"Send email approver to {user.UserEmail}");
                var mergeFields = new Dictionary<string, string>();
                mergeFields["ApproverName"] = user.UserFullName;
                var bizTypes = userTasks.Select(x => x.ItemType).Distinct();
                var bizContent = "<ul>";
                var bizContentVN = "<ul>";
                foreach (var bizType in bizTypes)
                {
                    string linkType, bizName, bizNameVN;
                    Utilities.GetLinkType(bizType, out linkType, out bizName);
                    Utilities.GetLinkTypeVN(bizType, out linkType, out bizNameVN);
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
                mergeFields["BusinessName"] = bizContent;
                mergeFields["BusinessNameVN"] = bizContentVN;
                mergeFields["Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/todo\">Link</a>";
                var recipients = new List<string>() { user.UserEmail };
                _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send email approver to {user.UserEmail}");
                _logger.LogError("Error Send Mail:" + ex.Message);
            }
        }
        private async Task SendEmailNotificationForApprover(EmailTemplateName type, EdocTask task)
        {
            try
            {
                //_logger.LogInformation($"Send email approver to {task.User.UserEmail}");
                var mergeFields = new Dictionary<string, string>();
                mergeFields["ApproverName"] = task.User.UserFullName;
                var bizTypesEdoc1 = task.Edoc1Tasks.Select(x => x.ItemType).Distinct();
                var bizTypesEdoc2 = task.Edoc2Tasks.Select(x => x.ItemType).Distinct();
                UpdateMergeField(mergeFields, bizTypesEdoc1, true);
                UpdateMergeField(mergeFields, bizTypesEdoc2, false);
                var recipients = new List<string>() { task.User.UserEmail };
                var ccRecipients = new List<string>();
                if (task.CCUser != null)
                {
                    ccRecipients.Add(task.CCUser.UserEmail);
                }
                await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients, null, ccRecipients);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send email approver to {task.User.UserEmail}");
                _logger.LogError("Error Send Mail:" + ex.Message);
            }
        }
        private void UpdateMergeField(Dictionary<string, string> mergeFields, IEnumerable<string> bizTypes, bool edoc1)
        {
            if (bizTypes.Any())
            {
                var bizContent = "<ul>";
                var bizContentVN = "<ul>";
                foreach (var bizType in bizTypes)
                {
                    string linkType, bizName, bizNameVN;
                    Utilities.GetLinkType(bizType, out linkType, out bizName);
                    Utilities.GetLinkTypeVN(bizType, out linkType, out bizNameVN);
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
                    mergeFields["Edoc2Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/todo\">Link</a>";
                }
                else
                {
                    mergeFields["Edoc1BusinessName"] = bizContent;
                    mergeFields["Edoc1BusinessNameVN"] = bizContentVN;
                    mergeFields["Edoc1Link"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["edoc1Url"])}/default.aspx#/todo\">Link</a>";
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
        private string checkUserEmailEdoc1(UserListViewModel user)
        {
            try
            {
                string dbITConnectionString = ConfigurationManager.ConnectionStrings["HRDbContext"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(dbITConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM ITUsers WHERE Id = @UserId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("EmailEdoc1")))
                                {
                                    user.Email = reader.GetString(reader.GetOrdinal("EmailEdoc1"));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: checkUserEmailEdoc1 " + ex.Message);
            }
            return user.Email;
        }
    }
}
