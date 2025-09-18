using Aeon.Academy.API.DTOs;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.Mappers
{
    public static class CategoryMapper
    {
        public static Category ToEntity(this CategoryDto dto, Category category, User user)
        {
            if (category == null) category = new Category()
            {
                IsActivated = true,
                Created = DateTimeOffset.Now,
                CreatedBy = user.LoginName,
                CreatedByFullName = user.FullName,
                CreatedById = user.Id,
                Id = Guid.Empty
            };

            category.Name = dto.Name.Trim();
            category.ParentId = dto.ParentId;
            category.Modified = DateTimeOffset.Now;
            category.ModifiedBy = user.LoginName;
            category.ModifiedByFullName = user.FullName;
            category.ModifiedById = user.Id;

            return category;
        }

        public static CategoryDto ToDto(this Category entity)
        {
            var dto = new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ParentId = entity.ParentId,
                IsActivated = entity.IsActivated
            };
            return dto;
        }

        public static List<CategoryDto> ToDtos(this IList<Category> entities)
        {
            var dtos = new List<CategoryDto>();

            foreach (var c in entities)
            {
                dtos.Add(c.ToDto());
            }

            return dtos;
        }
    }
}