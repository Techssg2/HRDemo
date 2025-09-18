using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;

namespace Aeon.Academy.Services
{
    public interface ICategoryService
    {
        bool Delete(Category category);
        Category Get(Guid id);
        IList<Category> ListAll(bool includeDeactived = false);
        Guid Save(Category category);
        bool Validate(Category category);
    }
}