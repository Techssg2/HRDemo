using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ExportReportArg
    {
        public string Predicate { get; set; }
        public object[] PredicateParameters { get; set; }
        public string[] Codes { get; set; }
        public string Order { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
