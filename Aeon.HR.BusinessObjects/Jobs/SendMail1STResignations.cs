using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class SendMail1STResignations
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEmailNotification _emailNotification;

        public SendMail1STResignations(ILogger logger, IUnitOfWork uow, IEmailNotification emailNotification)
        {
            _logger = logger;
            _uow = uow;
            _emailNotification = emailNotification;
        }

        public async Task<bool> SendNotifications()
        {
            _logger.LogInformation("LogInformation.Start mail for 1st for Resignation: SendNotifications" + DateTimeOffset.Now);
            var edoc2Tasks = new List<EdocTask>();
            var groupDept = new List<NotificationUser>();
            var tasks = await _uow.GetRepository<WorkflowTask>(true).FindByAsync(x => !x.IsCompleted && !x.IsTurnedOffSendNotification);
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
            var assistanceNodes = _uow.GetRepository<Department>().FindBy<DepartmentViewModel>(x => x.UserDepartmentMappings.Any(y => y.IsHeadCount && y.Role == Group.Assistance));
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

            if (edoc2Tasks.Any())
            {
                try
                {
                    var edocTaskResignations = edoc2Tasks.Where(x => x.Edoc2Tasks.Any() && x.Edoc2Tasks.Where(y => y.ItemType == "ResignationApplication").Any()).ToList();
                    if (edocTaskResignations.Any())
                    {
                        List<string> refer = new List<string>();
                        foreach (var edocTask in edocTaskResignations)
                        {
                            foreach (var ed2 in edocTask.Edoc2Tasks)
                            {
                                _logger.LogInformation("SendNotifications.Information.REF: " + ed2.ReferenceNumber);
                                refer.Add(ed2.ReferenceNumber);
                            }
                        }
                        if (refer.Any())
                        {
                            await SendNotification1STResignation(refer);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("SendNotifications.Error: " + e.Message);
                }
            }
            return true;
        }

        public async Task<bool> SendNotification1STResignation(List<string> ReferecenNumbers)
        {
            // Kiem tra list send mail da gui hay chua
            HashSet<EmailHistory> emailHistoriesNotSend = new HashSet<EmailHistory>();
            HashSet<User> userSending = new HashSet<User>();

            var allEmailHistories = await _uow.GetRepository<EmailHistory>().FindByAsync(x => !x.IsDeleted && ReferecenNumbers.Contains(x.ReferenceNumber));
            if (allEmailHistories.Any())
            {
                foreach (var emailHistories in allEmailHistories)
                {
                    var userSend = (await _uow.GetRepository<User>().FindByAsync(x => x.UserDepartmentMappings.Any(y => y.DepartmentId == emailHistories.DepartmentId && y.Role == emailHistories.DepartmentType && y.IsHeadCount) && x.IsActivated && !x.IsDeleted));
                    if (userSend.Any())
                    {
                        foreach (var user in userSend) userSending.Add(user); // user se send email
                        if (!string.IsNullOrEmpty(emailHistories.UserSent))
                        {
                            var userSended = Mapper.Map<List<EmailHistory.UserInfo>>(JsonConvert.DeserializeObject<List<EmailHistory.UserInfo>>(emailHistories.UserSent));
                            if (userSended.Any())
                            {
                                foreach (var user in userSend)
                                {
                                    bool isNeedSend = userSended.Where(x => x.UserId != user.Id).Any();
                                    if (isNeedSend)
                                    {
                                        emailHistoriesNotSend.Add(emailHistories);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            emailHistoriesNotSend.Add(emailHistories);
                        }
                    }
                }
            }

            if (emailHistoriesNotSend != null)
            {
                try
                {
                    var groupByDepartments = emailHistoriesNotSend.GroupBy(x => new { x.DepartmentId, x.DepartmentType }).ToList();
                    foreach (var grDept in groupByDepartments)
                    {
                        List<string> referenceNumbers = grDept.Select(x => x.ReferenceNumber).ToList();
                        var histories = emailHistoriesNotSend.Where(x => referenceNumbers.Contains(x.ReferenceNumber)).ToList();
                        var listUserSendEmail = userSending.Where(x => x.UserDepartmentMappings.Any(y => y.DepartmentId == (new Guid(grDept.Key.DepartmentId.ToString())) && y.Role == ((Group)grDept.Key.DepartmentType))).ToList();
                        await SendEmailNotificationFor1stResignations(EmailTemplateName.For1STResignation, listUserSendEmail, histories);
                    }
                    await _uow.CommitAsync();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("SendNotification1STResignation: " + e.Message);
                }
            }
            return true;
        }

        public async Task UpdateInfoEmailHistories(User user, List<string> referenceNumbers)
        {
            if (referenceNumbers.Any() && user != null)
            {
                foreach (var refe in referenceNumbers)
                {
                    var emailHistories = await _uow.GetRepository<EmailHistory>().GetSingleAsync(x => x.ReferenceNumber == refe);
                    if (emailHistories != null)
                    {
                        List<EmailHistory.UserInfo> infor = new List<EmailHistory.UserInfo>();
                        if (string.IsNullOrEmpty(emailHistories.UserSent))
                        {
                            EmailHistory.UserInfo uf = new EmailHistory.UserInfo()
                            {
                                UserId = user.Id,
                                FullName = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName
                            };
                            infor.Add(uf);
                        }
                        else
                        {
                            infor = Mapper.Map<List<EmailHistory.UserInfo>>(JsonConvert.DeserializeObject<List<EmailHistory.UserInfo>>(emailHistories.UserSent));
                            if (infor.Any())
                            {
                                EmailHistory.UserInfo uf = new EmailHistory.UserInfo()
                                {
                                    UserId = user.Id,
                                    FullName = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName
                                };
                                infor.Add(uf);
                            }
                        }
                        emailHistories.UserSent = JsonConvert.SerializeObject(infor);
                        _uow.GetRepository<EmailHistory>().Update(emailHistories);
                    }
                }
            }
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
                Edoc2Tasks = new List<WorkflowTaskViewModel>();
            }
            public NotificationUser User { get; set; }
            public NotificationUser CCUser { get; set; }
            public List<WorkflowTaskViewModel> Edoc1Tasks { get; set; }
            public List<WorkflowTaskViewModel> Edoc2Tasks { get; set; }
        }

        private async Task SendEmailNotificationFor1stResignations(EmailTemplateName type, List<User> users, List<EmailHistory> emailHistories)
        {
            try
            {
                foreach (var user in users)
                {
                    List<string> referencenNumberValid = new List<string>();
                    foreach (var emailHis in emailHistories)
                    {
                        // truong hop co gui cho user roi
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
                        var mergeFields = new Dictionary<string, string>();
                        mergeFields["ApproverName"] = user.FullName;
                        UpdateMergeFieldForResignations(mergeFields, referencenNumberValid);
                        var recipients = new List<string>() { user.Email };
                        var ccRecipients = new List<string>();
                        _logger.LogInformation("SendEmailNotificationFor1stResignations: " + user.Email + " - " + DateTimeOffset.Now);
                        bool isSendSuccess = await _emailNotification.SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients);
                        if (isSendSuccess)
                        {
                            await UpdateInfoEmailHistories(user, referencenNumberValid);
                        }
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
    }
}
