using JobAutoRetryPayloadSAP.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobAutoRetryPayloadSAP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingAutoRetryPayloadSAP.ProcessingAPI();
        }
    }
}
