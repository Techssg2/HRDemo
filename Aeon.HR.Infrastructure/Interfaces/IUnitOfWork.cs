using Aeon.HR.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity, new();
        IRepository<T> GetRepository<T>(bool forceAllItems) where T : class, IEntity, new();
        int Commit();
        Task<int> CommitAsync();
        void RefreshContext(IEntity entity);
        DataSet ExecuteQuery(string sql, Dictionary<string, object> parameters);
        DataSet ExecuteQuerySQL(DbConnection conn, DbTransaction transaction, string sql, Dictionary<string, object> parameters);
        DataSet QuerySQL(string sql, Dictionary<string, object> parameters);
        UserContext UserContext { get; set; }
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
