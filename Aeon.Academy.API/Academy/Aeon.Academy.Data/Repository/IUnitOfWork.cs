using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Aeon.Academy.Data.Repository
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        IGenericRepository<T> GetRepository<T>() where T : class, IDataEntity;
        int Complete();
    }
}
