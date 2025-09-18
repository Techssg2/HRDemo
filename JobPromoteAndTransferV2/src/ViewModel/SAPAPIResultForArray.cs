
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2.src.ViewModel
{
    public class SAPAPIResultForArray
    {
        public SAPAPIResultDetailForArray D { get; set; }
    }
    public class SAPAPIResultForSingleItem
    {
        public object D { get; set; }
    }
    public class SAPAPIResultDetailForArray
    {
        public List<object> Results { get; set; }
    }

    public class CustomJsonSerializerSettings
    {
        public JsonSerializerSettings Setting { get; set; }
        public object SynceData { get; set; }
    }
}
