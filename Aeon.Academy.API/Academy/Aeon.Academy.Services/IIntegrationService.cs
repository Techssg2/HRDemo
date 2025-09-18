using Aeon.Academy.Common.Entities;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    public interface IIntegrationService
    {
        HttpResponseMessage SyncSAP(TrainingInvitationParticipant participant, ref int update, ref int fail, bool isSendMail = false);
        AcademyTrainingRequest BuilModel(TrainingInvitationParticipant participant, out string referenceNumber, out Guid invitationId);
    }
}
