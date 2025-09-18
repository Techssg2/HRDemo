using Aeon.Academy.API.DTOs;
using Aeon.Academy.Common.Consts;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingInvitationMapper
    {
        public static TrainingInvitation ToEntity(this TrainingInvitationDto dto, TrainingInvitation trainingInvitation, User user)
        {
            if (trainingInvitation == null) trainingInvitation = new TrainingInvitation()
            {
                Created = DateTimeOffset.Now,
                CreatedBy = user.LoginName,
                CreatedByFullName = user.FullName,
                CreatedById = user.Id,
                CreatedBySapCode = user.SapCode,
            };

            trainingInvitation.Id = dto.Id;
            trainingInvitation.Modified = DateTimeOffset.Now;
            trainingInvitation.ModifiedBy = user.LoginName;
            trainingInvitation.ModifiedByFullName = user.FullName;
            trainingInvitation.ModifiedById = user.Id;

            trainingInvitation.TrainingRequestId = dto.TrainingRequestId;
            trainingInvitation.ReferenceNumber = dto.ReferenceNumber;
            trainingInvitation.CategoryId = dto.CategoryId;
            trainingInvitation.CourseId = dto.CourseId;
            trainingInvitation.CourseName = dto.CourseName;
            trainingInvitation.ServiceProvider = dto.ServiceProvider;
            trainingInvitation.TrainerId = dto.TrainerId;
            trainingInvitation.TrainerName = dto.TrainerName;
            trainingInvitation.StartDate = dto.StartDate;
            trainingInvitation.EndDate = dto.EndDate;

            trainingInvitation.TotalOnlineTrainingHours = dto.TotalOnlineTrainingHours;
            trainingInvitation.TotalOfflineTrainingHours = dto.TotalOfflineTrainingHours;
            trainingInvitation.TrainingLocation = dto.TrainingLocation;
            trainingInvitation.AfterTrainingReportNotRequired = dto.AfterTrainingReportNotRequired;
            trainingInvitation.AfterTrainingReportDeadline = dto.AfterTrainingReportDeadline;
            trainingInvitation.Content = dto.Content;
            trainingInvitation.HoursPerDay = dto.HoursPerDay;
            trainingInvitation.NumberOfDays = dto.NumberOfDays;
            trainingInvitation.TotalHours = dto.TotalHours;
            trainingInvitation.Note = dto.Note;

            trainingInvitation.TrainingInvitationParticipants = new List<TrainingInvitationParticipant>();
            trainingInvitation.TrainingInvitationParticipants.Clear();
            if (dto.Participants != null)
            {
                foreach (var item in dto.Participants)
                {
                    var emailContent = dto.Content.Replace(InvitationEmailTemplate.EmployeeName, item.Name).Replace(InvitationEmailTemplate.TrainingLocation, trainingInvitation.TrainingLocation)
                        .Replace(InvitationEmailTemplate.Coursename, trainingInvitation.CourseName).Replace(InvitationEmailTemplate.StartDate, trainingInvitation.StartDate.ToString("MMMM d, yyyy"));
                    emailContent = string.IsNullOrEmpty(trainingInvitation.ServiceProvider)
                        ? emailContent.Replace(InvitationEmailTemplate.ServiceProvider, InvitationEmailTemplate.AeonVN)
                        : emailContent.Replace(InvitationEmailTemplate.ServiceProvider, trainingInvitation.ServiceProvider);
                    emailContent = trainingInvitation.AfterTrainingReportNotRequired
                        ? emailContent.Replace(InvitationEmailTemplate.IgnoreSectionIfNotReportEN, "<br>").Replace(InvitationEmailTemplate.IgnoreSectionIfNotReportVN, "<br>").Replace("<br><br><br>", "<br><br>")
                        : emailContent.Replace(InvitationEmailTemplate.AfterTrainingReportDeadline, trainingInvitation.AfterTrainingReportDeadline.ToString("MMMM d, yyyy")); ;

                    trainingInvitation.TrainingInvitationParticipants.Add(new TrainingInvitationParticipant()
                    {
                        Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
                        TrainingInvitationId = trainingInvitation.Id,
                        ParticipantId = item.ParticipantId,
                        SapCode = item.SapCode,
                        Name = item.Name,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Position = item.Position,
                        Department = item.Department,
                        EmailContent = emailContent,
                        Response = ResponseType.Pending,
                        ReasonOfDecline = item.ReasonOfDecline,
                        StatusOfReport = item.StatusOfReport,
                        SapStatusCode = (int)SAPStatusCode.NotSubmit
                    });
                }
            }
            return trainingInvitation;
        }

        public static TrainingInvitationDto ToDto(this TrainingInvitation entity)
        {
            var dto = new TrainingInvitationDto
            {
                Id = entity.Id,
                TrainingRequestId = entity.TrainingRequestId,
                ReferenceNumber = entity.ReferenceNumber,
                CategoryId = entity.CategoryId,
                CategoryName = entity.TrainingRequest == null ? string.Empty : entity.TrainingRequest.Category != null ? entity.TrainingRequest.Category.Name : string.Empty,
                CourseId = entity.CourseId,
                CourseName = entity.CourseName,
                ServiceProvider = entity.ServiceProvider,
                TrainerId = entity.TrainerId,
                TrainerName = entity.TrainerName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                TotalOfflineTrainingHours = entity.TotalOfflineTrainingHours,
                TotalOnlineTrainingHours = entity.TotalOnlineTrainingHours,
                TrainingLocation = entity.TrainingLocation,
                AfterTrainingReportNotRequired = entity.AfterTrainingReportNotRequired,
                AfterTrainingReportDeadline = entity.AfterTrainingReportDeadline,
                Content = entity.Content,
                Status = entity.Status,
                HoursPerDay = entity.HoursPerDay,
                NumberOfDays = entity.NumberOfDays,
                TotalHours = entity.TotalHours,
                Note = entity.Note,
                CreateDate = entity.Created,
                CreatedBySapCode = entity.CreatedBySapCode,
                NumberOfParticipant = 0,
            };
            if (entity.TrainingInvitationParticipants != null)
            {
                dto.NumberOfParticipant = entity.TrainingInvitationParticipants.Count;
                foreach (var item in entity.TrainingInvitationParticipants)
                {
                    dto.Participants.Add(new TrainingInvitationParticipantDto()
                    {
                        Id = item.Id,
                        TrainingInvitationId = item.TrainingInvitationId,
                        ParticipantId = item.ParticipantId,
                        SapCode = item.SapCode,
                        Name = item.Name,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Position = item.Position,
                        Department = item.Department,
                        EmailContent = item.EmailContent,
                        Response = item.Response,
                        ReasonOfDecline = item.ReasonOfDecline,
                        StatusOfReport = item.StatusOfReport
                    });
                }
            }
            return dto;
        }
        public static List<TrainingInvitationDto> ToDtos(this IList<TrainingInvitation> entities)
        {
            var list = new List<TrainingInvitationDto>();
            foreach (var c in entities)
                list.Add(c.ToDto());
            return list;
        }
        public static IList<WorkflowTaskDto> ToMyTaskDtos(this IList<TrainingInvitation> requests, Guid userId, string status)
        {
            var dtos = new List<WorkflowTaskDto>();

            if (requests == null) return dtos;

            var itemType = typeof(TrainingInvitation).Name;

            foreach (var req in requests)
            {
                var dto = new WorkflowTaskDto
                {
                    Created = req.Created,
                    DueDate = req.AfterTrainingReportDeadline.ToString("dd/MM/yyyy") != "01/01/1970" ? req.AfterTrainingReportDeadline : DateTimeOffset.Now.AddDays(7),
                    ItemType = itemType,
                    ReferenceNumber = req.ReferenceNumber,
                    RequestedDepartmentId = req.DepartmentId,
                    RequestedDepartmentName = req.DepartmentName,
                    RequestedDepartmentCode = string.Empty,
                    RequestorFullName = req.CreatedByFullName,
                    RequestorUserName = req.CreatedBy,
                    RequestorId = req.CreatedById,
                    Status = status,
                    RegionName = string.Empty,
                };
                if (req.TrainingInvitationParticipants != null)
                {
                    foreach (var item in req.TrainingInvitationParticipants)
                    {
                        if (item.ParticipantId == userId)
                        {
                            dto.ItemId = item.Id;
                            break;
                        }
                    }
                }
                dtos.Add(dto);
            }

            return dtos;
        }

    }
}