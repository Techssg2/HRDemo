using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Aeon.HR.Infrastructure.Utilities
{
    public class ServiceLocator : IDisposable
    {
        #region "Singleton Stuff"
        private static ServiceLocator serviceLocator;
        private static readonly object lockObj = new object();
        private IUnityContainer container;

        private ServiceLocator()
        {

        }

        private static ServiceLocator Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (serviceLocator == null)
                        serviceLocator = new ServiceLocator();
                }
                return serviceLocator;
            }
        }
        #endregion

        #region "Get Methods"
        public static T Resolve<T>()
        {
            var instance = Instance.container.Resolve<T>();
            return instance == null ? default(T) : instance;
        }

        public static object Resolve(Type type)
        {
            return Instance.container.Resolve(type);
        }

        public static T Resolve<T>(string name)
        {
            var instance = Instance.container.Resolve<T>(name);
            return instance == null ? default(T) : instance;
        }

        public static object Resolve(Type type, string name)
        {
            return Instance.container.Resolve(type, name);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            var instances = Instance.container.ResolveAll<T>();
            return instances;
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            var instances = Instance.container.ResolveAll(type);
            return instances;
        }
        #endregion

        #region "Container"
        public static void SetContainer(IUnityContainer container)
        {
            Instance.container = container;
            var type = Type.GetType("Aeon.Bootstrapper.ModuleLoader, Aeon.Bootstrapper", false);
            if (type != null) type.GetMethod("Initialize").Invoke(null, new object[] { container });
        }
        #endregion

        #region "IDisposable"
        public void Dispose()
        {
            serviceLocator = null;
        }
        #endregion
    }
}
