using JobSendMail1STResignationsV2.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignationsV2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Utilities.CleanupOldLogs(30);
                Utilities.WriteLogError("//----------------------------------------JobSendMail1STResignations started-----------------------------------------//");
                var job = new JobSendMail1STResignations();
                await job.Run();
                Utilities.WriteLogError("//---------------------------------JobSendMail1STResignations completed successfully---------------------------------//");
                Utilities.WriteLogError("\n\n");
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("//-----------------------------------------JobSendMail1STResignations failed-----------------------------------------//");
                Utilities.WriteLogError(ex);
            }
        }
    }
}
