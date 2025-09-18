using Aeon.HR.BusinessObjects.Attributes;
using Aeon.HR.BusinessObjects.Handlers;
using Aeon.HR.BusinessObjects.Handlers.Other;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.CompleteActions
{
    [Action(Type = typeof(BusinessTripApplication))]
    public class BTACompleteAction : ICompleteAction
    {
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");
        public async Task Execute(IUnitOfWork uow, Guid itemId, IDashboardBO dashboardBO, IWorkflowBO workflowBO, ILogger logger)
        {
            try
            {
                var businessTrip = await uow.GetRepository<BusinessTripApplication>(true).FindByIdAsync(itemId);
                if (businessTrip != null)
                {
                    var attachments = new Dictionary<string, byte[]>();
                    ILogger<AttachmentFileBO> _loggerAttachment = null;
                    AttachmentFileBO _attachmentFileBO = new AttachmentFileBO(uow, _loggerAttachment);
                    if (businessTrip.Status == "Completed")
                    {
                        var attachmentDetails = string.IsNullOrEmpty(businessTrip.DocumentDetails) ? new List<BusinessTripFileDTO>() : JsonConvert.DeserializeObject<List<BusinessTripFileDTO>>(businessTrip.DocumentDetails);
                        var businessTripDetails = await uow.GetRepository<BusinessTripApplicationDetail>().FindByAsync(x => x.BusinessTripApplicationId == businessTrip.Id);
                        if (businessTripDetails.Any() && attachmentDetails.Count > 0)
                        {
                            foreach (var x in attachmentDetails)
                            {
                                var attachment = await _attachmentFileBO.Get(x.Id);
                                if (attachment != null)
                                {
                                    var att = Mapper.Map<AttachmentFileViewModel>(attachment);
                                    var filepath = Path.Combine(_uploadedFilesFolder, att.FileUniqueName);
                                    byte[] imageArray = System.IO.File.ReadAllBytes(filepath);
                                    attachments.Add(att.FileDisplayName, imageArray);
                                }
                            }
                            //
                            foreach (var item in businessTripDetails)
                            {
                                if (item.Email != null)
                                {
                                    await SendEmailBTA(uow, logger, EmailTemplateName.BTASendFileWhenComplete, item.Email, item.FullName, attachments);
                                }
                            }
                        }
                    }
                    else if (businessTrip.Status == "Completed Changing")
                    {
                        var attachmentChanges = string.IsNullOrEmpty(businessTrip.DocumentChanges) ? new List<BusinessTripFileDTO>() : JsonConvert.DeserializeObject<List<BusinessTripFileDTO>>(businessTrip.DocumentChanges);
                        var businessTripChanges = await uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == businessTrip.Id && !x.IsCancel);
                        var businessTripChangeIsCancels = await uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == businessTrip.Id && x.IsCancel);
                        if (businessTripChanges.Any() && attachmentChanges.Count > 0)
                        {
                            foreach (var x in attachmentChanges)
                            {
                                var attachment = await _attachmentFileBO.Get(x.Id);
                                if (attachment != null)
                                {
                                    var att = Mapper.Map<AttachmentFileViewModel>(attachment);
                                    var filepath = Path.Combine(_uploadedFilesFolder, att.FileUniqueName);
                                    byte[] imageArray = System.IO.File.ReadAllBytes(filepath);
                                    attachments.Add(att.FileDisplayName, imageArray);
                                }

                            }
                            //
                            foreach (var item in businessTripChanges)
                            {
                                if (item.Email != null)
                                {
                                    await SendEmailBTA(uow, logger, EmailTemplateName.BTASendFileWhenComplete, item.Email, item.FullName, attachments);
                                }
                            }
                        }
                        if (businessTripChangeIsCancels.Any())
                        {
                            var groupSapCodes = businessTripChangeIsCancels.GroupBy(u => u.SAPCode).ToList();
                            foreach (var sapCodes in groupSapCodes)
                            {
                               
                                var cancelDetailsEN = new StringBuilder();
                                var cancelDetailsVN = new StringBuilder();
                                cancelDetailsEN.Append("<ul>" + Environment.NewLine);
                                cancelDetailsVN.Append("<ul>" + Environment.NewLine);
                                foreach (var item in sapCodes)
                                {
                                    cancelDetailsEN.AppendFormat("<li>{0} - {1} to {2}</li>", String.Format("{0:d/M/yyyy hh:mm tt}", (item.NewFromDate != null && item.NewFromDate.HasValue) ? item.NewFromDate.Value.LocalDateTime : item.FromDate.Value.LocalDateTime), String.Format("{0:d/M/yyyy hh:mm tt}", (item.NewToDate != null && item.NewToDate.HasValue) ? item.NewToDate.Value.LocalDateTime : item.ToDate.Value.LocalDateTime), item.DestinationName, Environment.NewLine);
                                    cancelDetailsVN.AppendFormat("<li>{0} - {1} đến {2}</li>", String.Format("{0:d/M/yyyy hh:mm tt}", (item.NewFromDate != null && item.NewFromDate.HasValue) ? item.NewFromDate.Value.LocalDateTime : item.FromDate.Value.LocalDateTime), String.Format("{0:d/M/yyyy hh:mm tt}", (item.NewToDate != null && item.NewToDate.HasValue) ? item.NewToDate.Value.LocalDateTime : item.ToDate.Value.LocalDateTime), item.DestinationName, Environment.NewLine);
                                }
                                cancelDetailsEN.Append("</ul>" + Environment.NewLine);
                                cancelDetailsVN.Append("</ul>" + Environment.NewLine);
                                StringBuilder[] cancelledDetails = new StringBuilder[2] { cancelDetailsEN, cancelDetailsVN };
                                await SendEmailBTA(uow, logger, EmailTemplateName.BTASendFileWhenCompleteChanging, sapCodes.FirstOrDefault().Email, sapCodes.FirstOrDefault().FullName, null, cancelledDetails);
                            }
                        }
                        //add from date and to date to list bta
                        var listBusinessTrip = new List<BusinessTripApplicationDetail>();
                        listBusinessTrip.AddRange(businessTrip.BusinessTripApplicationDetails);
                        var changeCancelBusinessTripDetails = await uow.GetRepository<ChangeCancelBusinessTripDetail>().FindByAsync(x => x.BusinessTripApplicationId == businessTrip.Id);
                        foreach (var changeCancelBusinessTripDetail in changeCancelBusinessTripDetails)
                        {
                            if (!changeCancelBusinessTripDetail.IsCancel)
                            {
                                var oldItem = listBusinessTrip.FirstOrDefault(x => x.BusinessTripApplicationId == changeCancelBusinessTripDetail.BusinessTripApplicationId);
                                oldItem.FromDate = changeCancelBusinessTripDetail.FromDate;
                                oldItem.ToDate = changeCancelBusinessTripDetail.ToDate;
                            }
                        }
                        businessTrip.BusinessTripFrom = listBusinessTrip.OrderBy(x => x.FromDate).FirstOrDefault().FromDate;
                        businessTrip.BusinessTripTo = listBusinessTrip.OrderByDescending(x => x.ToDate).FirstOrDefault().ToDate;
                        await uow.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error at method: Execute BTACompleteAction " + ex.Message);
            }
        }

        private async Task SendEmailBTA(IUnitOfWork uow, ILogger logger, EmailTemplateName type, string email, string fullName, Dictionary<string, byte[]> attachments = null, StringBuilder []cancelledDetails = null)
        {
            try
            {
                EmailNotification emailNotification = new EmailNotification(logger, uow);
                logger.LogInformation($"Send email BTA: ${fullName}");
                var mergeFields = new Dictionary<string, string>();
                mergeFields["FullName"] = fullName;
                if (cancelledDetails != null)
                {
                    mergeFields["CancelledDetailsEN"] = cancelledDetails[0].ToString();
                    mergeFields["CancelledDetailsVN"] = cancelledDetails[1].ToString();
                }
                var recipients = new List<string>() { email };
                await emailNotification.SendEmail(type, type, mergeFields, recipients, attachments);
            }
            catch (Exception ex)
            {
                logger.LogError("Error at method: SendEmailNotificationToAssignee " + ex.Message);
            }
        }
    }
}
