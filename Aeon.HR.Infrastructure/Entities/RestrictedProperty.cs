using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Entities
{
    public class RestrictedProperty
    {
        public string Name { get; set; }
        public string FieldPattern { get; set; }
        public bool IsRequired { get; set; }
    }
}
