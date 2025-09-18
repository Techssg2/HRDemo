using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.HR.ViewModels.Args;
using Microsoft.Extensions.Logging;
using Aeon.HR.Infrastructure.Interfaces;
using System.Net.Configuration;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using Aeon.HR.BusinessObjects.Helpers;
using AutoMapper;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public class EmailNotification : IEmailNotification
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly SmtpSection _section;
        private readonly string URL_Default = "http://api.email-waitlist.aeon.com.vn/_api/";
        public EmailNotification(ILogger log, IUnitOfWork uow) : base()
        {
            _logger = log;
            _uow = uow;
            _section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }
        public async Task<bool> SendEmail(EmailTemplateName template, EmailTemplateName layoutName, Dictionary<string, string> mergedFields, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
        {
            if (recipients != null && recipients.Any())
            {
                _logger.LogDebug(string.Format("Sending email to {0}", string.Join(";", recipients)));
            }
            var result = false;
            try
            {
                var mailTemplate = await GetEmailTemplate(template.ToString());
                if (mailTemplate is null)
                {
                    _logger.LogError("Get email template from API is null");
                    return false;
                }
                var layout = new EmailTemplate() { Body = mailTemplate.Body, Subject = mailTemplate.Subject, TemplatCode = mailTemplate.Code, Name = mailTemplate.Subject };
                var emailTemplate = new EmailTemplate() { Body = mailTemplate.Body, Subject = mailTemplate.Subject, TemplatCode = mailTemplate.Code, Name = mailTemplate.Code };
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
                        _logger.LogError("Error when send email");
                    }
                }
                else
                {
                    _logger.LogDebug(string.Format("Email Template '{0}' not found.", template.ToString()));
                }

                result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return result;
        }   
        private void AddDefaultMergedFields(Dictionary<string, string> mergedFields)
        {
            if (mergedFields == null)
            {
                mergedFields = new Dictionary<string, string>();
            }
            mergedFields["BaseUrl"] = "Aeon";
        }
        private EmailNotificationArgs ProcessEmailNotificationArgs(EmailTemplate emailTemplate, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null)
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
                if(ccRecipients!= null)
                {
                    email.CcRecipients = ccRecipients;
                }
                return email;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in ProcessEmailNotificationArgs()", ex);
            }
            return null;
        }
        private void ProcessEmailContent(EmailTemplate template, EmailTemplate layout, Dictionary<string, string> mergedFields)
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
                _logger.LogError("Error in ProcessEmailContent()", ex);
            }
        }
        private async Task SendBySmtpClient(EmailNotificationArgs args, EmailTemplate type)
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
                if(args.CcRecipients != null)
                {
                    foreach(var cc in args.CcRecipients)
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
                                /*var attachment = new System.Net.Mail.Attachment(stream, fileName);*/
                                attachmentFile.Add(new L_AttachmentFile() { FileName = fileName, Base64 = Convert.ToBase64String(stream.ToArray()) });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error.Attachment: " + e.Message);
                    }

                    var mailwait = new MailWaitList();
                    mailwait.TemplateCode = type.TemplatCode;
                    mailwait.Subject = type.Subject;
                    mailwait.Body = mail.Body;
                    mailwait.MailTo = new List<string>() { mail.To.ToString() };
                    mailwait.MailCC = new List<string>() {};
                    mailwait.MailBCC = new List<string>() {};
                    mailwait.CreatedBy = _uow.UserContext.CurrentUserName;
                    mailwait.ModifiedBy = _uow.UserContext.CurrentUserName;
                    mailwait.Module = "Edoc2";
                    mailwait.Attachments = attachmentFile;
                    mailwait.SendCount = 0;
                    mailwait.SendDate = DateTimeOffset.Now.AddSeconds(30);
                    await SendMailToWaitList(mailwait);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error.SendBySmtpClient: " + ex.Message);
                _logger.LogError("Error.SendBySmtpClient: " + ex.StackTrace);
            }
        }
        public async Task<ResponseCreateEmailWaitlistModel> SendMailToWaitList(MailWaitList email)
        {
            var responseCreateMail = new ResponseCreateEmailWaitlistModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var baseUrl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["mailAPI"]) ? URL_Default : ConfigurationManager.AppSettings["mailAPI"];
                    var requestUri = new Uri(new Uri(baseUrl), "CreateEmailWaitlist");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonContent = JsonConvert.SerializeObject(email);
                    _logger.LogInformation("SendMailToWaitList.jsonContent: " + jsonContent);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string httpResponseResult = await response.Content.ReadAsStringAsync();
                        responseCreateMail = JsonConvert.DeserializeObject<ResponseCreateEmailWaitlistModel>(httpResponseResult);
                        _logger.LogInformation("CreateMailTemplate.Status: " + responseCreateMail.Status);
                    }
                    /*var payload = JsonConvert.SerializeObject(email);
                    var convertPaylload = JsonConvert.DeserializeObject<MailWaitList>(payload);
                    foreach(var item in convertPaylload.Attachments)
                    {
                        byte[] fileData = Convert.FromBase64String(item.Base64);
                        string filePath = "D:\\Database\\Save\\" + item.FileName; // Đường dẫn tới file Excel cần lưu
                        System.IO.File.WriteAllBytes(filePath, fileData);
                    }*/
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("responseCreateMail: " + ex.Message);
                _logger.LogError("responseCreateMail: " + ex.StackTrace);
            }
            return responseCreateMail;
        }

        //api 
        public async Task<ResponseEmailTemplateModel> GetEmailTemplate(string name)
        {
            var responseEmailTemplate = new ResponseEmailTemplateModel();
            try
            {
                using (var client = new HttpClient())
                {
                    var baseUrl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["mailAPI"]) ? URL_Default : ConfigurationManager.AppSettings["mailAPI"];
                    _logger.LogInformation("GetEmailTemplate.baseUrl: " + baseUrl);
                    var requestUri = new Uri(new Uri(baseUrl), "GetEmailTemplateByCode");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var model = new EmailTemplateViewModel { Code = name };
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
                _logger.LogError("Error: " + ex.Message);
            }
            return responseEmailTemplate;
        }
    }

    public class EmailTemplateViewModel
    {
        public string Code { get; set; }
    }

    public class ResponseEmailTemplateModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
    }

    public class ResponseCreateEmailWaitlistModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

}
