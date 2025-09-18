using JobSendMail1STResignations.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignations
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingSendMail1STResignations.ProcessingAPI();
        }
    }
}
