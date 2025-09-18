using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Helpers
{
    public static class GenericExtension<T>
    {
        public static T DeserializeObject(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
        public static T GetCustomAttributeInfo(object instance)
        {            
            return (T)instance.GetType().GetCustomAttributes(false).FirstOrDefault();
        }
    }
}
