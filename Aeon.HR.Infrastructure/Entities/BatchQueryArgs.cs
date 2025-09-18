using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.HR.Infrastructure
{
    public class BatchQueryArgs
    {
        public IList<BatchQuery> Queries { get; set; }
    }
}
