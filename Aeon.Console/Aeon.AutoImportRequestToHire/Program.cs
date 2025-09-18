using Aeon.AutoImportRequestToHire.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.AutoImportRequestToHire
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingAutoImportRequestToHire.ProcessingAPI();
        }
    }
}
