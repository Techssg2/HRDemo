using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ActionAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
