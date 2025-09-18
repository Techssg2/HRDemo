using Aeon.Academy.API;
using Unity;

namespace Aeon.Bootstrapper
{
    public class ModuleLoader
    {
        public static void Initialize(UnityContainer container)
        {
            UnityConfig.RegisterComponents(container);
        }
    }
}