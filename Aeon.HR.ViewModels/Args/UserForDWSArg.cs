using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    public class UserForDWSArg
    {
        public UserForDWSArg()
        {
            Items = new List<DeptForTarget>();
        }
        //public Guid[] Ids { get; set; }
        public List<DeptForTarget> Items { get; set; }
        public Guid? PeriodId { get; set; }
        public string[] ActiveUsers { get; set; }
        public QueryArgs Query { get; set; }
        public bool IsNoDivisionChosen { get; set; }
        public Guid? DivisionId { get; set; }
        public TypeTaget Type { get; set; }
        public string LoginName { get; set; }
    }
}
