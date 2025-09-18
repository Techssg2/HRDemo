using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class Edoc1Arg
    {
        public string LoginName { get; set; }
        public int Top { get; set; }
        public int Skip { get; set; }
        public string OrderBy { get; set; }
    }
}
