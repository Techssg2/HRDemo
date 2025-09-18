using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.ExternalHelper.SAP
{
    public class SAPAPIResultForArray
    {
        public SAPAPIResultDetailForArray D { get; set; }
    }
    public class SAPAPIResultForSingleItem
    {
        public object D { get; set; }
    }
}
