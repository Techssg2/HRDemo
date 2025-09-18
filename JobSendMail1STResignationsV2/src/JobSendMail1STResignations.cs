using JobSendMail1STResignationsV2.src.Entity;
using JobSendMail1STResignationsV2.src.Enums;
using JobSendMail1STResignationsV2.src.Helpers;
using JobSendMail1STResignationsV2.src.Model;
using JobSendMail1STResignationsV2.src.Services;
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
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2.src
{
    public class JobSendMail1STResignations
    {
        private readonly WorkflowTaskService _workflowTaskService;
        private readonly UserService _userService;
        private readonly DepartmentService _departmentService;
        private readonly UserDepartmentMappingService _userDepartmentMappingService;
        private readonly EmailHistoryService _emailHistoryService;
        private readonly SmtpSection _section;

        // Constructor
        public JobSendMail1STResignations()
        {
            _workflowTaskService = new WorkflowTaskService();
            _departmentService = new DepartmentService();
            _userDepartmentMappingService = new UserDepartmentMappingService();
            _emailHistoryService = new EmailHistoryService();
            _userService = new UserService();
            _section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }

        public async Task Run()
        {
            await StartJobSendMail1STResignations();
        }

        #region Functions
        public async Task StartJobSendMail1STResignations()
        {
            Utilities.WriteLogError("LogInformation.Start mail for 1st for Resignation: SendNotifications" + DateTimeOffset.Now);

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
                    var userTasks = tasks.Where(x => x.AssignedToId == userId
                    || (x.AssignedToDepartmentId == notiUser.DepartmentId && x.AssignedToDepartmentGroup == (int)notiUser.DepartmentGroup)).Select(x => new WorkflowTaskModel
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

            //qle24: add more tasks from new modules
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
                                Utilities.WriteLogError("SendNotifications.Information.REF: " + ed2.ReferenceNumber);
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
                    Utilities.WriteLogError("SendNotifications.Error: " + e.Message);
                }
            }
        }
        public async Task<bool> SendNotification1STResignation(List<string> ReferecenNumbers)
        {
            // Kiem tra list send mail da gui hay chua
            HashSet<EmailHistoryEntity> emailHistoriesNotSend = new HashSet<EmailHistoryEntity>();
            HashSet<UserEntity> userSending = new HashSet<UserEntity>();

            //var allEmailHistories = await _uow.GetRepository<EmailHistory>().FindByAsync(x => !x.IsDeleted && ReferecenNumbers.Contains(x.ReferenceNumber));
            var allEmailHistories = _emailHistoryService.GetEmailHistoriesByReferenceNumbers(ReferecenNumbers);
            if (allEmailHistories.Any())
            {
                foreach (var emailHistories in allEmailHistories)
                {
                    //var userSend = (await _uow.GetRepository<User>().FindByAsync(x => x.UserDepartmentMappings.Any(y => y.DepartmentId == emailHistories.DepartmentId && y.Role == emailHistories.DepartmentType && y.IsHeadCount) && x.IsActivated && !x.IsDeleted));
                    var userSend = _userService.GetUsersByDepartmentAndRole(emailHistories.DepartmentId.Value, emailHistories.DepartmentType, true);
                    if (userSend.Any())
                    {
                        //foreach (var user in userSend) userSending.Add(user);
                        if (!string.IsNullOrEmpty(emailHistories.UserSent))
                        {
                            //var userSended = Mapper.Map<List<EmailHistoryEntity.UserInfo>>(JsonConvert.DeserializeObject<List<EmailHistoryEntity.UserInfo>>(emailHistories.UserSent));
                            var userSended = JsonConvert.DeserializeObject<List<EmailHistoryEntity.UserInfo>>(emailHistories.UserSent)
                            ?.Select(x => new EmailHistoryEntity.UserInfo
                            {
                                UserId = x.UserId,
                                FullName = x.FullName
                            }).ToList() ?? new List<EmailHistoryEntity.UserInfo>();

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
                        var listUserSendEmail = _userService.GetUsersByDepartmentAndRole(grDept.Key.DepartmentId.Value, grDept.Key.DepartmentType, true);
                        Utilities.WriteLogError("\n");
                        Utilities.WriteLogError("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                        await SendEmailNotificationFor1stResignations(EmailTemplateName.For1STResignation, listUserSendEmail, histories);
                        Utilities.WriteLogError("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                    }
                }
                catch (Exception e)
                {
                    Utilities.WriteLogError("SendNotification1STResignation: " + e.Message);
                }
            }
            return true;
        }
        private async Task SendEmailNotificationFor1stResignations(EmailTemplateName type, List<UserEntity> users, List<EmailHistoryEntity> emailHistories)
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
                            var info = JsonConvert.DeserializeObject<List<EmailHistoryEntity.UserInfo>>(emailHis.UserSent)
                            ?.Select(x => new EmailHistoryEntity.UserInfo
                            {
                                UserId = x.UserId,
                                FullName = x.FullName
                            }).ToList() ?? new List<EmailHistoryEntity.UserInfo>();

                            if (info.Any())
                            {
                                bool isExist = info.Where(x => x.UserId == user.Id).Any();
                                if (isExist)
                                {
                                    Utilities.WriteLogError($"User {user.FullName} already received notification for {emailHis.ReferenceNumber}");
                                    continue;
                                }
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
                        Utilities.WriteLogError("SendEmailNotificationFor1stResignations: " + user.Email + " - " + DateTimeOffset.Now);
                        bool isSendSuccess = await SendEmail(type, EmailTemplateName.MainLayout, mergeFields, recipients);
                        if (isSendSuccess)
                        {
                            await UpdateInfoEmailHistories(user, referencenNumberValid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("SendEmailNotificationFor1stResignations: " + ex.Message);
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
                mergeFields["Edoc2AllResignationsLink"] = $"<a href=\"{Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/resignationApplication/allRequests\">Link</a>";
            }
            else
            {
                mergeFields["Edoc2ReferenceNumber"] = "------------------------------------------";
                mergeFields["Edoc2AllResignationsLink"] = "";
            }
        }
        public async Task UpdateInfoEmailHistories(UserEntity user, List<string> referenceNumbers)
        {
            if (referenceNumbers.Any() && user != null)
            {
                var allEmailHistories = _emailHistoryService.GetEmailHistoriesByReferenceNumbers(referenceNumbers);

                foreach (var refe in referenceNumbers)
                {
                    var emailHistories = allEmailHistories.FirstOrDefault(x => x.ReferenceNumber == refe);
                    if (emailHistories != null)
                    {
                        List<EmailHistoryEntity.UserInfo> infor = new List<EmailHistoryEntity.UserInfo>();

                        if (string.IsNullOrEmpty(emailHistories.UserSent))
                        {
                            EmailHistoryEntity.UserInfo uf = new EmailHistoryEntity.UserInfo()
                            {
                                UserId = user.Id,
                                FullName = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName
                            };
                            infor.Add(uf);
                        }
                        else
                        {
                            infor = JsonConvert.DeserializeObject<List<EmailHistoryEntity.UserInfo>>(emailHistories.UserSent)
                                ?.Select(x => new EmailHistoryEntity.UserInfo
                                {
                                    UserId = x.UserId,
                                    FullName = x.FullName
                                }).ToList() ?? new List<EmailHistoryEntity.UserInfo>();

                            if (infor.Any())
                            {
                                EmailHistoryEntity.UserInfo uf = new EmailHistoryEntity.UserInfo()
                                {
                                    UserId = user.Id,
                                    FullName = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName
                                };
                                infor.Add(uf);
                            }
                        }

                        emailHistories.UserSent = JsonConvert.SerializeObject(infor);
                        bool updateResult = await _emailHistoryService.UpdateEmailHistory(emailHistories);

                        if (!updateResult)
                        {
                            Utilities.WriteLogError($"Failed to update EmailHistory for ReferenceNumber: {refe}");
                        }
                    }
                }
            }
        }
        #endregion

        #region SendMail
        async Task<bool> SendEmail(EmailTemplateName template, EmailTemplateName layoutName, Dictionary<string, string> mergedFields, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
        {
            if (recipients != null && recipients.Any())
            {
                Utilities.WriteLogError($"Sending email to: {string.Join("; ", recipients)}");
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
        private EmailNotificationModel ProcessEmailNotificationArgs(EmailTemplateModel emailTemplate, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
        {
            try
            {
                EmailNotificationModel email = new EmailNotificationModel();
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
        private async Task SendBySmtpClient(EmailNotificationModel args, EmailTemplateModel type)
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
                    //mailwait.CreatedBy = _uow.UserContext.CurrentUserName;
                    //mailwait.ModifiedBy = _uow.UserContext.CurrentUserName;
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
    }
}