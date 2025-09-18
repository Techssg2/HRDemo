using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Academy.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork = null;
        private readonly IGenericRepository<Course> repository = null;

        public CourseService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.repository = unitOfWork.GetRepository<Course>();
        }

        public Course Get(Guid id)
        {
            return repository.Get(id);
        }

        public IList<Course> GetAll(bool includeDeactived = false)
        {
            if (includeDeactived)
            {
                return repository.GetAll().ToList();
            }
            return repository.Query(q => q.IsActivated).ToList();
        }

        public IList<Course> ListByCategoryId(Guid categoryId, bool includeDeactived = false)
        {
            var repoCategories = unitOfWork.GetRepository<Category>();
            var categories = repoCategories.GetAll().ToList();
            var list = new List<Guid>() {
                categoryId
            };
            ListCategoryTree(categoryId, categories, list);
            if (includeDeactived)
            {
                repository.Query(c => list.Contains(c.CategoryId)).ToList(); ;
            }
            return repository.Query(c => list.Contains(c.CategoryId) && c.IsActivated).ToList();
        }
        private void ListCategoryTree(Guid id, List<Category> categories, List<Guid> list)
        {
            var childIds = categories.Where(x => x.ParentId == id).Select(x => x.Id);

            if (childIds != null && childIds.Any())
            {
                list.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    ListCategoryTree(childId, categories, list);
                }
            }
        }

        public Guid Save(Course course)
        {
            if (course.Id == Guid.Empty)
            {
                repository.Add(course);
            }
            else
            {
                var category = unitOfWork.GetRepository<Category>().Get(course.CategoryId);
                if (category == null || !category.IsActivated)
                    return Guid.Empty;
                repository.Update(course);
            }
            unitOfWork.Complete();

            return course.Id;
        }

        public bool Delete(Course course)
        {
            if (course == null || course.Id == Guid.Empty) return false;

            var existing = repository.Get(course.Id);
            if (existing == null) return false;

            var trainingRequestRepo = unitOfWork.GetRepository<TrainingRequest>();
            if (trainingRequestRepo.Query(r => r.CourseId == course.Id).Count() > 0)
            {
                return false;
            }
            course.IsActivated = false;
            repository.Update(course);
            unitOfWork.Complete();
            return true;
        }
        public bool Validate(Course course)
        {
            if (course.Id == Guid.Empty)
            {
                bool existing = repository.Query(c => c.Name.Equals(course.Name)).Any();
                return !existing;
            }
            else
            {
                bool existing = repository.Query(c => !c.Id.Equals(course.Id) && c.Name.Equals(course.Name)).Any();
                return !existing;
            }
        }
    }
}
