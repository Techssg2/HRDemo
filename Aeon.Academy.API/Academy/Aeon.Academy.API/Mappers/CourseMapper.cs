using Aeon.Academy.API.DTOs;
using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.Academy.API.Mappers
{
    public static class CourseMapper
    {
        public static Course ToEntity(this CourseDto dto, Course course,User user)
        {
            if (course == null) course = new Course()
            {
                IsActivated = true,
                Created = DateTimeOffset.Now,
                CreatedBy = user.LoginName,
                CreatedByFullName = user.FullName,
                CreatedById = user.Id,
                Id = Guid.Empty
            };
            course.Id = dto.Id;
            course.CategoryId = dto.CategoryId;
            course.CategoryName = dto.CategoryName;
            course.Name = dto.Name;
            course.Code = dto.Code;
            course.Type = dto.Type;
            course.ServiceProvider = dto.ServiceProvider;
            course.Description = dto.Description;
            course.Duration = dto.Duration;
            course.Image = dto.Image;
            course.ImageName = dto.ImageName;
            course.Modified = DateTimeOffset.Now;
            course.ModifiedBy = user.LoginName;
            course.ModifiedByFullName = user.FullName;
            course.ModifiedById = user.Id;
            course.ServiceProviderCode = dto.ServiceProviderCode;

            return course;
        }

        public static CourseDto ToDto(this Course entity)
        {
            var dto = new CourseDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                Name = entity.Name,
                Code = entity.Code,
                Type = entity.Type,
                ServiceProvider = entity.ServiceProvider,
                Description = entity.Description,
                Duration = entity.Duration,
                Image = entity.Image,
                ImageName = entity.ImageName,
                IsActivated = entity.IsActivated,
                ServiceProviderCode = entity.ServiceProviderCode,
            };

            return dto;
        }

        public static List<CourseDto> ToDtos(this IList<Course> entities)
        {
            var dtos = new List<CourseDto>();

            foreach (var c in entities)
            {
                dtos.Add(c.ToDto());
            }

            return dtos;
        }
    }
}