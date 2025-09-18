using Aeon.Academy.API.DTOs;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Data.Entities;
using System.Collections.Generic;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingInvitationParticipantMapper
    {
        public static TrainingInvitationParticipant ToEntity(this TrainingInvitationParticipantDto dto, User user)
        {
            var participant = new TrainingInvitationParticipant
            {
                Id = dto.Id,
                TrainingInvitationId = dto.TrainingInvitationId,
                ParticipantId = user.Id,
                SapCode = dto.SapCode,
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Position = dto.Position,
                Department = dto.Department,
                EmailContent = dto.EmailContent,
                Response = dto.Response,
                ReasonOfDecline = dto.ReasonOfDecline,
                StatusOfReport = dto.StatusOfReport
            };

            return participant;
        }

        public static TrainingInvitationParticipantDto ToDto(this TrainingInvitationParticipant entity)
        {
            var dto = new TrainingInvitationParticipantDto
            {
                Id = entity.Id,
                TrainingInvitationId = entity.TrainingInvitationId,
                ParticipantId = entity.ParticipantId,
                SapCode = entity.SapCode,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Position = entity.Position,
                Department = entity.Department,
                EmailContent = entity.EmailContent,
                Response = entity.Response,
                ReasonOfDecline = entity.ReasonOfDecline,
                StatusOfReport = entity.StatusOfReport,
            };
            if (entity.TrainingInvitation != null)
            {
                dto.ReferenceNumber = entity.TrainingInvitation.ReferenceNumber;
                dto.ServiceProvider = entity.TrainingInvitation.ServiceProvider;
                dto.CategoryName = entity.TrainingInvitation.TrainingRequest == null ? string.Empty : entity.TrainingInvitation.TrainingRequest.Category != null ? entity.TrainingInvitation.TrainingRequest.Category.Name : string.Empty;
                dto.CourseName = entity.TrainingInvitation.CourseName;
                dto.TrainerName = entity.TrainingInvitation.TrainerName;
                dto.TrainingLocation = entity.TrainingInvitation.TrainingLocation;
                dto.StartDate = entity.TrainingInvitation.StartDate;
                dto.EndDate = entity.TrainingInvitation.EndDate;
                dto.CreateDate = entity.TrainingInvitation.Created;
                dto.AfterTrainingReportNotRequired = entity.TrainingInvitation.AfterTrainingReportNotRequired;
                dto.AfterTrainingReportDeadline = entity.TrainingInvitation.AfterTrainingReportDeadline;
                dto.CourseType = entity.TrainingInvitation.TrainingRequest.TypeOfTraining;
                dto.NumberOfParticipant = entity.TrainingInvitation.TrainingInvitationParticipants.Count;
                dto.Requester = entity.TrainingInvitation.CreatedByFullName;
            }

            return dto;
        }
        public static List<TrainingInvitationParticipantDto> ToDtos(this IList<TrainingInvitationParticipant> entities)
        {
            var list = new List<TrainingInvitationParticipantDto>();
            foreach (var c in entities)
                list.Add(c.ToDto());
            return list;
        }
    }
}