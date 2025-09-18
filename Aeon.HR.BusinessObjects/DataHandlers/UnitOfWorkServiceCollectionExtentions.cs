using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Unity;
using Aeon.HR.Infrastructure.Utilities;

namespace Aeon.HR.BusinessObjects.DataHandlers
{
    public static class UnitOfWorkServiceCollectionExtentions
    {
        public static IUnityContainer AddUnitOfWork<TContext>(this UnityContainer container)
             where TContext : DbContext
        {
            container.BindInRequestScope<IUnitOfWork, UnitOfWork<TContext>>();
            return container;
        }
    }
}
