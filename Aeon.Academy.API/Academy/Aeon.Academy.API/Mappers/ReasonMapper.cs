using Aeon.Academy.API.DTOs;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.API.Mappers
{
    public static class ReasonMapper
    {
        public static ReasonOfTrainingRequest ToEntity(this ReasonDto dto, ReasonOfTrainingRequest reason)
        {
            if (reason == null) reason = new ReasonOfTrainingRequest
            {
                CreatedDate = DateTime.Now,
                Id = Guid.Empty
            };
            reason.Value = dto.Value.Trim();
            return reason;
        }

        public static ReasonDto ToDto(this ReasonOfTrainingRequest entity)
        {
            var dto = new ReasonDto
            {
                Id = entity.Id,
                Value = entity.Value
            };
            return dto;
        }

        public static List<ReasonDto> ToDtos(this IList<ReasonOfTrainingRequest> entities)
        {
            var dtos = new List<ReasonDto>();

            foreach (var c in entities)
            {
                dtos.Add(c.ToDto());
            }

            return dtos;
        }
    }
}