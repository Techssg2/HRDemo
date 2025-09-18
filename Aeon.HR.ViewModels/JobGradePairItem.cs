using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class JobGradePairItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public int Grade { get; set; }
    }
}