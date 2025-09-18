using Aeon.Academy.API.Core;
using Aeon.Academy.API.DTOs;
using Aeon.Academy.API.Filters;
using Aeon.Academy.API.Mappers;
using Aeon.Academy.API.Utils;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Windows;

namespace Aeon.Academy.API.Controllers
{
    public class TrainingInvitationController : BaseAuthApiController
    {
        private readonly ITrainingInvitationService trainingInvitationService;
        private readonly SmtpSection _section;
        private readonly ILogger _logger;
        private readonly SharepointFile sharepointFile;

        public TrainingInvitationController(ITrainingInvitationService invitationService, ILogger logger)
        {
            this.trainingInvitationService = invitationService;
            this._section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            this._logger = logger;
            this.sharepointFile = new SharepointFile(DocumentLibraryName.TrainingInvitation);
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var request = trainingInvitationService.Get(id);
            if (request == null) return Ok(new { });
            var dto = request.ToDto();
            var attachments = sharepointFile.GetCourseDocument(request.Id.ToString());
            dto.Attachments = attachments;
            return Ok(dto);
        }

        [HttpPost]
        [ValidateModel]
        public IHttpActionResult GetAll(TrainingInvitationFilter filter)
        {
            var request = trainingInvitationService.GetAll(CurrentUser, filter);
            if (request == null) return NotFound();

            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToDtos()
            });
        }

        [HttpGet]
        public IHttpActionResult GetByRequest(Guid requestId)
        {
            var request = trainingInvitationService.GetByRequest(requestId);
            if (request == null) return Ok(new { });
            var dto = request.ToDto();
            var attachments = sharepointFile.GetCourseDocument(request.Id.ToString());
            dto.Attachments = attachments;

            return Ok(dto);
        }

        [HttpGet]
        public IHttpActionResult GetByParticipant(Guid id)
        {
            var request = trainingInvitationService.GetParticipant(id);
            if (request == null) return NotFound();
            var formActions = trainingInvitationService.GetEnabledActions(request, CurrentUser.Id);
            var dto = request.ToDto();
            dto.EnabledActions = formActions;
            var attachments = sharepointFile.GetCourseDocument(request.TrainingInvitationId.ToString());
            dto.Attachments = attachments;

            return Ok(dto);
        }

        [HttpPost]
        [ValidateModel]
        public IHttpActionResult ListAllInvitations(TrainingInvitationFilter filter)
        {
            filter.UserId = CurrentUser.Id;
            var request = trainingInvitationService.GetAllInvitations(CurrentUser, filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToDtos()
            });
        }

        [HttpPost]
        [ValidateModel]
        public IHttpActionResult ListMyInvitations(TrainingInvitationFilter filter)
        {
            filter.UserId = CurrentUser.Id;
            var request = trainingInvitationService.ListMyInvitations(filter);
            if (request == null) return NotFound();
            return Ok(new
            {
                Count = request.TotalCount,
                Totalpages = request.TotalPages,
                Data = request.ToDtos()
            });
        }

        [HttpPost]
        [ValidateModel]
        public IHttpActionResult Save(TrainingInvitationDto dto)
        {

            var trainingInvitation = trainingInvitationService.GetByRequest(dto.TrainingRequestId);
            if (trainingInvitation != null)
            {
                dto.Id = trainingInvitation.Id;
                dto.Status = trainingInvitation.Status;
            }
            var request = dto.ToEntity(trainingInvitation, CurrentUser);
            if (!IsAuthorized(request)) return Unauthorized();
            request.Status = TrainingInvitationStatus.Save;
            var id = trainingInvitationService.Save(request);
            if (id == Guid.Empty)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("Can not Create")
                };
                return ResponseMessage(response);
            }
            var result = sharepointFile.UploadFiles(id.ToString(), dto.Attachments);
            if (!string.IsNullOrEmpty(result))
            {
                //delete temporary
                if (dto.Id == Guid.Empty)
                {
                    var invitation = trainingInvitationService.Get(id);
                    trainingInvitationService.Delete(invitation);
                }
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(result)
                };
                return ResponseMessage(response);
            }

            return Ok(new { Id = id });
        }

        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            var trainingInvitation = trainingInvitationService.Get(id);
            if (trainingInvitation == null) return NotFound();
            if (!IsAuthorized(trainingInvitation)) return Unauthorized();
            var delete = trainingInvitationService.Delete(trainingInvitation);
            if (!delete)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent("Can not delete")
                };
                return ResponseMessage(response);
            }

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Accept(TrainingInvitationParticipantDto dto)
        {
            var request = trainingInvitationService.Get(dto.TrainingInvitationId);
            var participant = trainingInvitationService.GetParticipant(dto.Id);
            if (request == null || participant == null) return NotFound();
            if (!IsAuthorized(participant)) return Unauthorized();
            var id = trainingInvitationService.Accept(participant);

            return Ok(new { Id = id });
        }

        [HttpPost]
        public IHttpActionResult Decline(TrainingInvitationParticipantDto dto)
        {
            var request = trainingInvitationService.Get(dto.TrainingInvitationId);
            var participant = trainingInvitationService.GetParticipant(dto.Id);
            if (request == null || participant == null) return NotFound();
            if (!IsAuthorized(participant)) return Unauthorized();
            participant.ReasonOfDecline = dto.ReasonOfDecline;
            var id = trainingInvitationService.Decline(participant);

            return Ok(new { Id = id });
        }

        [HttpPost]
        public IHttpActionResult SendInvitation(TrainingInvitationDto dto)
        {
            var invitation = trainingInvitationService.Get(dto.Id);
            var request = dto.ToEntity(invitation, CurrentUser);
            if (invitation == null) return NotFound();
            if (!IsAuthorized(invitation)) return Unauthorized();
            request.Status = TrainingInvitationStatus.SendInvitation;

            try
            {
                var smtpClient = new SmtpClient(_section.Network.Host, _section.Network.Port)
                {
                    EnableSsl = _section.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_section.Network.UserName, _section.Network.Password),
                    UseDefaultCredentials = _section.Network.DefaultCredentials,
                };
                var listManager = new List<User>();
                foreach (var participant in request.TrainingInvitationParticipants)
                {
                    MailMessage employeeEmail = new MailMessage();

                    employeeEmail.From = new MailAddress(CurrentUser.Email);
                    employeeEmail.To.Add(participant.Email);
                    var emloyeeManager = trainingInvitationService.FindEmployeeManager(participant);
                    if (emloyeeManager != null)
                    {
                        if (listManager == null || listManager.Count == 0)
                        {
                            listManager.Add(emloyeeManager);
                        }
                        else
                        {
                            if (!listManager.Contains(emloyeeManager))
                            {
                                listManager.Add(emloyeeManager);
                            }
                        }
                    }
                    employeeEmail.Subject = "Thông Báo Mời Học";
                    employeeEmail.Body = participant.EmailContent;
                    employeeEmail.IsBodyHtml = true;
                    smtpClient.Send(employeeEmail);
                    _logger.LogInfo("Email sent successfully: " + employeeEmail.From + " to " + employeeEmail.To);
                }
                if (listManager != null && listManager.Count != 0)
                {
                    foreach (var manager in listManager)
                    {
                        var hodEmailTemplate = InvitationEmailTemplate.HODBodyEmail.Replace(InvitationEmailTemplate.EmployeeHODName, manager.FullName).Replace(InvitationEmailTemplate.TrainingLocation, request.TrainingLocation)
                        .Replace(InvitationEmailTemplate.Coursename, request.CourseName).Replace(InvitationEmailTemplate.StartDate, request.StartDate.ToString("MMMM d, yyyy"));
                        hodEmailTemplate = string.IsNullOrEmpty(request.ServiceProvider)
                            ? hodEmailTemplate.Replace(InvitationEmailTemplate.ServiceProvider, InvitationEmailTemplate.AeonVN)
                            : hodEmailTemplate.Replace(InvitationEmailTemplate.ServiceProvider, request.ServiceProvider); ;
                        MailMessage employeeHODEmail = new MailMessage();
                        employeeHODEmail.From = new MailAddress(CurrentUser.Email);
                        employeeHODEmail.To.Add(manager.Email);
                        employeeHODEmail.Subject = "Thông Báo Mời Học";
                        employeeHODEmail.Body = hodEmailTemplate;
                        employeeHODEmail.IsBodyHtml = true;
                        smtpClient.Send(employeeHODEmail);
                        _logger.LogInfo("Email sent successfully: " + employeeHODEmail.From + " to " + employeeHODEmail.To);
                    }

                }
                smtpClient.Dispose();
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent("Send email failed")
                };
                _logger.LogError(ex.ToString());
                return ResponseMessage(response);
            }
            var id = trainingInvitationService.Save(request);

            return Ok(new { Id = id });
        }

        [HttpPost]
        public IHttpActionResult CancelInvitation(TrainingInvitationDto dto)
        {
            var invitation = trainingInvitationService.Get(dto.Id);
            var request = dto.ToEntity(invitation, CurrentUser);
            if (invitation == null) return NotFound();
            if (!AllowCancelInvitation(invitation)) return Unauthorized();
            request.Status = TrainingInvitationStatus.Cancelled;

            //Waiting implement send noticed cancel 
            //try
            //{
            //    var smtpClient = new SmtpClient(_section.Network.Host, _section.Network.Port)
            //    {
            //        EnableSsl = _section.Network.EnableSsl,
            //        DeliveryMethod = SmtpDeliveryMethod.Network,
            //        Credentials = new NetworkCredential(_section.Network.UserName, _section.Network.Password),
            //        UseDefaultCredentials = _section.Network.DefaultCredentials,
            //    };

            //    foreach (var participant in dto.Participants)
            //    {
            //        MailMessage mail = new MailMessage();
            //        mail.From = new MailAddress(CurrentUser.Email);
            //        mail.To.Add(participant.Email);
            //        var hodEmail = trainingInvitationService.FindHODEmailInDepartment(participant.ParticipantId);
            //        mail.Subject = "Thông Báo Hủy Khóa Học";
            //        mail.Body = participant.EmailContent;
            //        mail.IsBodyHtml = true;
            //        smtpClient.Send(mail);
            //        _logger.LogInfo("Email sent successfully: " + mail.From + " to " + mail.To);
            //    }
            //    smtpClient.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    var response = new HttpResponseMessage(HttpStatusCode.NotImplemented)
            //    {
            //        Content = new StringContent("Send email failed")
            //    };
            //    _logger.LogError(ex.ToString());
            //    return ResponseMessage(response);
            //}
            var id = trainingInvitationService.Save(request);

            return Ok(new { Id = id });
        }

        private bool IsAuthorized(TrainingInvitation request)
        {
            if (trainingInvitationService.CheckAcademyUser(CurrentUser.Id) && request.Status != TrainingInvitationStatus.SendInvitation &&
                request.Status != TrainingInvitationStatus.WaitingForAfterReport && request.Status != TrainingInvitationStatus.Completed && request.Status != TrainingInvitationStatus.Cancelled)
                return true;
            return false;
        }
        private bool AllowCancelInvitation(TrainingInvitation request)
        {
            if (trainingInvitationService.CheckAcademyUser(CurrentUser.Id) && request.Status == TrainingInvitationStatus.SendInvitation)
                return true;
            return false;
        }
        private bool IsAuthorized(TrainingInvitationParticipant participant)
        {
            if (participant.ParticipantId == CurrentUser.Id && participant.Response == ResponseType.Pending && participant.TrainingInvitation.Status == TrainingInvitationStatus.SendInvitation)
                return true;
            return false;
        }
    }
}