using Aeon.Academy.API.DTOs;
using Aeon.Academy.Data.Entities;
using System.Collections.Generic;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingReportHistoryMapper
    {
        public static TrainingReportHistoryDto ToDto(this TrainingReportHistory entity)
        {
            var dto = new TrainingReportHistoryDto
            {
                Id = entity.Id,
                TrainingReportId = entity.TrainingReportId,
                Created = entity.Created,
                CreatedById = entity.CreatedById,
                CreatebBy = entity.CreatebBy,
                CreatedByFullName = entity.CreatedByFullName,
                ReferenceNumber = entity.ReferenceNumber,
                Comment = entity.Comment,
                Action = entity.Action,
                StepNumber = entity.StepNumber,
                AssignedToDepartmentName = entity.AssignedToDepartmentName,
                StartDate = entity.StartDate,
                DueDate = entity.DueDate,
                RoundNumber = entity.RoundNumber
            };
            return dto;
        }

        public static List<TrainingReportHistoryDto> ToDto(this IList<TrainingReportHistory> entities)
        {
            var dtos = new List<TrainingReportHistoryDto>();

            foreach(var c in entities)
            {
                dtos.Add(c.ToDto());
            }

            return dtos;
        }
    }
}