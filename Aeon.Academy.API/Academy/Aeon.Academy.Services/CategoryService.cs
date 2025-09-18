using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<Category> repository = null;

        public CategoryService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.GetRepository<Category>();
        }

        public Category Get(Guid id)
        {
            return repository.Get(id);
        }

        public IList<Category> ListAll(bool includeDeactived = false)
        {
            if (includeDeactived)
            {
                return repository.GetAll().ToList();
            }
            return repository.Query(q => q.IsActivated).ToList();
        }

        public Guid Save(Category category)
        {
            if (category.Id == Guid.Empty)
            {
                repository.Add(category);
            }
            else
            {
                repository.Update(category);
            }

            unitOfWork.Complete();

            return category.Id;
        }

        public bool Delete(Category category)
        {
            if (category == null || category.Id == Guid.Empty) return false;

            var existing = repository.Get(category.Id);
            if (existing == null) return false;
            if (repository.Query(c => c.ParentId == category.Id && c.IsActivated).Count() > 0)
            {
                return false;
            }
            var courseRepo = unitOfWork.GetRepository<Course>();
            if (courseRepo.Query(c => c.CategoryId == category.Id && c.IsActivated).Count() > 0)
            {
                return false;
            }
            category.IsActivated = false;
            repository.Update(category);
            unitOfWork.Complete();
            return true;
        }

        public bool Validate(Category category)
        {
            if (category.Id == Guid.Empty)
            {
                bool existing = repository.Query(c => c.Name.Equals(category.Name)).Any();
                return !existing;
            }
            else
            {
                bool existing = repository.Query(c => !c.Id.Equals(category.Id) && c.Name.Equals(category.Name)).Any();
                return !existing;
            }
        }
    }
}
