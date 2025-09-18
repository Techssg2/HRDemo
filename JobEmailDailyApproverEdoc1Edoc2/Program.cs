using JobEmailDailyApproverEdoc1Edoc2.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobEmailDailyApproverEdoc1Edoc2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingDailyEmailApprover.ProcessingAPI();
        }
    }
}
