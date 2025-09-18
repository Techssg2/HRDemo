using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Utilities
{
    public static class Enums
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                 (DescriptionAttribute[])fi.GetCustomAttributes(
                 typeof(DescriptionAttribute),
                 false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
