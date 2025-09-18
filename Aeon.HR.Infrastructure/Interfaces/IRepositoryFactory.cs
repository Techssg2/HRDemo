using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, IEntity, new();
        IRepository<T> GetRepository<T>(bool forceAllItems) where T : class, IEntity, new();
    }
}
