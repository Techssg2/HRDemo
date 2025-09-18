using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.HR.Infrastructure
{
    public class BatchAction
    {
        public string Entity { get; set; }
        public CommonEntityAction Operation { get; set; }
        public dynamic Data { get; set; }
    }
}
