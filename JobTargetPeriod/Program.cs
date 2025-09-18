using JobTargetPeriod.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTargetPeriod
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingTargetPeriod.ProcessingAPI();
        }
    }
}
