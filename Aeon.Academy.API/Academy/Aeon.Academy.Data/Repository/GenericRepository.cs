using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Aeon.Academy.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IDataEntity
    {
        protected readonly DbContext context;
        public GenericRepository(DbContext context)
        {
            this.context = context;
        }

        public T Get(Guid id)
        {
            return context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate);
        }

        public void Add(T entity)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            var existing = Get(entity.Id);
            context.Entry(existing).CurrentValues.SetValues(entity);
        }
    }
}
