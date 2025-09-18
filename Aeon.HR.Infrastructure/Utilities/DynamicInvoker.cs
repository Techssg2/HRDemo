using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Utilities
{
    public static class DynamicInvoker
    {
        public static object InvokeGeneric(object objInstance, string methodName, Type type, params object[] prs)
        {
            var method = objInstance.GetGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var generic = method.MakeGenericMethod(type);
            var result = generic.Invoke(objInstance, prs);
            return result;
        }
        public static object Invoke(object objInstance, string methodName, params object[] prs)
        {
            var method = objInstance.GetNonGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var result = method.Invoke(objInstance, prs);
            return result;
        }
        public static object InvokeGeneric(object objInstance, string methodName, string type, params object[] prs)
        {
            var method = objInstance.GetGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var generic = method.MakeGenericMethod(Type.GetType(type));
            var result = generic.Invoke(objInstance, prs);
            return result;
        }
        public static async Task<object> InvokeGenericAsync(object objInstance, string methodName, Type type, params object[] prs)
        {
            var method = objInstance.GetGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var generic = method.MakeGenericMethod(type);
            var task = (Task)generic.Invoke(objInstance, prs);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
        public static async Task<object> InvokeAsync(object objInstance, string methodName, params object[] prs)
        {
            var method = objInstance.GetNonGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var task = (Task)method.Invoke(objInstance, prs);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
        public static async Task<object> InvokeGenericAsync(object objInstance, string methodName, string type, params object[] prs)
        {
            var method = objInstance.GetGenericMethod(methodName, prs.Select(x => x.GetType()).ToArray());
            var generic = method.MakeGenericMethod(Type.GetType(type));
            var task = (Task)generic.Invoke(objInstance, prs);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        private static MethodInfo GetGenericMethod(this object objectInstance, string methodName, Type[] types)
        {
            return objectInstance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, new GenericBinder(), types, null);
        }

        private static MethodInfo GetNonGenericMethod(this object objectInstance, string methodName, Type[] types)
        {
            return objectInstance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, new NonGenericBinder(), types, null);
        }
    }
}
