using JobResignationV2.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignationV2
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var job = new ResignationJob();
            await job.Run();
            Utilities.WriteLogError("Resignation job executed.");
        }
    }
}
