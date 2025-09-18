using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface ICourseService
    {
        bool Delete(Course course);
        Course Get(Guid id);
        IList<Course> GetAll(bool includeDeactived = false);
        IList<Course> ListByCategoryId(Guid categoryId, bool includeDeactived = false);
        Guid Save(Course course);
        bool Validate(Course course);
    }
}