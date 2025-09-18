using Aeon.Academy.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Aeon.Academy.Data.Repository
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext: DbContext
    {
        private Dictionary<Type, object> repositoryList = new Dictionary<Type, object>();
        private readonly DbContext context;
        
        public UnitOfWork(TContext dbContext)
        {
            this.context = dbContext;
        }
        public IGenericRepository<T> GetRepository<T>() where T : class, IDataEntity
        {
            if (repositoryList.ContainsKey(typeof(T)))
            {
                if (repositoryList[typeof(T)] != null) return repositoryList[typeof(T)] as IGenericRepository<T>;
            }
                
            repositoryList[typeof(T)] = new GenericRepository<T>(context);
            return repositoryList[typeof(T)] as IGenericRepository<T>;
        }

        public int Complete()
        {
            return context.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
    }
}
