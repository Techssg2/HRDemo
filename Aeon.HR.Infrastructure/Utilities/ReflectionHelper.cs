using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Aeon.HR.Infrastructure.Attributes;

namespace Aeon.HR.Infrastructure.Utilities
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public static string ReadResourceFile<T>(string filename)
        {
            var thisAssembly = Assembly.GetAssembly(typeof(T));
            using (var stream = thisAssembly.GetManifestResourceStream(filename))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
