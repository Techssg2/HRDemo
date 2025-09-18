using Aeon.Academy.Common.Configuration;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using Aeon.Academy.IntegrationServices;
using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;

namespace Aeon.Academy.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly ILogger _logger = new Logger();
        private readonly SmtpSection _section;
        private readonly IQueueService queueService;
        private readonly IGenericRepository<TrainingInvitationParticipant> participantRepository = null;
        public IntegrationService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this._section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            this.queueService = new QueueService(unitOfWork);
            this.participantRepository = unitOfWork.GetRepository<TrainingInvitationParticipant>();
        }

        public HttpResponseMessage SyncSAP(TrainingInvitationParticipant participant, ref int update, ref int fail, bool isSendMail = false)
        {
            var data = BuilModel(participant, out string referenceNumber, out Guid invitationId);
            //call sync SAP
            var response = new SapService().SyncData(data);
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            var item = queueService.GetBySapCode(participant.SapCode, invitationId);
            var isQueueItem = false;
            var jsonData = Common.Utils.CommonUtil.SerializeObject(data);
            if (item != null)
            {
                item.Response = responseMsg;
                item.NumberOfCall = item.NumberOfCall + 1;
                item.Modified = DateTime.Now;
                isQueueItem = true;
            }
            else
            {
                item = new ServiceQueue
                {
                    InstanceData = jsonData,
                    InstanceType = IntegraitionType.Sap,
                    ErrorMessage = responseMsg,
                    Disabled = 0,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    NumberOfCall = 1,
                    Status = "Failed",
                    ReferenceNumber = referenceNumber,
                    TrainingInvitationId = invitationId,
                    SapCode = participant.SapCode,
                    Response = responseMsg
                };
            }
            if (!response.IsSuccessStatusCode)
            {
                item.ErrorMessage = item.ErrorMessage + Environment.NewLine + $"StatusCode: {response.StatusCode.ToString()} - Error message: {responseMsg}";
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    item.Disabled = 1;
                }
                queueService.Save(item);
                if(item.Disabled == 1 && isSendMail)
                {
                    var error = BuildError(referenceNumber, participant.SapCode, responseMsg);
                    SendEmailNotification(error);
                }
                fail = fail + 1;
                UpdateParticipant(participant, Common.Consts.SAPStatusCode.Fail);
                _logger.LogError(string.Format("SAP integration failed: Reference - {2}, Sap Code - {3}, Reason {0}, Message {1}", response.ReasonPhrase, responseMsg, referenceNumber, participant.SapCode));
                return response;
            }
            else
            {
                var res = Common.Utils.CommonUtil.DeserializeObject<AcademyTrainingResponse>(responseMsg);
                if (res.Data.Status != "S")
                {
                    item.Disabled = 1;
                    fail = fail + 1;
                    item.ErrorMessage = item.ErrorMessage + Environment.NewLine + $"StatusCode: {response.StatusCode.ToString()} - Error message: {responseMsg}";
                    queueService.Save(item);
                    var error = BuildError(referenceNumber, participant.SapCode, responseMsg);
                    UpdateParticipant(participant, Common.Consts.SAPStatusCode.Fail);
                    _logger.LogError(error);
                    if (isSendMail)
                    {
                        // send email to Support Team
                        SendEmailNotification(error);
                    }
                }
                else
                {
                    if (isQueueItem)
                    {
                        item.Disabled = 1;
                        item.Status = "Success";
                        queueService.Save(item);
                    }
                    update = update + 1;
                    UpdateParticipant(participant, Common.Consts.SAPStatusCode.Submit);
                    _logger.LogInfo("SAP Integration Sync successfully. Reference - " + referenceNumber + ", Sap Code - " + participant.SapCode);
                }
                return response;
            }
        }
        private static string BuildError(string referenceNumber, string sapCode, string response)
        {
            var error = $"SAP integration failed - {referenceNumber} - Sap code: {sapCode}" + Environment.NewLine +
                $"Response: {response}";
            return error;
        }
        public AcademyTrainingRequest BuilModel(TrainingInvitationParticipant participant, out string referenceNumber, out Guid invitationId)
        {
            var invitation = participant.TrainingInvitation;
            var programCode = "IND";
            if (invitation == null)
            {
                invitation = unitOfWork.GetRepository<TrainingInvitation>().Get(participant.TrainingInvitationId);
                invitation = invitation ?? new TrainingInvitation();
            }
            var trainingRequest = invitation.TrainingRequest;
            if (trainingRequest == null)
            {
                trainingRequest = unitOfWork.GetRepository<TrainingRequest>().Get(invitation.TrainingRequestId);
                trainingRequest = trainingRequest ?? new TrainingRequest();
                programCode = invitation.CourseName.Length >= 3 ? invitation.CourseName.Substring(0, 3) : programCode;
            }
            if (trainingRequest.Course != null)
            {
                programCode = trainingRequest.Course.Code.Length >=3 ? trainingRequest.Course.Code.Substring(0, 3): programCode;
            }
            var location = trainingRequest.TrainingMethod.Equals("online", StringComparison.CurrentCultureIgnoreCase) ? "Online" : invitation.TrainingLocation;
            var data = new AcademyTrainingRequest
            {
                Pernr = participant.SapCode,
                Begda = invitation.StartDate.ToString("yyyyMMdd"),
                Endda = invitation.EndDate.ToString("yyyyMMdd"),
                Ztrain_loc = location,
                Zprg_code = programCode.ToUpper(),
                Zprogram = invitation.CourseName,
                Zhours_day = invitation.HoursPerDay.ToString(),
                Znumofday = invitation.NumberOfDays.ToString(),
                Zagency = invitation.ServiceProvider,
                Ztrainer = string.Empty,
                Ztrain_cost = trainingRequest.TrainingFee.ToString("#"),
                Ztrain_cont = trainingRequest.SponsorshipContractNumber ?? string.Empty,
                Zstart = trainingRequest.WorkingCommitmentFrom != null ? trainingRequest.WorkingCommitmentFrom.Value.ToString("yyyyMMdd") : string.Empty,
                Zend = trainingRequest.WorkingCommitmentTo != null ? trainingRequest.WorkingCommitmentTo.Value.ToString("yyyyMMdd") : string.Empty,
                Ztotal_hours = invitation.TotalHours.ToString()
            };
            if (trainingRequest.TypeOfTraining.Equals("internal", StringComparison.CurrentCultureIgnoreCase))
            {
                data.Zin_ex = "I";
                data.Zagency = string.Empty;
                data.Ztrain_cost = string.Empty;
                data.Ztrainer = invitation.TrainerName;
            }
            else if (trainingRequest.TypeOfTraining.Equals("external", StringComparison.CurrentCultureIgnoreCase))
            {
                data.Zin_ex = "II";
            }
            invitationId = participant.TrainingInvitationId;
            referenceNumber = trainingRequest.ReferenceNumber;
            return data;
        }
        private void SendEmailNotification(string error)
        {
            var emailTo = ApplicationSettings.AcademySupportEmail;
            if (!string.IsNullOrEmpty(emailTo))
            {
                try
                {
                    var smtpClient = new SmtpClient(_section.Network.Host, _section.Network.Port)
                    {
                        EnableSsl = _section.Network.EnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(_section.Network.UserName, _section.Network.Password),
                        UseDefaultCredentials = _section.Network.DefaultCredentials,
                    };
                    MailMessage email = new MailMessage();
                    var fromEmailAddress = string.IsNullOrEmpty(_section.From) ? "e-document.notification@aeon.com.vn" : _section.From;
                    email.From = new MailAddress(fromEmailAddress, "AEON Notification");
                    email.To.Add(emailTo);
                    email.Subject = "[Failed] integrate Academy and SAP";
                    email.Body = error;
                    email.IsBodyHtml = true;
                    smtpClient.Send(email);

                    _logger.LogInfo("Email sent successfully: " + fromEmailAddress + " to " + email.To);
                    smtpClient.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Email sent failed: " + ex);
                }
            }
        }
        private void UpdateParticipant(TrainingInvitationParticipant participant, Common.Consts.SAPStatusCode code)
        {
            participant.SapStatusCode = (int) code;
            participantRepository.Update(participant);
            unitOfWork.Complete();
        }
    }
}
