using Aeon.Academy.Common.Entities;
using Aeon.Academy.Common.Utils;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface ITrainingInvitationService
    {
        TrainingInvitation Get(Guid id);
        PagedList<TrainingInvitation> GetAll(User currentUser, TrainingInvitationFilter filter);
        TrainingInvitation GetByRequest(Guid requestId);
        TrainingInvitationParticipant GetParticipant(Guid id);
        PagedList<TrainingInvitationParticipant> GetAllInvitations(User currentUser, TrainingInvitationFilter filter);
        PagedList<TrainingInvitationParticipant> ListMyInvitations(TrainingInvitationFilter filter);
        PagedList<TrainingInvitation> ListPendingByUserId(Guid userId, int pageNumber, int pageSize);
        PagedList<TrainingInvitation> ListAfterReportByUserId(Guid userId, int pageNumber, int pageSize);
        Guid Save(TrainingInvitation trainingInvitation);
        bool Delete(TrainingInvitation trainingInvitation);
        Guid Accept(TrainingInvitationParticipant participant);
        Guid Decline(TrainingInvitationParticipant participant);
        bool CheckAcademyUser(Guid UserId);
        List<string> GetEnabledActions(TrainingInvitationParticipant participant, Guid UserId);
        User FindEmployeeManager(TrainingInvitationParticipant participant);
        TrainingInvitationParticipant GetParticipantBySapCode(string sapCode, Guid? invitationId);
        List<TrainingInvitationParticipant> GetParticipantAccepted(Guid invitationId);
        List<TrainingInvitationParticipant> GetParticipantsWithNotSubmitted();
    }
}
