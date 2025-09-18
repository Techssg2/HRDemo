using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobPromoteAndTransferV2.src;

namespace JobPromoteAndTransferV2
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var job = new JobPromoteAndTransferV2.src.JobPromoteAndTransfer();
            await job.Run();
            Console.WriteLine("Resignation job executed.");
        }
    }
}
