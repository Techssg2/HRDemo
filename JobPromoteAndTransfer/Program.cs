using JobPromoteAndTransfer.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPromoteAndTransfer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingPromoteAndTransfer.ProcessingAPI();
        }
    }
}
