using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.HR.Infrastructure
{
    public class BatchQuery
    {
        public string Entity { get; set; }
        public QueryArgs QueryArgs { get; set; }
    }
}
