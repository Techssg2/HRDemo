using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.CustomSection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.Test
{
    public class TestSendMail
    {
        private readonly TradeContractSettingsSection _tradeContractSetting;
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private readonly IEmailNotification _emailNotification;

        public TestSendMail(IUnitOfWork uow, ILogger logger, IEmailNotification emailNotification)
        {
            _uow = uow;
            _tradeContractSetting = (TradeContractSettingsSection) ConfigurationManager.GetSection("aeonTradeContractSettings");
            _logger = logger;
            _emailNotification = emailNotification;
        }

        public async Task SendNotifications()
        {
            var edoc2Tasks = new List<EdocTask>();
            var groupDept = new List<NotificationUser>();
            var tasks = (await _uow.GetRepository<WorkflowTask>(true).FindByAsync(x => !x.IsCompleted && !x.IsTurnedOffSendNotification)).ToList();
            var deptTasks = tasks.Where(x => x.AssignedToDepartmentId.HasValue);
            foreach (var deptTask in deptTasks)
            {
                if (!groupDept.Any(x => x.DepartmentId == deptTask.AssignedToDepartmentId && x.DepartmentGroup == deptTask.AssignedToDepartmentGroup))
                {
                    var users = _uow.GetRepository<User>().FindBy(x => !string.IsNullOrEmpty(x.Email) && x.UserDepartmentMappings.Any(t => t.Role == deptTask.AssignedToDepartmentGroup && t.DepartmentId == deptTask.AssignedToDepartmentId));
                    foreach (var user in users)
                    {
                        groupDept.Add(new NotificationUser { UserFullName = user.FullName, UserEmail = user.Email, UserId = user.Id, DepartmentId = deptTask.AssignedToDepartmentId.Value, DepartmentGroup = deptTask.AssignedToDepartmentGroup });
                    }
                }
            }

            var assistanceNodes = _uow.GetRepository<Department>().FindBy<DepartmentViewModel>(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.Role == Infrastructure.Enums.Group.Assistance));
            var userMappingDepartments = _uow.GetRepository<UserDepartmentMapping>().FindBy<UserDepartmentMappingViewModel>(x => assistanceNodes.Any(y => y.Id == x.DepartmentId));
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
                    var users = tasks.Where(x => x.AssignedToId == userId || (x.AssignedToDepartmentId == notiUser.DepartmentId && x.AssignedToDepartmentGroup == notiUser.DepartmentGroup)).Select(x => Mapper.Map<WorkflowTaskViewModel>(x)).ToList();
                    //SendEmailNotificationForApprover(EmailTemplateName.ForApprover, userTasks, notiUser);
                    edoc2Tasks.Add(new EdocTask
                    {
                        User = notiUser,
                        Edoc2Tasks = users
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

            List<string> list = new List<string>() {
            "RES-000003191-2022",
            "RES-000003190-2022",
            "RES-000003189-2022",
            "RES-000003188-2022",
            "RES-000003187-2022",
            "RES-000003186-2022",
            };
            // 
            Guid gu = new Guid("ABD88D1B-419F-4BDA-8E89-2DE8F89285B2");
            var userTasks = tasks.Where(x => list.Contains(x.ReferenceNumber)).Select(x => Mapper.Map<WorkflowTaskViewModel>(x)).ToList();

            foreach(var it in userTasks)
            {
                edoc2Tasks.Add(new EdocTask
                {
                    User = (new NotificationUser { UserFullName = "TamPV", UserEmail = "Phavatam@gmail.com", UserId = new Guid("2F28130A-1C16-4299-A88F-CC82984D7DCD"), DepartmentId = new Guid("18FC0C92-36CD-408F-B0E2-D89E3B38B9AA"), DepartmentGroup = Group.Checker }),
                    Edoc2Tasks = new List<WorkflowTaskViewModel> { it }
                });
            }
            
            /*var edocTasks = GetTasks(edoc2Tasks);*/
            if (edoc2Tasks.Any())
            {
                var ccRecipients = new List<string>();
                /*foreach (var task in edoc2Tasks)
                {
                    task.CCUser = GetAssistanceUser(task.User, assistanceNodes, userMappingDepartments);
                    var mergeFields = new Dictionary<string, string>();
                    mergeFields["ApproverName"] = "Test";
                    var bizTypesEdoc1 = task.Edoc1Tasks.Select(x => x.ItemType).Distinct();
                    var bizTypesEdoc2 = task.Edoc2Tasks.Select(x => x.ItemType).Distinct();
                    UpdateMergeField(mergeFields, bizTypesEdoc1, true);
                    UpdateMergeField(mergeFields, bizTypesEdoc2, false);
                    var recipients = new List<string>() { "phavatam@gmail.com" };
                    *//*SendEmailNotificationForApprover(EmailTemplateName.ForApprover, task);*//*
                    await _emailNotification.SendEmail(EmailTemplateName.For1STResignation, EmailTemplateName.MainLayout, mergeFields, recipients);
                }*/
                // send mail for 1st for resignation
                var edocTaskResignations = edoc2Tasks.Where(x => x.Edoc2Tasks.Any() && x.Edoc2Tasks.Where(y => y.ItemType == "ResignationApplication").Any()).FirstOrDefault();
                if (edocTaskResignations != null)
                {
                    var allUsers = _uow.GetRepository<User>().GetAll();
                    var udm = _uow.GetRepository<UserDepartmentMapping>().GetAll();
                    /*var ids = edocTaskResignations.Edoc2Tasks.Where(z => z.ReferenceNumber == x.ReferenceNumber && z.ItemId == x.ItemId && z.ItemType == x.ItemType);*/
                    var emailHistories = _uow.GetRepository<EmailHistory>().FindBy(x => x.ItemId == (new Guid("2EB4267F-C85F-43B4-A3B4-182A7B97C3D6"))).ToList();
                    var deptGroup = emailHistories.GroupBy(x => new { x.DepartmentId, x.DepartmentType }).ToList();
                    foreach (var task in deptGroup)
                    {
                        var departmentType = (Group)task.Key.GetPropValue("DepartmentType");
                        Guid departmentId = (new Guid(task.Key.GetPropValue("DepartmentId").ToString()));
                        var udmm = udm.Where(x => x.DepartmentId == departmentId && x.Role == departmentType).ToList();
                        var listUserSendMail = allUsers.Where(x => udmm.Select(y => y.UserId).ToList().Contains(x.Id)).ToList();
                        List<string> referenceNumber = task.Select(x => x.ReferenceNumber).ToList();
                        await SendEmailNotificationFor1stResignations(EmailTemplateName.For1STResignation, referenceNumber, listUserSendMail, emailHistories);
                        UpdateInfoEmailHistories(listUserSendMail, emailHistories);
                    }
                    _uow.Commit();
                }
            }
        }

        public void UpdateInfoEmailHistories(List<User> users, List<EmailHistory> emailHistories)
        {
            if (emailHistories.Any())
            {
                foreach (var itemRef in emailHistories)
                {
                    List<EmailHistory.UserInfo> infor = new List<EmailHistory.UserInfo>();
                    if (!string.IsNullOrEmpty(itemRef.UserSent))
                    {
                        infor = Mapper.Map<List<EmailHistory.UserInfo>>(JsonConvert.DeserializeObject<List<EmailHistory.UserInfo>>(itemRef.UserSent));
                        if (infor.Any())
                        {
                            List<User> us = users.Where(x => !infor.Select(y => y.UserId).ToList().Contains(x.Id)).ToList();
                            foreach (var u in us)
                            {
                                EmailHistory.UserInfo uf = new EmailHistory.UserInfo()
                                {
                                    UserId = u.Id,
                                    FullName = u.FullName
                                };
                                infor.Add(uf);
                            }
                        }
                        itemRef.UserSent = JsonConvert.SerializeObject(infor);
                    }
                    else
                    {
                        if (users.Any())
                        {
                            foreach(var us in users)
                            {
                                EmailHistory.UserInfo uf = new EmailHistory.UserInfo()
                                {
                                    UserId = us.Id,
                                    FullName = us.FullName
                                };
                                infor.Add(uf);
                            }
                            itemRef.UserSent = JsonConvert.SerializeObject(infor);
                        }
                    }
                }
            }
        }

        private async Task SendEmailNotificationFor1stResignations(EmailTemplateName type, List<string> referenNumber, List<User> users, List<EmailHistory> emailHistories)
        {
            try
            {
                foreach (var user in users)
                {
                    List<string> referencenNumberValid = new List<string>();
                    foreach (var emailHis in emailHistories)
                    {
                        if (!string.IsNullOrEmpty(emailHis.UserSent))
                        {
                            List<EmailHistory.UserInfo> info = Mapper.Map<List<EmailHistory.UserInfo>>(JsonConvert.DeserializeObject<List<EmailHistory.UserInfo>>(emailHis.UserSent));
                            if (info.Any())
                            {
                                bool isExist = info.Where(x => x.UserId == user.Id).Any();
                                if (isExist) continue;
                            }
                        }
                        referencenNumberValid.Add(emailHis.ReferenceNumber);
                    }
                    if (referencenNumberValid.Any())
                    {
                        _logger.LogInformation("SendEmailNotificationFor1stResignations: User: " + user.Email);
                        _logger.LogInformation("SendEmailNotificationFor1stResignations: ReferencenNumbers: " + referenNumber);
                        var mergeFields = new Dictionary<string, string>();
                        mergeFields["ApproverName"] = "1ˢᵗ Approver";
                        UpdateMergeFieldForResignations(mergeFields, referencenNumberValid);
                        var recipients = new List<string>() { user.Email };
                        var ccRecipients = new List<string>();
                        await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients, null, ccRecipients);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        private void UpdateMergeFieldForResignations(Dictionary<string, string> mergeFields, IEnumerable<string> bizTypes)
        {
            if (bizTypes.Any())
            {
                var bizContent = "<ul>";
                foreach (var bizType in bizTypes)
                {
                    bizContent += $"<li>{bizType}</li>";
                }
                bizContent += "</ul>";
                mergeFields["Edoc2ReferenceNumber"] = bizContent;
                mergeFields["Edoc2AllResignationsLink"] = $"<a href=\"{ Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/resignationApplication/allRequests\">Link</a>";
            }
            else
            {
                mergeFields["Edoc2ReferenceNumber"] = "------------------------------------------";
                mergeFields["Edoc2AllResignationsLink"] = "";
            }
        }
        private NotificationUser GetAssistanceUser(NotificationUser user, IEnumerable<DepartmentViewModel> allAssistanceNodes, IEnumerable<UserDepartmentMappingViewModel> userDepartmentMappings)
        {
            NotificationUser cUser = null;
            var assistanceNode = allAssistanceNodes.Where(x => x.ParentId == user.DepartmentId).Select(y => Mapper.Map<DepartmentViewModel>(y)).FirstOrDefault();
            if (assistanceNode != null)
            {
                var currentDepartmentMapping = userDepartmentMappings.FirstOrDefault(x => x.DepartmentId == assistanceNode.Id && x.IsHeadCount);
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

        private class NotificationUser
        {
            public Guid UserId { get; set; }
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public Guid DepartmentId { get; set; }
            public Infrastructure.Enums.Group DepartmentGroup { get; set; }
        }

        private class EdocTask
        {
            public EdocTask()
            {

                Edoc1Tasks = new List<WorkflowTaskViewModel>();
                Edoc2Tasks = new List<WorkflowTaskViewModel>();
                TraddeContractTasks = new List<WorkflowTaskViewModel>();
            }
            public NotificationUser User { get; set; }
            public NotificationUser CCUser { get; set; }
            public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; }
            public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
            public List<WorkflowTaskViewModel> TraddeContractTasks { get; set; }
        }

        private void SendEmailNotificationForApprover(EmailTemplateName type, List<WorkflowTask> userTasks, NotificationUser user)
        {
            try
            {
                _logger.LogInformation($"Send email approver to {user.UserEmail}");
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
                _logger.LogError(ex.Message, ex);
            }
        }
        private void SendEmailNotificationForApprover(EmailTemplateName type, EdocTask task)
        {
            try
            {
                _logger.LogInformation($"Send email approver to {task.User.UserEmail}");
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
                _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients, null, ccRecipients).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
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
    }
}
