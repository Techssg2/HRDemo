using Aeon.UpdateUserInfoFromSapJob.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.UpdateUserInfoFromSapJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingUpdateUserInfoFromSapJob.ProcessingAPI();
        }
    }
}
