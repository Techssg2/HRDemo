using Aeon.Academy.API.DTOs;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.Mappers
{
    public static class TrainingRequestHistoryMapper
    {
        public static TrainingRequestHistoryDto ToDto(this TrainingRequestHistory entity)
        {
            var dto = new TrainingRequestHistoryDto
            {
                Id = entity.Id,
                TrainingRequestId = entity.TrainingRequestId,
                Created = entity.Created != DateTimeOffset.MinValue ? entity.Created : (DateTimeOffset?) null,
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

        public static List<TrainingRequestHistoryDto> ToDto(this IList<TrainingRequestHistory> entities)
        {
            var dtos = new List<TrainingRequestHistoryDto>();

            foreach(var c in entities)
            {
                dtos.Add(c.ToDto());
            }

            return dtos;
        }
    }
}