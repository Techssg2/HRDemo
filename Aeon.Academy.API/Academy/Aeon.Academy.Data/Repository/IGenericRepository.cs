using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Data.Repository
{
    public interface IGenericRepository<T> where T : class, IDataEntity
    {
        T Get(Guid id);
        IEnumerable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
    }

}
