using Aeon.Academy.Common.Consts;
using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    public class DoTaskService : IDoTaskService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<TrainingInvitationParticipant> repoInvitationParticipant = null;
        private readonly SmtpSection _section;
        private readonly ILogger _logger;

        public DoTaskService(IUnitOfWork<AppDbContext> unitOfWork, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.repoInvitationParticipant = unitOfWork.GetRepository<TrainingInvitationParticipant>();
            this._section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            this._logger = logger;
        }

        public void SendInviteeNotification()
        {
            var participants = repoInvitationParticipant.Query(p => p.Response == ResponseType.Pending && p.TrainingInvitation.Status == TrainingInvitationStatus.SendInvitation).ToList();

            if (participants != null && participants.Any())
            {
                var smtpClient = new SmtpClient(_section.Network.Host, _section.Network.Port)
                {
                    EnableSsl = _section.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(_section.Network.UserName, _section.Network.Password),
                    UseDefaultCredentials = _section.Network.DefaultCredentials,
                };
                foreach (var participant in participants)
                {
                    MailMessage email = new MailMessage();

                    var fromEmailAddress = string.IsNullOrEmpty(_section.From) ? "e-document.notification@aeon.com.vn" : _section.From;
                    email.From = new MailAddress(fromEmailAddress, "AEON Notification");
                    email.To.Add(participant.Email);
                    email.Subject = "Training Invitation";
                    email.Body = participant.EmailContent;
                    email.IsBodyHtml = true;
                    smtpClient.Send(email);

                    _logger.LogInfo("Email sent successfully: " + fromEmailAddress + " to " + email.To);
                }
                smtpClient.Dispose();
            }



            return;
        }
    }
}
