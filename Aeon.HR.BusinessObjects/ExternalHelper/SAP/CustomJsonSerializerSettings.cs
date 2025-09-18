using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.ExternalHelper.SAP
{
    public class CustomJsonSerializerSettings
    {
        public JsonSerializerSettings Setting { get; set; }
        public object SynceData { get; set; }
    }
}
