using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class MasterExternalDataViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public dynamic RawData { get; set; }
        public string NameVN { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
