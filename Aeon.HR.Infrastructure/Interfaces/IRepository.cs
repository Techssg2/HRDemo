using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        IEnumerable<T> GetAll(string order = "", params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll(string order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(string order = "", params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(string order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync<TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit, params Expression<Func<T, object>>[] includeProperties);
        T FindById(Guid id, params Expression<Func<T, object>>[] includeProperties);
        Task<T> FindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);
        Task<T> ITFindByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);
        T GetSingle(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties); 
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties);
        // Dung cho select cac record theo dieu kien
        Task<T> GetSingleAsyncIsNotDeleted(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties); 
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> ITFindBy(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate, string order = "", params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindByAsync(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(string predicate, object[] parameters, string order = "", params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> FindBy(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindByAsync(string predicate, object[] parameters, string order = "", params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindByAsync(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, string predicate, object[] parameters, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<H> GetAll<H>(string order = "");
        IEnumerable<H> GetAll<H>(string order, int pageIndex, int limit);
        Task<IEnumerable<H>> GetAllAsync<H>(string order = "");
        Task<IEnumerable<H>> GetAllAsync<H>(string order, int pageIndex, int limit);
        Task<IEnumerable<H>> GetAllAsync<H, TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit);
        H FindById<H>(Guid id);
        Task<H> FindByIdAsync<H>(Guid id);
        Task<H> ITFindByIdAsync<H>(Guid id);
        H GetSingle<H>(Expression<Func<T, bool>> predicate, string order = "");
        Task<H> GetSingleAsync<H>(Expression<Func<T, bool>> predicate, string order = "");
        IEnumerable<H> FindBy<H>(Expression<Func<T, bool>> predicate, string order = "");
        IEnumerable<H> FindBy<H>(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<H>> FindByAsync<H>(Expression<Func<T, bool>> predicate, string order = "");
        Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<H>> FindByAsync<H, TKey>(Expression<Func<T, TKey>> order, int pageIndex, int limit, Expression<Func<T, bool>> predicate);
        IEnumerable<H> FindBy<H>(string predicate, object[] parameters, string order = "");
        IEnumerable<H> FindBy<H>(string order, int pageIndex, int limit, string predicate, object[] parameters);
        Task<IEnumerable<H>> FindByAsync<H>(string predicate, object[] parameters, string order = "");
        Task<IEnumerable<H>> FindByAsync<H>(string order, int pageIndex, int limit, string predicate, object[] parameters);
        int CountAll();
        Task<int> CountAllAsync();
        int Count(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        int Count(string predicate, object[] parameters);
        bool Any(string predicate, object[] parameters);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(string predicate, object[] parameters);
        Task<int> CountAsync(string predicate, object[] parameters);
        void Add(T entity);
        /// <summary>
        /// Thêm bản ghi nhưng giữ nguyên ID
        /// </summary>
        /// <param name="entity">The entity.</param>        
        void AddWithId(T entity);
        void Delete(T entity);
        void Update(T entity);
        void Add(IEnumerable<T> entities);
        void Delete(IEnumerable<T> entities);
        void Update(IEnumerable<T> entities);
        Task<List<TResult>> ExecuteRawSqlQueryAsync<TResult>(string sql, params SqlParameter[] parameters) where TResult : class, new();
    }
}
