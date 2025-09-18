using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity;
using Unity.Lifetime;

namespace Aeon.HR.Infrastructure.Utilities
{
    public static class Extensions
    {
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static int GetNumberFromString(this string source)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(source))
            {
                var numString = new string(source.Where(c => char.IsDigit(c)).ToArray());
                if (!string.IsNullOrEmpty(numString))
                {
                    Int32.TryParse(numString, out result);
                }
            }
            return result;
        }
        public static T ToEntity<T>(this object source)
        {
            return Mapper.Map<T>(source);
        }
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

        public static void BindInSingletonScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new ContainerControlledLifetimeManager());
        }
    }
}
