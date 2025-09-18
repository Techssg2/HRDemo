using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class ExportRevokingArg
    {
        public string Predicate { get; set; }
        public object[] PredicateParameters { get; set; }
        public string[] Statuses { get; set; }
    }
}
