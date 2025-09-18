using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IEntityBaseRepository<T> where T : class, IEntity, new()
    {
        void Load(T entity, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll(string order = "");
        IEnumerable<T> GetAll(string order, int pageIndex, int limit);
        Task<IEnumerable<T>> GetAllAsync(string order = "", params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(string order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties);
        T FindById(Guid id);
        Task<T> FindByIdAsync(Guid id);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, string order = "");
        IEnumerable<T> FindBy(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate, string order = "");
        Task<IEnumerable<T>> FindByAsync(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate);
        int CountAll();
        Task<int> CountAllAsync();
        int Count(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Add(IEnumerable<T> entities);
        void Delete(IEnumerable<T> entities);
        void Edit(IEnumerable<T> entities);
        int Commit();
        Task<int> CommitAsync();
    }
}
