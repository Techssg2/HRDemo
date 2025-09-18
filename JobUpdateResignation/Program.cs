using JobResignation.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobResignation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingResignation.ProcessingAPI();
        }
    }
}
